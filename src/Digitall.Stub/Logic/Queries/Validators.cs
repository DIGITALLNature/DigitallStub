// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries
{
    public static class Validators
    {
        /// <summary>
        ///     Recursively validates the aliases in the linked entities of a query expression.
        /// </summary>
        /// <param name="queryExpression">The query expression to validate.</param>
        /// <param name="linkEntity">The link entity to validate.</param>
        public static void ValidateLinkedAliases(QueryExpression queryExpression, LinkEntity linkEntity)
        {
            // Validate the criteria of the link entity
            if (linkEntity.LinkCriteria != null)
            {
                ValidateFilterExpressionAliases(queryExpression, linkEntity.LinkCriteria);
            }

            // Recursively validate the linked entities in the link entity
            if (linkEntity.LinkEntities != null)
            {
                foreach (var innerLink in linkEntity.LinkEntities)
                {
                    ValidateLinkedAliases(queryExpression, innerLink);
                }
            }
        }

        /// <summary>
        ///     Recursively validates the aliases in the filter expression.
        /// </summary>
        /// <param name="queryExpression">The query expression to validate.</param>
        /// <param name="filterExpression">The filter expression to validate.</param>
        public static void ValidateFilterExpressionAliases(QueryExpression queryExpression, FilterExpression filterExpression)
        {
            // Recursively validate the filters in the filter expression
            if (filterExpression.Filters != null)
            {
                foreach (var innerFilter in filterExpression.Filters)
                {
                    ValidateFilterExpressionAliases(queryExpression, innerFilter);
                }
            }

            // Validate the conditions in the filter expression
            if (filterExpression.Conditions != null)
            {
                foreach (var condition in filterExpression.Conditions)
                {
                    // Validate the condition only if it has an entity name
                    if (!string.IsNullOrEmpty(condition.EntityName))
                    {
                        ValidateConditionAliases(queryExpression, condition);
                    }
                }
            }
        }

        /// <summary>
        ///     Validates the aliases of a condition expression in a query expression.
        /// </summary>
        /// <param name="queryExpression">The query expression to validate.</param>
        /// <param name="conditionExpression">The condition expression to validate.</param>
        private static void ValidateConditionAliases(QueryExpression queryExpression, ConditionExpression conditionExpression)
        {
            // Check if the condition has a matching alias in the query expression's link entities
            var matches = queryExpression.LinkEntities != null ? MatchByAlias(conditionExpression, queryExpression.LinkEntities) : 0;

            // If there are multiple matches, throw an exception
            if (matches > 1)
            {
                throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault(),
                    $"Table {conditionExpression.EntityName} is not unique amongst all top-level table and join aliases");
            }

            // If there are no matches, check if there is a matching entity in the query expression's link entities
            if (matches == 0)
            {
                if (queryExpression.LinkEntities != null)
                {
                    matches = MatchByEntity(conditionExpression, queryExpression.LinkEntities);
                }

                // If there are multiple matches, throw an exception
                if (matches > 1)
                {
                    throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault(), $"There's more than one LinkEntity expressions with name={conditionExpression.EntityName}");
                }

                // If there are no matches, check if the condition's entity name matches the query expression's entity name
                if (matches == 0)
                {
                    if (conditionExpression.EntityName == queryExpression.EntityName)
                    {
                        return;
                    }

                    throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault(), $"LinkEntity with name or alias {conditionExpression.EntityName} is not found");
                }

                // If there is a match, append "1" to the condition's entity name
                conditionExpression.EntityName += "1";
            }
        }

        /// <summary>
        ///     Recursively counts the number of link entities in the given collection that match the given condition by entity name.
        /// </summary>
        /// <param name="condition">The condition to match.</param>
        /// <param name="linkEntities">The collection of link entities to search.</param>
        /// <returns>The number of link entities that match the given condition.</returns>
        private static int MatchByEntity(ConditionExpression condition, DataCollection<LinkEntity> linkEntities)
        {
            var matches = 0;

            // Iterate over each link entity in the collection
            foreach (var link in linkEntities)
            {
                // Check if the link entity has no alias and its LinkToEntityName matches the condition's EntityName
                if (string.IsNullOrEmpty(link.EntityAlias) && condition.EntityName == link.LinkToEntityName)
                {
                    // Increment the match count
                    matches += 1;
                }

                // If the link entity has nested link entities, recursively count the matches in those nested entities
                if (link.LinkEntities != null)
                {
                    matches += MatchByEntity(condition, link.LinkEntities);
                }
            }

            // Return the total number of matches
            return matches;
        }

        /// <summary>
        ///     Recursively counts the number of link entities in the given collection that match the given condition by entity alias.
        /// </summary>
        /// <param name="condition">The condition to match.</param>
        /// <param name="linkEntities">The collection of link entities to search.</param>
        /// <returns>The number of link entities that match the given condition.</returns>
        private static int MatchByAlias(ConditionExpression condition, DataCollection<LinkEntity> linkEntities)
        {
            var matches = 0;

            // Iterate over each link entity in the collection
            foreach (var link in linkEntities)
            {
                // Check if the link entity's alias matches the condition's entity name
                if (link.EntityAlias == condition.EntityName)
                {
                    // Increment the match count
                    matches += 1;
                }

                // If the link entity has nested link entities, recursively count the matches in those nested entities
                if (link.LinkEntities != null)
                {
                    matches += MatchByAlias(condition, link.LinkEntities);
                }
            }

            // Return the total number of matches
            return matches;
        }
    }
}
