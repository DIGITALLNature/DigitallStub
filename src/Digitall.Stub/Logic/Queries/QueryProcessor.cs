// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries
{
    public class QueryProcessor
    {
        private readonly DataverseStub _state;
        readonly LinkedEntitiesProcessor _linkedEntitiesProcessor;
        readonly ExpressionProcessor _expressionProcessor;

        public QueryProcessor(DataverseStub state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _linkedEntitiesProcessor = new LinkedEntitiesProcessor(_state, this);
            _expressionProcessor = new ExpressionProcessor(_state);
        }

        public IQueryable<Entity> ExecuteQueryExpression(QueryExpression queryExpression)
        {
            if (queryExpression == null)
            {
                throw new ArgumentNullException(nameof(queryExpression));
            }

            _state.ThrowIfNotKnownEntityType(queryExpression.EntityName);
            var query = _state.CreateQuery<Entity>(queryExpression.EntityName);

            query = _linkedEntitiesProcessor.FilterQuery(queryExpression, query);
            query = _expressionProcessor.FilterQuery(queryExpression,query);

            query = OrderQuery(queryExpression, query);
            query = ProjectQuery(queryExpression, query);

            return query;
        }

        public QueryExpression ConvertXmlDocumentToQueryExpression(XDocument xmlDocument)
        {
            Debug.Assert(xmlDocument != null, nameof(xmlDocument) + " != null");
            ValidateXmlDocument(xmlDocument);

            var entityNode = RetrieveFetchXmlNode(xmlDocument, "entity");
            var query = new QueryExpression(entityNode.GetAttribute("name").Value);

            query.ColumnSet = xmlDocument.ToColumnSet();

            // Ordering is done after grouping/aggregation
            if (!xmlDocument.IsAggregateFetchXml())
            {
                var orders = xmlDocument.ToOrderExpressionList();
                foreach (var order in orders)
                {
                    query.AddOrder(order.AttributeName, order.OrderType);
                }
            }

            query.Distinct = xmlDocument.IsDistincFetchXml();

            query.Criteria = xmlDocument.ToCriteria();

            query.TopCount = xmlDocument.ToTopCount();

            query.PageInfo.Count = xmlDocument.ToCount() ?? 0;
            query.PageInfo.PageNumber = xmlDocument.ToPageNumber() ?? 1;
            query.PageInfo.ReturnTotalRecordCount = xmlDocument.ToReturnTotalRecordCount();

            var linkedEntities = xmlDocument.ToLinkEntities();
            foreach (var le in linkedEntities)
            {
                query.LinkEntities.Add(le);
            }

            return query;
        }

        private static void ValidateXmlDocument(XDocument xmlDocument)
        {
            //Validate nodes
            if (!xmlDocument.Descendants().All(el => el.IsFetchXmlNodeValid()))
                throw new Exception("At least some node is not valid");

            //Root node
            if (!xmlDocument.Root.Name.LocalName.Equals("fetch", StringComparison.Ordinal))
            {
                throw new Exception("Root node must be fetch");
            }
        }

        private IQueryable<Entity> ProjectQuery(QueryExpression queryExpression, IQueryable<Entity> query)
        {
           query = query.Select(x => x.CloneEntity().ProjectAttributes(queryExpression, _state));
            return query;
        }


        private static IQueryable<Entity> OrderQuery(QueryExpression qe, IQueryable<Entity> query)
        {
            //Sort results
            if (qe.Orders != null)
            {
                if (qe.Orders.Count > 0)
                {
                    IOrderedQueryable<Entity> orderedQuery = null;

                    var order = qe.Orders[0];
                    if (order.OrderType == OrderType.Ascending)
                    {
                        orderedQuery = query.OrderBy(e => e.Attributes.ContainsKey(order.AttributeName) ? e[order.AttributeName] : null, new XrmOrderByAttributeComparer());
                    }
                    else
                    {
                        orderedQuery = query.OrderByDescending(e => e.Attributes.ContainsKey(order.AttributeName) ? e[order.AttributeName] : null, new XrmOrderByAttributeComparer());
                    }

                    //Subsequent orders should use ThenBy and ThenByDescending
                    for (var i = 1; i < qe.Orders.Count; i++)
                    {
                        var thenOrder = qe.Orders[i];
                        if (thenOrder.OrderType == OrderType.Ascending)
                        {
                            orderedQuery = orderedQuery.ThenBy(e => e.Attributes.ContainsKey(thenOrder.AttributeName) ? e[thenOrder.AttributeName] : null, new XrmOrderByAttributeComparer());
                        }
                        else
                        {
                            orderedQuery = orderedQuery.ThenByDescending(e => e[thenOrder.AttributeName], new XrmOrderByAttributeComparer());
                        }
                    }

                    query = orderedQuery;
                }
            }

            return query;
        }

        private static XElement RetrieveFetchXmlNode(XContainer xContainer, string nodeName)
        {
            Debug.Assert(xContainer != null, nameof(xContainer) + " != null");
            return xContainer.Descendants().FirstOrDefault(e => e.Name.LocalName.Equals(nodeName, StringComparison.Ordinal));
        }
    }
}
