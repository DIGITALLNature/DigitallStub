// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using System.Text.RegularExpressions;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Logic.Queries;

public partial class LinkedEntitiesProcessor(FakeOrganizationService state, QueryProcessor queryProcessor)
{
    private readonly Dictionary<string, int> _linkedEntities = new();

        public IQueryable<Entity> FilterQuery(QueryExpression qe, IQueryable<Entity> query)
        {
            // Add as many Joins as linked entities
            foreach (var le in qe.LinkEntities)
            {
                Validators.ValidateLinkedAliases(qe, le);
                if (string.IsNullOrWhiteSpace(le.EntityAlias))
                {
                    le.EntityAlias = EnsureUniqueLinkedEntityAlias(le.LinkToEntityName);
                }

                query = TranslateLinkedEntityToLinq(le, query);
            }

            return query;
        }

        private  string EnsureUniqueLinkedEntityAlias(string entityName)
        {
            if (_linkedEntities.TryGetValue(entityName, out var value))
            {
                _linkedEntities[entityName] = ++value;
            }
            else
            {
                _linkedEntities[entityName] = 1;
            }

            return $"{entityName}{_linkedEntities[entityName]}";
        }

        private IQueryable<Entity> TranslateLinkedEntityToLinq(LinkEntity le, IQueryable<Entity> query, string linkFromAlias = "", string linkFromEntity = "")
        {
            if (!string.IsNullOrEmpty(le.EntityAlias))
            {
                if (!EntityAliasRegex().IsMatch(le.EntityAlias))
                {
                    var errorMsg =
                        $"Invalid character specified for alias: {le.EntityAlias}. Only characters within the ranges [A-Z], [a-z] or [0-9] or _ are allowed.  The first character may only be in the ranges [A-Z], [a-z] or _.";
                    throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)ErrorCodes.QueryBuilderInvalidAlias, Message = errorMsg }, errorMsg);
                }
            }

            var leAlias = string.IsNullOrWhiteSpace(le.EntityAlias) ? le.LinkToEntityName : le.EntityAlias;
            state.ThrowIfNotKnownEntityType(le.LinkFromEntityName != linkFromAlias ? le.LinkFromEntityName : linkFromEntity);
            state.ThrowIfNotKnownEntityType(le.LinkToEntityName);

            if (!state.IsKnownAttributeForType(le.LinkToEntityName, le.LinkToAttributeName, out _))
            {
                var errorMsg = $"The attribute {le.LinkToAttributeName} does not exist on this entity.";
                throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)ErrorCodes.QueryBuilderNoAttribute, Message = errorMsg }, errorMsg);
            }

            IQueryable<Entity> inner;
            if (le.JoinOperator is JoinOperator.LeftOuter or JoinOperator.Any or JoinOperator.Exists or JoinOperator.In or JoinOperator.NotAny)
            {
                //filters are applied in the inner query and then ignored during filter evaluation
                var innerQueryExpression = new QueryExpression
                {
                    EntityName = le.LinkToEntityName,
                    ColumnSet = new ColumnSet(true)
                };
                // LinkCriteria can be null (e.g. when parsed from FetchXml with no <filter> element)
                if (le.LinkCriteria != null)
                {
                    innerQueryExpression.Criteria = le.LinkCriteria;
                }

                inner = queryProcessor.ExecuteQueryExpression(innerQueryExpression);
            }
            else
            {
                //Filters are applied after joins
                inner = state.CreateQuery<Entity>(le.LinkToEntityName);
            }

            if (string.IsNullOrWhiteSpace(linkFromAlias))
            {
                linkFromAlias = le.LinkFromAttributeName;
            }
            else
            {
                linkFromAlias += "." + le.LinkFromAttributeName;
            }

            query = le.JoinOperator switch
            {
                JoinOperator.Inner or JoinOperator.Natural => query.Join<Entity, Entity, object, Entity>(inner, outerKey => outerKey.KeySelector(linkFromAlias),
                    innerKey => innerKey.KeySelector(le.LinkToAttributeName), (outerEl, innerEl) => outerEl.CloneEntity().JoinAttributes(innerEl, new ColumnSet(true), leAlias)),
                JoinOperator.LeftOuter => query.GroupJoin(inner,
                        outerKey => outerKey.KeySelector(linkFromAlias), innerKey => innerKey.KeySelector(le.LinkToAttributeName), (outerEl, innerElemsCol) => new { outerEl, innerElemsCol })
                    .SelectMany(x => x.innerElemsCol.DefaultIfEmpty(), (x, y) => x.outerEl.CloneEntity().JoinAttributes(y, new ColumnSet(true), leAlias)),
                JoinOperator.Any or JoinOperator.Exists or JoinOperator.In =>
                    ExistsFilter(query, inner, linkFromAlias, le.LinkToAttributeName, negate: false),
                JoinOperator.NotAny =>
                    ExistsFilter(query, inner, linkFromAlias, le.LinkToAttributeName, negate: true),
                _ => ThrowUnsupportedJoinOperator(le.JoinOperator)
            };

            static IQueryable<Entity> ThrowUnsupportedJoinOperator(JoinOperator op)
            {
                ErrorFactory.ThrowFault(ErrorCodes.InvalidOperatorCode, $"The join operator '{op}' is not supported");
                return null!; // unreachable
            }

            // Process nested linked entities recursively
            foreach (var nestedLinkedEntity in le.LinkEntities)
            {
                if (string.IsNullOrWhiteSpace(le.EntityAlias))
                {
                    le.EntityAlias = le.LinkToEntityName;
                }

                if (string.IsNullOrWhiteSpace(nestedLinkedEntity.EntityAlias))
                {
                    nestedLinkedEntity.EntityAlias = EnsureUniqueLinkedEntityAlias(nestedLinkedEntity.LinkToEntityName);
                }

                query = TranslateLinkedEntityToLinq(nestedLinkedEntity, query,  le.EntityAlias, le.LinkToEntityName);
            }

            return query;
        }

        [GeneratedRegex(@"^[A-Za-z_](\w|\.)*$", RegexOptions.ECMAScript)]
        private static partial Regex EntityAliasRegex();

        private static IQueryable<Entity> ExistsFilter(
            IQueryable<Entity> query,
            IQueryable<Entity> inner,
            string linkFromAlias,
            string linkToAttributeName,
            bool negate)
        {
            // Materialize the inner sequence once so we don't re-enumerate it per outer row.
            var innerList = inner.ToList();
            // AsEnumerable is required because statement-body lambdas cannot be converted to
            // expression trees (IQueryable requires expression trees, IEnumerable does not).
            return query
                .AsEnumerable()
                .Where(outer =>
                {
                    var outerKey = outer.KeySelector(linkFromAlias);
                    var hasMatch = innerList.Any(innerEl => Equals(outerKey, innerEl.KeySelector(linkToAttributeName)));
                    return negate ? !hasMatch : hasMatch;
                })
                .AsQueryable();
        }
    }
