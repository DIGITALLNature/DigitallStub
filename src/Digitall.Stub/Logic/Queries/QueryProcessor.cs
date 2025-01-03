// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Digitall.Stub.Logic.Queries.FetchAggregation;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries
{
    public class QueryProcessor
    {
        private readonly DataverseStub _state;
        readonly LinkedEntitiesProcessor _linkedEntitiesProcessor;
        readonly ExpressionProcessor _expressionProcessor;
        readonly FetchProcessor _fetchProcessor;

        public QueryProcessor(DataverseStub state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _linkedEntitiesProcessor = new LinkedEntitiesProcessor(_state, this);
            _expressionProcessor = new ExpressionProcessor(_state);
            _fetchProcessor = new FetchProcessor(_state);
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
            if (xmlDocument == null)
            {
                throw new ArgumentNullException(nameof(xmlDocument));
            }
            _fetchProcessor.ValidateXmlDocument(xmlDocument);

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

            query.Criteria =   _fetchProcessor.ExtractCriteria(xmlDocument);

            query.TopCount = xmlDocument.ToTopCount();

            query.PageInfo.Count = xmlDocument.ToCount() ?? 0;
            query.PageInfo.PageNumber = xmlDocument.ToPageNumber() ?? 1;
            query.PageInfo.ReturnTotalRecordCount = xmlDocument.ToReturnTotalRecordCount();

            var linkedEntities = _fetchProcessor.ExtractLinkEntities(xmlDocument);
            foreach (var le in linkedEntities)
            {
                query.LinkEntities.Add(le);
            }

            return query;
        }

        public List<Entity> ProcessAggregateFetchXml(XDocument xmlDoc, List<Entity> internalResult)
        {
           // Validate that <all-attributes> is not present,
            // that all attributes have groupby or aggregate, and an alias,
            // and that there is exactly 1 groupby.
            if (RetrieveFetchXmlNode(xmlDoc, "all-attributes") != null)
            {
                throw new Exception("Can't have <all-attributes /> present when using aggregate");
            }

            var ns = xmlDoc.Root.Name.Namespace;

            var entityName = RetrieveFetchXmlNode(xmlDoc, "entity")?.GetAttribute("name")?.Value;
            if (string.IsNullOrEmpty(entityName))
            {
                throw new Exception("Can't find entity name for aggregate query");
            }

            var aggregates = new List<FetchAggregate>();
            var groups = new List<FetchGrouping>();

            foreach (var attr in xmlDoc.Descendants(ns + "attribute"))
            {
                //TODO: Find entity alias. Handle aliasedvalue in the query result.
                var namespacedAlias = attr.Ancestors(ns + "link-entity").Select(x => x.GetAttribute("alias")?.Value != null ? x.GetAttribute("alias").Value : x.GetAttribute("name").Value).ToList();
                namespacedAlias.Add(attr.GetAttribute("alias")?.Value);
                var alias = string.Join(".", namespacedAlias);
                namespacedAlias.RemoveAt(namespacedAlias.Count - 1);
                namespacedAlias.Add(attr.GetAttribute("name")?.Value);
                var logicalName = string.Join(".", namespacedAlias);

                if (string.IsNullOrEmpty("alias"))
                {
                    throw new Exception("Missing alias for attribute in aggregate fetch xml");
                }
                if (string.IsNullOrEmpty("name"))
                {
                    throw new Exception("Missing name for attribute in aggregate fetch xml");
                }

                if (attr.IsAttributeTrue("groupby"))
                {
                    var dategrouping = attr.GetAttribute("dategrouping")?.Value;
                    if (dategrouping != null)
                    {
                        DateGroupType t;
                        if (!Enum.TryParse(dategrouping, true, out t))
                        {
                            throw new Exception("Unknown dategrouping value '" + dategrouping + "'");
                        }
                        groups.Add(new DateTimeGroup
                        {
                            Type = t,
                            OutputAlias = alias,
                            Attribute = logicalName
                        });
                    }
                    else
                    {
                        groups.Add(new SimpleValueGroup
                        {
                            OutputAlias = alias,
                            Attribute = logicalName
                        });
                    }
                }
                else
                {
                    var agrFn = attr.GetAttribute("aggregate")?.Value;
                    if (string.IsNullOrEmpty(agrFn))
                    {
                        throw new Exception("Attributes must have be aggregated or grouped by when using aggregation");
                    }

                    FetchAggregate newAgr = null;
                    switch (agrFn?.ToLower())
                    {
                        case "count":
                            newAgr = new CountAggregate();
                            break;

                        case "countcolumn":
                            if (attr.IsAttributeTrue("distinct"))
                            {
                                newAgr = new CountDistinctAggregate();
                            }
                            else
                            {
                                newAgr = new CountColumnAggregate();
                            }
                            break;

                        case "min":
                            newAgr = new MinAggregate();
                            break;

                        case "max":
                            newAgr = new MaxAggregate();
                            break;

                        case "avg":
                            newAgr = new AvgAggregate();
                            break;

                        case "sum":
                            newAgr = new SumAggregate();
                            break;

                        default:
                            throw new Exception("Unknown aggregate function '" + agrFn + "'");
                    }

                    newAgr.OutputAlias = alias;
                    newAgr.Attribute = logicalName;
                    aggregates.Add(newAgr);
                }
            }

            List<Entity> aggregateResult;

            if (groups.Any())
            {
                aggregateResult = ProcessGroupedAggregate(entityName, internalResult, aggregates, groups);
            }
            else
            {
                aggregateResult = new List<Entity>();
                var ent = ProcessAggregatesForSingleGroup(entityName, internalResult, aggregates);
                aggregateResult.Add(ent);
            }

            return OrderAggregateResult(xmlDoc, aggregateResult.AsQueryable());
        }

        private static List<Entity> ProcessGroupedAggregate(string entityName, IList<Entity> resultOfQuery, IList<FetchAggregate> aggregates, IList<FetchGrouping> groups)
        {
            // Group by the groupBy-attribute
            var grouped = resultOfQuery.GroupBy(e =>
            {
                return groups
                    .Select(g => g.Process(e))
                    .ToArray();
            }, new ArrayComparer());

            // Perform aggregates in each group
            var result = new List<Entity>();
            foreach (var g in grouped)
            {
                var firstInGroup = g.First();

                // Find the aggregates values in the group
                var ent = ProcessAggregatesForSingleGroup(entityName, g, aggregates);

                // Find the group values
                for (var rule = 0; rule < groups.Count; ++rule)
                {
                    if (g.Key[rule] != null)
                    {
                        object value = g.Key[rule];
                        ent[groups[rule].OutputAlias] = new AliasedValue(null, groups[rule].Attribute, value is ComparableEntityReference ? (value as ComparableEntityReference).entityReference : value);
                    }
                }

                result.Add(ent);
            }

            return result;
        }

        private static Entity ProcessAggregatesForSingleGroup(string entityName, IEnumerable<Entity> entities, IList<FetchAggregate> aggregates)
        {
            var ent = new Entity(entityName);

            foreach (var agg in aggregates)
            {
                var val = agg.Process(entities);
                if (val != null)
                {
                    ent[agg.OutputAlias] = new AliasedValue(null, agg.Attribute, val);
                }
                else
                {
                    //if the aggregate value cannot be calculated
                    //CRM still returns an alias
                    ent[agg.OutputAlias] = new AliasedValue(null, agg.Attribute, null);
                }
            }

            return ent;
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


        private static List<Entity> OrderAggregateResult(XDocument xmlDoc, IQueryable<Entity> result)
        {
            var ns = xmlDoc.Root.Name.Namespace;
            foreach (var order in xmlDoc.Root.Element(ns + "entity").Elements(ns + "order"))
            {
                var alias = order.GetAttribute("alias")?.Value;

                // These error is also thrown by CRM
                if (order.GetAttribute("attribute") != null)
                {
                    throw new Exception("An attribute cannot be specified for an order clause for an aggregate Query. Use an alias");
                }
                if (string.IsNullOrEmpty("alias"))
                {
                    throw new Exception("An alias is required for an order clause for an aggregate Query.");
                }

                if (order.IsAttributeTrue("descending"))
                    result = result.OrderByDescending(e => e.Attributes.ContainsKey(alias) ? e.Attributes[alias] : null, new XrmOrderByAttributeComparer());
                else
                    result = result.OrderBy(e => e.Attributes.ContainsKey(alias) ? e.Attributes[alias] : null, new XrmOrderByAttributeComparer());
            }

            return result.ToList();
        }
    }
}
