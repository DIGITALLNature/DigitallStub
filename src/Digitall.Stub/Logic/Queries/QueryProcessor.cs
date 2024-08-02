// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
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
    }
}
