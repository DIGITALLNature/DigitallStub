// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Linq.Expressions;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Logic.Queries;

public class ExpressionProcessor(FakeOrganizationService fakeOrgService)
{
    public IQueryable<Entity> FilterQuery(QueryExpression queryExpression, IQueryable<Entity> query)
    {
        Validators.ValidateFilterExpressionAliases(queryExpression, queryExpression.Criteria);
        return query.Where(Generate(queryExpression));
    }

    public Expression<Func<Entity, bool>> Generate(QueryExpression queryExpression)
    {
        // Compose the expression tree that represents the parameter to the predicate.
        var entity = Expression.Parameter(typeof(Entity));
        var expTreeBody = ParseToExpression(queryExpression, entity);
        return Expression.Lambda<Func<Entity, bool>>(expTreeBody, entity);
    }

    private static void EnsureSupportedTypedExpression(TypedConditionExpression typedExpression)
    {
        var supportedOperators = typedExpression.AttributeType == typeof(OptionSetValueCollection)
            ?
            [
                ConditionOperator.ContainValues,
                ConditionOperator.DoesNotContainValues,
                ConditionOperator.Equal,
                ConditionOperator.NotEqual,
                ConditionOperator.NotNull,
                ConditionOperator.Null,
                ConditionOperator.In,
                ConditionOperator.NotIn
            ]
            : Enum.GetValues<ConditionOperator>();

        if (!supportedOperators.Contains(typedExpression.CondExpression.Operator))
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidOperatorCode, "The operator is not valid or it is not supported.");
        }
    }

    private Expression ParseToExpression(QueryExpression queryExpression, ParameterExpression entity)
    {
        var linkedEntitiesQueryExpressions = new List<Expression>();
        foreach (var le in queryExpression.LinkEntities)
        {
            var listOfExpressions = TranslateLinkedEntityFilterExpressionToExpression(queryExpression, le, entity);
            linkedEntitiesQueryExpressions.AddRange(listOfExpressions);
        }

        if (linkedEntitiesQueryExpressions.Count > 0 && queryExpression.Criteria != null)
        {
            //Return the and of the two
            Expression andExpression = Expression.Constant(true);
            andExpression = linkedEntitiesQueryExpressions.Aggregate(andExpression, (current, e) => Expression.And(e, current));

            var feExpression = TranslateFilterExpressionToExpression(queryExpression, queryExpression.EntityName, queryExpression.Criteria, entity, false);
            return Expression.And(andExpression, feExpression);
        }

        if (linkedEntitiesQueryExpressions.Count > 0)
        {
            //Linked entity expressions only
            Expression andExpression = Expression.Constant(true);

            return linkedEntitiesQueryExpressions.Aggregate(andExpression, (current, e) => Expression.And(e, current));
        }

        //Criteria only
        return TranslateFilterExpressionToExpression(queryExpression, queryExpression.EntityName, queryExpression.Criteria, entity, false);
    }

    private Expression TranslateFilterExpressionToExpression(QueryExpression queryExpression, string sEntityName, FilterExpression? fe, ParameterExpression entity, bool bIsOuter)
    {
        if (fe == null)
        {
            return Expression.Constant(true);
        }

        BinaryExpression? conditionsLambda = null;
        BinaryExpression? filtersLambda = null;
        Expression? anyAllLambda = null;

        if (fe.Conditions is { Count: > 0 })
        {
            conditionsLambda = TranslateMultipleConditionExpressions(queryExpression, sEntityName, fe.Conditions.ToList(), fe.FilterOperator, entity, bIsOuter);
        }

        //Process nested filters recursively
        if (fe.Filters is { Count: > 0 })
        {
            filtersLambda = TranslateMultipleFilterExpressions(queryExpression, sEntityName, fe.Filters.ToList(), fe.FilterOperator, entity, bIsOuter);
        }

        // Process AnyAllFilterLinkEntity (EXISTS/NOT EXISTS subquery)
        if (fe.AnyAllFilterLinkEntity != null)
        {
            anyAllLambda = TranslateAnyAllFilterLinkEntity(fe.AnyAllFilterLinkEntity, entity);
        }

        // Combine all parts using the FilterOperator
        var parts = new List<Expression>();
        if (conditionsLambda != null) parts.Add(conditionsLambda);
        if (filtersLambda != null) parts.Add(filtersLambda);
        if (anyAllLambda != null) parts.Add(anyAllLambda);

        if (parts.Count == 0)
        {
            return Expression.Constant(true);
        }

        return parts.Aggregate((left, right) =>
            fe.FilterOperator == LogicalOperator.And ? Expression.And(left, right) : Expression.Or(left, right));
    }

    /// <summary>
    ///     Translates a <see cref="FilterExpression.AnyAllFilterLinkEntity"/> into a boolean expression
    ///     that evaluates EXISTS/NOT EXISTS semantics against the in-memory state.
    /// </summary>
    private Expression TranslateAnyAllFilterLinkEntity(LinkEntity linkEntity, ParameterExpression entity)
    {
        // Build the inner query (with LinkCriteria applied)
        var filteredInnerQuery = new QueryExpression
        {
            EntityName = linkEntity.LinkToEntityName,
            ColumnSet = new ColumnSet(true)
        };
        if (linkEntity.LinkCriteria != null)
        {
            filteredInnerQuery.Criteria = linkEntity.LinkCriteria;
        }

        var queryProcessor = new QueryProcessor(fakeOrgService);
        var filteredInner = queryProcessor.ExecuteQueryExpression(filteredInnerQuery).ToList();

        var linkFromAttr = linkEntity.LinkFromAttributeName;
        var linkToAttr = linkEntity.LinkToAttributeName;

        Func<Entity, bool> predicate;

        switch (linkEntity.JoinOperator)
        {
            case JoinOperator.Any:
            case JoinOperator.NotAll:
            case JoinOperator.Exists:
            case JoinOperator.In:
                // EXISTS: parent has at least one matching linked record
                predicate = outer =>
                {
                    var outerKey = outer.KeySelector(linkFromAttr);
                    return filteredInner.Any(inner => Equals(outerKey, inner.KeySelector(linkToAttr)));
                };
                break;

            case JoinOperator.NotAny:
                // NOT EXISTS: parent has no matching linked records
                predicate = outer =>
                {
                    var outerKey = outer.KeySelector(linkFromAttr);
                    return !filteredInner.Any(inner => Equals(outerKey, inner.KeySelector(linkToAttr)));
                };
                break;

            case JoinOperator.All:
                // ALL: linked records exist (unfiltered) but none satisfy the criteria
                var unfilteredInner = fakeOrgService.CreateQuery<Entity>(linkEntity.LinkToEntityName).ToList();
                predicate = outer =>
                {
                    var outerKey = outer.KeySelector(linkFromAttr);
                    var hasAnyLinked = unfilteredInner.Any(inner => Equals(outerKey, inner.KeySelector(linkToAttr)));
                    if (!hasAnyLinked) return false;
                    var hasFilteredMatch = filteredInner.Any(inner => Equals(outerKey, inner.KeySelector(linkToAttr)));
                    return !hasFilteredMatch;
                };
                break;

            default:
                ErrorFactory.ThrowFault(
                    ErrorCodes.InvalidOperatorCode,
                    $"The join operator '{linkEntity.JoinOperator}' is not supported for AnyAllFilterLinkEntity");
                predicate = _ => false; // unreachable
                break;
        }

        // Wrap the predicate as an Expression.Invoke on a compiled delegate
        var predicateExpr = Expression.Constant(predicate);
        return Expression.Invoke(predicateExpr, entity);
    }


    /// <summary>
    ///     Translates the filter expressions of a linked entity to expressions.
    /// </summary>
    /// <param name="queryExpression">The query expression.</param>
    /// <param name="linkedEntity">The linked entity.</param>
    /// <param name="expression">The parameter expression.</param>
    /// <returns>A list of expressions related to the linked entities filter conditions.</returns>
    private List<Expression> TranslateLinkedEntityFilterExpressionToExpression(QueryExpression queryExpression, LinkEntity linkedEntity, ParameterExpression expression)
    {
        // Initialize list to store linked entity query expressions
        var linkedEntitiesQueryExpressions = new List<Expression>();

        // EXISTS-style operators apply LinkCriteria inside the subquery in LinkedEntitiesProcessor,
        // so we must not re-evaluate them here as a post-join filter.
        if (linkedEntity.JoinOperator is JoinOperator.Any or JoinOperator.NotAny or JoinOperator.Exists or JoinOperator.In or JoinOperator.All or JoinOperator.NotAll)
        {
            foreach (var nestedLinkedEntity in linkedEntity.LinkEntities)
            {
                linkedEntitiesQueryExpressions.AddRange(TranslateLinkedEntityFilterExpressionToExpression(queryExpression, nestedLinkedEntity, expression));
            }

            return linkedEntitiesQueryExpressions;
        }

        // Check if there are link criteria for the linked entity
        if (linkedEntity.LinkCriteria != null)
        {
            // Get the attribute metadata for the linked entity
            var attributeMetadata = fakeOrgService.State.EntityMetadata.TryGetValue(linkedEntity.LinkToEntityName, out var value) ? value.Attributes : null;
            var entityAlias = !string.IsNullOrEmpty(linkedEntity.EntityAlias) ? linkedEntity.EntityAlias : linkedEntity.LinkToEntityName;
            var aliasPrefix = entityAlias + ".";

            // Process each condition in the link criteria
            foreach (var ce in linkedEntity.LinkCriteria.Conditions)
            {
                // Strip any existing alias prefix to make this idempotent (safe for query reuse)
                var rawAttributeName = ce.AttributeName.StartsWith(aliasPrefix, StringComparison.Ordinal)
                    ? ce.AttributeName[aliasPrefix.Length..]
                    : ce.AttributeName;

                // Check if the attribute is not known for the type and ends with "name"
                if (!fakeOrgService.IsKnownAttributeForType(linkedEntity.LinkToEntityName, rawAttributeName, out _) && rawAttributeName.EndsWith("name", StringComparison.Ordinal))
                {
                    // Special case for referencing the name of an EntityReference
                    var slicedAttributeName = rawAttributeName[..^4];
                    if (fakeOrgService.IsKnownAttributeForType(linkedEntity.LinkToEntityName, slicedAttributeName, out var attributeInfo) && attributeInfo!.PropertyType == typeof(EntityReference))
                    {
                        rawAttributeName = slicedAttributeName;
                    }
                }
                else if (attributeMetadata != null && attributeMetadata.All(a => a.LogicalName != rawAttributeName) && rawAttributeName.EndsWith("name", StringComparison.Ordinal))
                {
                    // Special case for referencing the name of an EntityReference
                    var slicedAttributeName = rawAttributeName[..^4];
                    if (attributeMetadata.Any(a => a.LogicalName == slicedAttributeName))
                    {
                        rawAttributeName = slicedAttributeName;
                    }
                }

                // Prefix with entity alias
                ce.AttributeName = aliasPrefix + rawAttributeName;
            }

            // Process each filter condition in the link criteria
            foreach (var fe in linkedEntity.LinkCriteria.Filters)
            {
                foreach (var ce in fe.Conditions)
                {
                    // Strip any existing alias prefix (idempotent for reuse)
                    var rawName = ce.AttributeName.StartsWith(aliasPrefix, StringComparison.Ordinal)
                        ? ce.AttributeName[aliasPrefix.Length..]
                        : ce.AttributeName;
                    ce.AttributeName = aliasPrefix + rawName;
                }
            }
        }

        // Translate the specific link criteria to an expression
        linkedEntitiesQueryExpressions.Add(TranslateFilterExpressionToExpression(queryExpression, linkedEntity.LinkToEntityName, linkedEntity.LinkCriteria, expression,
            linkedEntity.JoinOperator == JoinOperator.LeftOuter));

        // Process nested linked entities
        foreach (var nestedLinkedEntity in linkedEntity.LinkEntities)
        {
            var nestedExpressions = TranslateLinkedEntityFilterExpressionToExpression(queryExpression, nestedLinkedEntity, expression);
            linkedEntitiesQueryExpressions.AddRange(nestedExpressions);
        }

        // Return the list of expressions related to the linked entities filter conditions
        return linkedEntitiesQueryExpressions;
    }

    private BinaryExpression TranslateMultipleConditionExpressions(QueryExpression queryExpression, string sEntityName, List<ConditionExpression> conditions, LogicalOperator logicalOperator,
        ParameterExpression entity, bool bIsOuter)
    {
        var binaryExpression = //Default initialisation depending on logical operator
            logicalOperator == LogicalOperator.And ? Expression.And(Expression.Constant(true), Expression.Constant(true)) : Expression.Or(Expression.Constant(false), Expression.Constant(false));

        foreach (var c in conditions)
        {
            var cEntityName = sEntityName;
            //Create a new typed expression
            var typedExpression = new TypedConditionExpression(c)
            {
                IsOuter = bIsOuter
            };

            var sAttributeName = c.AttributeName;

            //Find the attribute type if using early bound entities
            if (fakeOrgService.State.ModelAssemblies.Count != 0)
            {
                if (c.EntityName != null)
                {
                    cEntityName = queryExpression.GetEntityNameFromAlias(c.EntityName);
                }
                else
                {
                    if (c.AttributeName.Contains('.', StringComparison.CurrentCultureIgnoreCase))
                    {
                        var alias = c.AttributeName.Split('.')[0];
                        cEntityName = queryExpression.GetEntityNameFromAlias(alias);
                        sAttributeName = c.AttributeName.Split('.')[1];
                    }
                }



                if (fakeOrgService.IsKnownAttributeForType(cEntityName, sAttributeName, out var propertyInfo))
                {
                    typedExpression.AttributeType = propertyInfo!.PropertyType;

                    // Special case when filtering on the name of a Lookup
                    if (typedExpression.AttributeType == typeof(EntityReference) &&  sAttributeName.EndsWith("name", StringComparison.Ordinal))
                    {
                        var realAttributeName = c.AttributeName[..^4];

                        if (fakeOrgService.IsKnownAttributeForType(cEntityName, realAttributeName, out var attributeInfo))
                        {
                            if (attributeInfo!.PropertyType == typeof(EntityReference))
                            {
                                // Create a new typed expression with the corrected attribute name to avoid mutating the input
                                var adjustedCondition = new ConditionExpression(realAttributeName, c.Operator, c.Values.ToArray());
                                typedExpression = new TypedConditionExpression(adjustedCondition)
                                {
                                    AttributeType = typedExpression.AttributeType,
                                    IsOuter = typedExpression.IsOuter
                                };
                            }
                        }
                    }
                }
            }

            EnsureSupportedTypedExpression(typedExpression);

            //Build a binary expression
            binaryExpression = logicalOperator == LogicalOperator.And ? Expression.And(binaryExpression, ConditionParser.TranslateConditionExpression(queryExpression, fakeOrgService, typedExpression, entity)) : Expression.Or(binaryExpression, ConditionParser.TranslateConditionExpression(queryExpression, fakeOrgService, typedExpression, entity));
        }

        return binaryExpression;
    }


    private BinaryExpression TranslateMultipleFilterExpressions(QueryExpression queryExpression, string sEntityName, List<FilterExpression> filters, LogicalOperator logicalOperator,
        ParameterExpression entity, bool bIsOuter)
    {
        var binaryExpression = logicalOperator == LogicalOperator.And ? Expression.And(Expression.Constant(true), Expression.Constant(true)) : Expression.Or(Expression.Constant(false), Expression.Constant(false));

        foreach (var f in filters)
        {
            var thisFilterLambda = TranslateFilterExpressionToExpression(queryExpression, sEntityName, f, entity, bIsOuter);

            //Build a binary expression
            binaryExpression = logicalOperator == LogicalOperator.And ? Expression.And(binaryExpression, thisFilterLambda) : Expression.Or(binaryExpression, thisFilterLambda);
        }

        return binaryExpression;
    }
}
