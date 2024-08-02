// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Digitall.Stub.Logic.Queries;
using DotNetEnv;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.OrganizationRequests;

public class RetrieveMultipleStub : OrganizationRequestStub<RetrieveMultipleRequest, RetrieveMultipleResponse>
    {
        public override RetrieveMultipleResponse Execute(RetrieveMultipleRequest organizationRequest, DataverseStub state)
        {
            Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

            // Create a new QueryProcessor with the given state
            var queryProcessor = new QueryProcessor(state);

            // Initialize variables
            QueryExpression queryExpression = null;
            PagingInfo pageInfo = null;
            string entityName = null;
            List<Entity> internalResult = null;

            // Check if the query is a QueryExpression
            if (organizationRequest.Query is QueryExpression expression)
            {
                // Clone the QueryExpression
                queryExpression = expression.CloneQuery();
                entityName = queryExpression.EntityName;

                // Execute the query and store the results in a list
                var linqQuery = queryProcessor.ExecuteQueryExpression(queryExpression);
                internalResult = linqQuery.ToList();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(organizationRequest), organizationRequest.Query.GetType().Name, "Query type is unknown");
            }


            // If the query has distinct set to true, remove duplicates from the list
            if (queryExpression.Distinct)
            {
                internalResult = GetDistinctEntities(internalResult);
            }

            // If the query has a TopCount set, limit the number of results returned
            if (queryExpression.TopCount != null && queryExpression.TopCount.Value < internalResult.Count)
            {
                internalResult = internalResult.Take(queryExpression.TopCount.Value).ToList();
            }

            // Handle TotalRecordCount here
            int totalRecordCount = -1;
            if (queryExpression?.PageInfo?.ReturnTotalRecordCount == true)
            {
                totalRecordCount = internalResult.Count;
            }

            // Handle paging
            var maxRetireveCount = Env.GetInt("MaxRetrieveCount", 5000);
            var pageSize = maxRetireveCount;
            pageInfo = queryExpression.PageInfo;
            int pageNumber = 1;

            // Calculate the start position and number of items to retrieve based on the page number and page size
            if (pageInfo != null && pageInfo.PageNumber > 0)
            {
                pageNumber = pageInfo.PageNumber;
                pageSize = pageInfo.Count == 0 ? maxRetireveCount : pageInfo.Count;
            }

            // Figure out where in the list we need to start and how many items we need to grab
            int numberToGet = pageSize;
            int startPosition = 0;

            if (pageNumber != 1)
            {
                startPosition = (pageNumber - 1) * pageSize;
            }

            if (internalResult.Count < pageSize)
            {
                numberToGet = internalResult.Count;
            }
            else if (internalResult.Count - pageSize * (pageNumber - 1) < pageSize)
            {
                numberToGet = internalResult.Count - (pageSize * (pageNumber - 1));
            }

            var recordsToReturn = startPosition + numberToGet > internalResult.Count ? new List<Entity>() : internalResult.GetRange(startPosition, numberToGet);

             recordsToReturn.ForEach(e => PatchDateFormat(e,state));
             recordsToReturn.ForEach(e => PopulateFormattedValues(e));

            var response = new RetrieveMultipleResponse
            {
                Results = new ParameterCollection
                                 {
                                    { "EntityCollection", new EntityCollection(recordsToReturn) }
                                 }
            };
            response.EntityCollection.EntityName = entityName;
            response.EntityCollection.MoreRecords = (internalResult.Count - pageSize * pageNumber) > 0;
            response.EntityCollection.TotalRecordCount = totalRecordCount;

            if (response.EntityCollection.MoreRecords)
            {
                var first = response.EntityCollection.Entities.First();
                var last = response.EntityCollection.Entities.Last();
                response.EntityCollection.PagingCookie = $"<cookie page=\"{pageNumber}\">" +
                                                         $"<{first.LogicalName}id last=\"{last.Id:B.ToUpper()}\" first=\"{first.Id:B.ToUpper()}\" />" +
                                                         $"</cookie>";
            }

            return response;
        }

        /// <summary>
        /// Returns a new list containing only the distinct entities from the input list.
        /// Two entities are considered distinct if they have the same logical name and all their attributes are equal.
        /// </summary>
        /// <param name="entities">The list of entities to filter.</param>
        /// <returns>A new list containing only the distinct entities from the input list.</returns>
        private static List<Entity> GetDistinctEntities(List<Entity> entities)
        {
            var output = new List<Entity>();

            foreach (var entity in entities.Where(entity => !output.Exists(i => i.LogicalName == entity.LogicalName && i.Attributes.SequenceEqual(entity.Attributes))))
            {
                output.Add(entity);
            }

            return output;
        }

        internal static void PatchDateFormat(Entity record, DataverseStub stub)
        {
            if (stub.EntityMetadata.TryGetValue(record.LogicalName, out var entityMetadata))
            {
                foreach (var dateTimeAttribute in record.Attributes.Where(a => a.Value is DateTime))
                {
                    if (entityMetadata.Attributes.SingleOrDefault(a => a.LogicalName == dateTimeAttribute.Key) is DateTimeAttributeMetadata { Format: DateTimeFormat.DateOnly })
                    {
                        var currentValue = (DateTime)record[dateTimeAttribute.Key];
                        record[dateTimeAttribute.Key] = new DateTime(currentValue.Year, currentValue.Month, currentValue.Day, 0, 0, 0, DateTimeKind.Utc);
                    }
                }
            }
        }

        internal void PopulateFormattedValues(Entity e)
        {
            // Iterate through attributes and retrieve formatted values based on type
            foreach (var attKey in e.Attributes.Keys)
            {
                var value = e[attKey];
                string formattedValue = "";
                if (!e.FormattedValues.ContainsKey(attKey) && (value != null))
                {
                    bool bShouldAdd;
                    formattedValue = this.GetFormattedValueForValue(value, out bShouldAdd);
                    if (bShouldAdd)
                    {
                        e.FormattedValues.Add(attKey, formattedValue);
                    }
                }
            }
        }

        protected string GetFormattedValueForValue(object value, out bool bShouldAddFormattedValue)
        {
            bShouldAddFormattedValue = false;
            var sFormattedValue = string.Empty;

            if (value is Enum)
            {
                // Retrieve the enum type
                sFormattedValue = Enum.GetName(value.GetType(), value);
                bShouldAddFormattedValue = true;
            }
            else if (value is AliasedValue)
            {
                return this.GetFormattedValueForValue((value as AliasedValue)?.Value, out bShouldAddFormattedValue);
            }

            return sFormattedValue;
        }

    }
