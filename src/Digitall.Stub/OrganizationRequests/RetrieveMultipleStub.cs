// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using System.Linq;
using Digitall.Stub.Logic.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.OrganizationRequests;

public class RetrieveMultipleStub : OrganizationRequestStub<RetrieveMultipleRequest, RetrieveMultipleResponse>
    {
        public override RetrieveMultipleResponse Execute(RetrieveMultipleRequest organizationRequest, DataverseStub state)
        {
QueryExpression qe;
            var queryProcessor = new QueryProcessor(state);

            string entityName = null;
            List<Entity> list = null;

            if (organizationRequest.Query is QueryExpression expression)
            {
                qe = expression.CloneQuery();
                entityName = qe.EntityName;

                var linqQuery = queryProcessor.ExecuteQueryExpression(qe);
                list = linqQuery.ToList();
            }
            // else if (organizationRequest.Query is FetchExpression)
            // {
            //     var fetchXml = (organizationRequest.Query as FetchExpression).Query;
            //     var xmlDoc = queryProcessor.ParseFetchXml(fetchXml);
            //     qe = queryProcessor.TranslateFetchXmlDocumentToQueryExpression(ctx, xmlDoc);
            //     entityName = qe.EntityName;
            //
            //     var linqQuery = QueryProcessor.ExecuteQueryExpression(ctx, qe);
            //     list = linqQuery.ToList();
            //
            //     if (xmlDoc.IsAggregateFetchXml())
            //     {
            //         list = XrmFakedContext.ProcessAggregateFetchXml(ctx, xmlDoc, list);
            //     }
            // }
            // else if (organizationRequest.Query is QueryByAttribute)
            // {
            //     // We instantiate a QueryExpression to be executed as we have the implementation done already
            //     var query = organizationRequest.Query as QueryByAttribute;
            //     qe = new QueryExpression(query.EntityName);
            //     entityName = qe.EntityName;
            //
            //     qe.ColumnSet = query.ColumnSet;
            //     qe.Criteria = new FilterExpression();
            //     for (var i = 0; i < query.Attributes.Count; i++)
            //     {
            //         qe.Criteria.AddCondition(new ConditionExpression(query.Attributes[i], ConditionOperator.Equal, query.Values[i]));
            //     }
            //
            //     foreach (var order in query.Orders)
            //     {
            //         qe.AddOrder(order.AttributeName, order.OrderType);
            //     }
            //
            //     qe.PageInfo = query.PageInfo;
            //     qe.TopCount = query.TopCount;
            //
            //     // QueryExpression now done... execute it!
            //     var linqQuery = QueryProcessor.ExecuteQueryExpression(ctx, qe);
            //     list = linqQuery.ToList();
            // }
            // else
            // {
            //     throw PullRequestException.NotImplementedOrganizationRequest(request.Query.GetType());
            // }
            //
            //  if (qe.Distinct)
            // {
            //     list = GetDistinctEntities(list);
            // }
            //
            // // Handle the top count before taking paging into account
            // if (qe.TopCount != null && qe.TopCount.Value < list.Count)
            // {
            //     list = list.Take(qe.TopCount.Value).ToList();
            // }
            //
            // // Handle TotalRecordCount here?
            // int totalRecordCount = -1;
            // if (qe?.PageInfo?.ReturnTotalRecordCount == true)
            // {
            //     totalRecordCount = list.Count;
            // }
            //
            // // Handle paging
            // var pageSize = ctx.MaxRetrieveCount;
            // pageInfo = qe.PageInfo;
            // int pageNumber = 1;
            // if (pageInfo != null && pageInfo.PageNumber > 0)
            // {
            //     pageNumber = pageInfo.PageNumber;
            //     pageSize = pageInfo.Count == 0 ? ctx.MaxRetrieveCount : pageInfo.Count;
            // }
            //
            // // Figure out where in the list we need to start and how many items we need to grab
            // int numberToGet = pageSize;
            // int startPosition = 0;
            //
            // if (pageNumber != 1)
            // {
            //     startPosition = (pageNumber - 1) * pageSize;
            // }
            //
            // if (list.Count < pageSize)
            // {
            //     numberToGet = list.Count;
            // }
            // else if (list.Count - pageSize * (pageNumber - 1) < pageSize)
            // {
            //     numberToGet = list.Count - (pageSize * (pageNumber - 1));
            // }
            //
            // var recordsToReturn = startPosition + numberToGet > list.Count ? new List<Entity>() : list.GetRange(startPosition, numberToGet);
            //
            // recordsToReturn.ForEach(e => e.ApplyDateBehaviour(ctx));
            // recordsToReturn.ForEach(e => PopulateFormattedValues(e));
            //
            // var response = new RetrieveMultipleResponse
            // {
            //     Results = new ParameterCollection
            //                      {
            //                         { "EntityCollection", new EntityCollection(recordsToReturn) }
            //                      }
            // };
            // response.EntityCollection.EntityName = entityName;
            // response.EntityCollection.MoreRecords = (list.Count - pageSize * pageNumber) > 0;
            // response.EntityCollection.TotalRecordCount = totalRecordCount;
            //
            // if (response.EntityCollection.MoreRecords)
            // {
            //     var first = response.EntityCollection.Entities.First();
            //     var last = response.EntityCollection.Entities.Last();
            //     response.EntityCollection.PagingCookie = $"<cookie page=\"{pageNumber}\"><{first.LogicalName}id last=\"{last.Id.ToString("B").ToUpper()}\" first=\"{first.Id.ToString("B").ToUpper()}\" /></cookie>";
            // }

            return response;
        }

    }
