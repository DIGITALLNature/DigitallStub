// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Digitall.Stub.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries;

public class ExpressionProcessor(DataverseStub state)
{
    public IQueryable<Entity> FilterQuery(QueryExpression queryExpression, IQueryable<Entity> query)
    {
        Debug.Assert(queryExpression != null, nameof(queryExpression) + " != null");
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
            : (ConditionOperator[])Enum.GetValues(typeof(ConditionOperator));

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
            foreach (var e in linkedEntitiesQueryExpressions)
            {
                andExpression = Expression.And(e, andExpression);
            }

            var feExpression = TranslateFilterExpressionToExpression(queryExpression, queryExpression.EntityName, queryExpression.Criteria, entity, false);
            return Expression.And(andExpression, feExpression);
        }

        if (linkedEntitiesQueryExpressions.Count > 0)
        {
            //Linked entity expressions only
            Expression andExpression = Expression.Constant(true);
            foreach (var e in linkedEntitiesQueryExpressions)
            {
                andExpression = Expression.And(e, andExpression);
            }

            return andExpression;
        }

        //Criteria only
        return TranslateFilterExpressionToExpression(queryExpression, queryExpression.EntityName, queryExpression.Criteria, entity, false);
    }

    private Expression TranslateFilterExpressionToExpression(QueryExpression queryExpression, string sEntityName, FilterExpression fe, ParameterExpression entity, bool bIsOuter)
    {
        if (fe == null)
        {
            return Expression.Constant(true);
        }

        BinaryExpression conditionsLambda = null;
        BinaryExpression filtersLambda = null;
        if (fe.Conditions != null && fe.Conditions.Count > 0)
        {
            conditionsLambda = TranslateMultipleConditionExpressions(queryExpression, sEntityName, fe.Conditions.ToList(), fe.FilterOperator, entity, bIsOuter);
        }

        //Process nested filters recursively
        if (fe.Filters != null && fe.Filters.Count > 0)
        {
            filtersLambda = TranslateMultipleFilterExpressions(queryExpression, sEntityName, fe.Filters.ToList(), fe.FilterOperator, entity, bIsOuter);
        }

        if (conditionsLambda != null && filtersLambda != null)
        {
            //Satisfy both
            if (fe.FilterOperator == LogicalOperator.And)
            {
                return Expression.And(conditionsLambda, filtersLambda);
            }

            return Expression.Or(conditionsLambda, filtersLambda);
        }

        if (conditionsLambda != null)
        {
            return conditionsLambda;
        }

        if (filtersLambda != null)
        {
            return filtersLambda;
        }

        return Expression.Constant(true); //Satisfy filter if there are no conditions nor filters
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

        // Check if there are link criteria for the linked entity
        if (linkedEntity.LinkCriteria != null)
        {
            // Get the attribute metadata for the linked entity
            var attributeMetadata = state.EntityMetadata.TryGetValue(linkedEntity.LinkToEntityName, out var value) ? value.Attributes : null;

            // Process each condition in the link criteria
            foreach (var ce in linkedEntity.LinkCriteria.Conditions)
            {
                // Check if the attribute is not known for the type and ends with "name"
                if (!state.IsKnownAttributeForType(linkedEntity.LinkToEntityName, ce.AttributeName, out _) && ce.AttributeName.EndsWith("name", StringComparison.Ordinal))
                {
                    // Special case for referencing the name of an EntityReference
                    var slicedAttributeName = ce.AttributeName.Substring(0, ce.AttributeName.Length - 4);
                    if (state.IsKnownAttributeForType(linkedEntity.LinkToEntityName, slicedAttributeName, out var attributeInfo) && attributeInfo.PropertyType == typeof(EntityReference))
                    {
                        // Update the attribute name to avoid conflicts with the naming pattern
                        ce.AttributeName = slicedAttributeName;
                    }
                }
                else if (attributeMetadata != null && attributeMetadata.All(a => a.LogicalName != ce.AttributeName) && ce.AttributeName.EndsWith("name", StringComparison.Ordinal))
                {
                    // Special case for referencing the name of an EntityReference
                    var slicedAttributeName = ce.AttributeName.Substring(0, ce.AttributeName.Length - 4);
                    if (attributeMetadata.Any(a => a.LogicalName == slicedAttributeName))
                    {
                        ce.AttributeName = slicedAttributeName;
                    }
                }

                // Update the attribute name with the entity alias
                var entityAlias = !string.IsNullOrEmpty(linkedEntity.EntityAlias) ? linkedEntity.EntityAlias : linkedEntity.LinkToEntityName;
                ce.AttributeName = entityAlias + "." + ce.AttributeName;
            }

            // Process each filter condition in the link criteria
            foreach (var fe in linkedEntity.LinkCriteria.Filters)
            {
                foreach (var ce in fe.Conditions)
                {
                    // Update the attribute name with the entity alias
                    var entityAlias = !string.IsNullOrEmpty(linkedEntity.EntityAlias) ? linkedEntity.EntityAlias : linkedEntity.LinkToEntityName;
                    ce.AttributeName = entityAlias + "." + ce.AttributeName;
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
        BinaryExpression binaryExpression = null; //Default initialisation depending on logical operator
        if (logicalOperator == LogicalOperator.And)
        {
            binaryExpression = Expression.And(Expression.Constant(true), Expression.Constant(true));
        }
        else
        {
            binaryExpression = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        }

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
            if (state.ModelAssemblies.Count != 0)
            {
                if (c.EntityName != null)
                {
                    cEntityName = queryExpression.GetEntityNameFromAlias(c.EntityName);
                }
                else
                {
                    if (c.AttributeName.IndexOf(".", StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        var alias = c.AttributeName.Split('.')[0];
                        cEntityName = queryExpression.GetEntityNameFromAlias(alias);
                        sAttributeName = c.AttributeName.Split('.')[1];
                    }
                }


                if (state.EntityTypeIsKnow(cEntityName, out var earlyBoundType))
                {
                    if (!state.IsKnownAttributeForType(cEntityName, sAttributeName, out var _))
                    {
                        // Special case when filtering on the name of a Lookup
                        if (sAttributeName.EndsWith("name", StringComparison.Ordinal))
                        {
                            var realAttributeName = c.AttributeName.Substring(0, c.AttributeName.Length - 4);

                            if (state.IsKnownAttributeForType(cEntityName, realAttributeName, out var attributeInfo))
                            {
                                if (attributeInfo.PropertyType == typeof(EntityReference))
                                {
                                    // Need to make Lookups work against the real attribute, not the "name" suffixed attribute that doesn't exist
                                    c.AttributeName = realAttributeName;
                                }
                            }
                        }
                    }
                }
            }

            EnsureSupportedTypedExpression(typedExpression);

            //Build a binary expression
            if (logicalOperator == LogicalOperator.And)
            {
                binaryExpression = Expression.And(binaryExpression, ConditionParser.TranslateConditionExpression(queryExpression, state, typedExpression, entity));
            }
            else
            {
                binaryExpression = Expression.Or(binaryExpression, ConditionParser.TranslateConditionExpression(queryExpression, state, typedExpression, entity));
            }
        }

        return binaryExpression;
    }


    private BinaryExpression TranslateMultipleFilterExpressions(QueryExpression queryExpression, string sEntityName, List<FilterExpression> filters, LogicalOperator logicalOperator,
        ParameterExpression entity, bool bIsOuter)
    {
        BinaryExpression binaryExpression = null;
        if (logicalOperator == LogicalOperator.And)
        {
            binaryExpression = Expression.And(Expression.Constant(true), Expression.Constant(true));
        }
        else
        {
            binaryExpression = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        }

        foreach (var f in filters)
        {
            var thisFilterLambda = TranslateFilterExpressionToExpression(queryExpression, sEntityName, f, entity, bIsOuter);

            //Build a binary expression
            if (logicalOperator == LogicalOperator.And)
            {
                binaryExpression = Expression.And(binaryExpression, thisFilterLambda);
            }
            else
            {
                binaryExpression = Expression.Or(binaryExpression, thisFilterLambda);
            }
        }

        return binaryExpression;
    }
}
