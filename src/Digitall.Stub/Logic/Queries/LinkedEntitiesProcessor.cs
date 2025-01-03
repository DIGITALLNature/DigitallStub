// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using Digitall.Stub.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries
{
    public class LinkedEntitiesProcessor
    {
        private readonly DataverseStub _state;
        private readonly QueryProcessor _queryProcessor;
        readonly Dictionary<string, int> _linkedEntities = new Dictionary<string, int>();

        public LinkedEntitiesProcessor(DataverseStub state, QueryProcessor queryProcessor)
        {
            _state = state;
            _queryProcessor = queryProcessor;
        }

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
                if (!Regex.IsMatch(le.EntityAlias, "^[A-Za-z_](\\w|\\.)*$", RegexOptions.ECMAScript))
                {
                    var errorMsg =
                        $"Invalid character specified for alias: {le.EntityAlias}. Only characters within the ranges [A-Z], [a-z] or [0-9] or _ are allowed.  The first character may only be in the ranges [A-Z], [a-z] or _.";
                    throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)ErrorCodes.QueryBuilderInvalid_Alias, Message = errorMsg }, errorMsg);
                }
            }

            var leAlias = string.IsNullOrWhiteSpace(le.EntityAlias) ? le.LinkToEntityName : le.EntityAlias;
            _state.ThrowIfNotKnownEntityType(le.LinkFromEntityName != linkFromAlias ? le.LinkFromEntityName : linkFromEntity);
            _state.ThrowIfNotKnownEntityType(le.LinkToEntityName);

            if (!_state.IsKnownAttributeForType(le.LinkToEntityName, le.LinkToAttributeName, out _))
            {
                var errorMsg = $"The attribute {le.LinkToAttributeName} does not exist on this entity.";
                throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)ErrorCodes.QueryBuilderNoAttribute, Message = errorMsg }, errorMsg);
            }

            IQueryable<Entity> inner = null;
            if (le.JoinOperator == JoinOperator.LeftOuter)
            {
                //filters are applied in the inner query and then ignored during filter evaluation
                var outerQueryExpression = new QueryExpression
                {
                    EntityName = le.LinkToEntityName,
                    Criteria = le.LinkCriteria,
                    ColumnSet = new ColumnSet(true)
                };

                var outerQuery = _queryProcessor.ExecuteQueryExpression(outerQueryExpression);
                inner = outerQuery;
            }
            else
            {
                //Filters are applied after joins
                inner = _state.CreateQuery<Entity>(le.LinkToEntityName);
            }

            if (string.IsNullOrWhiteSpace(linkFromAlias))
            {
                linkFromAlias = le.LinkFromAttributeName;
            }
            else
            {
                linkFromAlias += "." + le.LinkFromAttributeName;
            }

            switch (le.JoinOperator)
            {
                case JoinOperator.Inner:
                case JoinOperator.Natural:
                    query = query.Join(inner,
                        outerKey => outerKey.KeySelector(linkFromAlias),
                        innerKey => innerKey.KeySelector(le.LinkToAttributeName),
                        (outerEl, innerEl) => outerEl.CloneEntity().JoinAttributes(innerEl, new ColumnSet(true), leAlias));

                    break;
                case JoinOperator.LeftOuter:
                    query = query.GroupJoin(inner,
                            outerKey => outerKey.KeySelector(linkFromAlias),
                            innerKey => innerKey.KeySelector(le.LinkToAttributeName),
                            (outerEl, innerElemsCol) => new { outerEl, innerElemsCol })
                        .SelectMany(x => x.innerElemsCol.DefaultIfEmpty()
                            , (x, y) => x.outerEl
                                .JoinAttributes(y, new ColumnSet(true), leAlias));


                    break;
                default: //This shouldn't be reached as there are only 3 types of Join...
                    throw new ArgumentException($"The join operator {le.JoinOperator} is currently not supported.");
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

    }
}
