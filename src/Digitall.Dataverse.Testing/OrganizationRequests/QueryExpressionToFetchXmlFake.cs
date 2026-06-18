// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Globalization;
using System.Text;
using System.Xml;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class QueryExpressionToFetchXmlFake : OrganizationRequestFake<QueryExpressionToFetchXmlRequest, QueryExpressionToFetchXmlResponse>
{
    public override QueryExpressionToFetchXmlResponse Execute(QueryExpressionToFetchXmlRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        if (organizationRequest.Query is not QueryExpression query)
        {
            throw new ArgumentException($"Query type {organizationRequest.Query?.GetType().Name ?? "null"} is not supported. Only {nameof(QueryExpression)} is supported.",
                nameof(organizationRequest));
        }

        var fetchXml = ConvertToFetchXml(query);

        return new QueryExpressionToFetchXmlResponse
        {
            Results =
            {
                ["FetchXml"] = fetchXml
            }
        };
    }

    private static string ConvertToFetchXml(QueryExpression query)
    {
        var sb = new StringBuilder();
        var settings = new XmlWriterSettings
        {
            OmitXmlDeclaration = true,
            Indent = false
        };

        using (var writer = XmlWriter.Create(sb, settings))
        {
            writer.WriteStartElement("fetch");

            if (query.PageInfo is { Count: > 0 })
            {
                writer.WriteAttributeString("count", query.PageInfo.Count.ToString(CultureInfo.InvariantCulture));
            }

            if (query.PageInfo is { PageNumber: > 0 })
            {
                writer.WriteAttributeString("page", query.PageInfo.PageNumber.ToString(CultureInfo.InvariantCulture));
            }

            if (query.PageInfo?.ReturnTotalRecordCount == true)
            {
                writer.WriteAttributeString("returntotalrecordcount", "true");
            }

            if (query.TopCount.HasValue)
            {
                writer.WriteAttributeString("top", query.TopCount.Value.ToString(CultureInfo.InvariantCulture));
            }

            if (query.Distinct)
            {
                writer.WriteAttributeString("distinct", "true");
            }

            if (query.NoLock)
            {
                writer.WriteAttributeString("no-lock", "true");
            }

            if (!string.IsNullOrEmpty(query.PageInfo?.PagingCookie))
            {
                writer.WriteAttributeString("paging-cookie", query.PageInfo.PagingCookie);
            }

            if (string.IsNullOrWhiteSpace(query.EntityName))
            {
                throw new ArgumentException($"{nameof(QueryExpression.EntityName)} is required to produce valid FetchXml.", nameof(query));
            }

            writer.WriteStartElement("entity");
            writer.WriteAttributeString("name", query.EntityName);

            WriteColumnSet(writer, query.ColumnSet);

            foreach (var order in query.Orders)
            {
                WriteOrder(writer, order);
            }

            if (query.Criteria != null && HasContent(query.Criteria))
            {
                WriteFilter(writer, query.Criteria);
            }

            foreach (var link in query.LinkEntities)
            {
                WriteLinkEntity(writer, link);
            }

            writer.WriteEndElement(); // entity
            writer.WriteEndElement(); // fetch
        }

        return sb.ToString();
    }

    private static string FormatValue(object value) =>
        value switch
        {
            null => string.Empty,
            string s => s,
            bool b => b ? "true" : "false",
            DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };

    private static bool HasContent(FilterExpression filter) =>
        filter.Conditions.Count > 0 || filter.Filters.Any(HasContent);

    private static string MapOperator(ConditionOperator op) =>
        op switch
        {
            ConditionOperator.Equal => "eq",
            ConditionOperator.NotEqual => "ne",
            ConditionOperator.GreaterThan => "gt",
            ConditionOperator.LessThan => "lt",
            ConditionOperator.GreaterEqual => "ge",
            ConditionOperator.LessEqual => "le",
            ConditionOperator.Like => "like",
            ConditionOperator.NotLike => "not-like",
            ConditionOperator.In => "in",
            ConditionOperator.NotIn => "not-in",
            ConditionOperator.Between => "between",
            ConditionOperator.NotBetween => "not-between",
            ConditionOperator.Null => "null",
            ConditionOperator.NotNull => "not-null",
            ConditionOperator.Yesterday => "yesterday",
            ConditionOperator.Today => "today",
            ConditionOperator.Tomorrow => "tomorrow",
            ConditionOperator.Last7Days => "last-seven-days",
            ConditionOperator.Next7Days => "next-seven-days",
            ConditionOperator.LastWeek => "last-week",
            ConditionOperator.ThisWeek => "this-week",
            ConditionOperator.NextWeek => "next-week",
            ConditionOperator.LastMonth => "last-month",
            ConditionOperator.ThisMonth => "this-month",
            ConditionOperator.NextMonth => "next-month",
            ConditionOperator.On => "on",
            ConditionOperator.OnOrBefore => "on-or-before",
            ConditionOperator.OnOrAfter => "on-or-after",
            ConditionOperator.LastYear => "last-year",
            ConditionOperator.ThisYear => "this-year",
            ConditionOperator.NextYear => "next-year",
            ConditionOperator.LastXHours => "last-x-hours",
            ConditionOperator.NextXHours => "next-x-hours",
            ConditionOperator.LastXDays => "last-x-days",
            ConditionOperator.NextXDays => "next-x-days",
            ConditionOperator.LastXWeeks => "last-x-weeks",
            ConditionOperator.NextXWeeks => "next-x-weeks",
            ConditionOperator.LastXMonths => "last-x-months",
            ConditionOperator.NextXMonths => "next-x-months",
            ConditionOperator.OlderThanXMonths => "olderthan-x-months",
            ConditionOperator.LastXYears => "last-x-years",
            ConditionOperator.NextXYears => "next-x-years",
            ConditionOperator.EqualUserId => "eq-userid",
            ConditionOperator.NotEqualUserId => "ne-userid",
            ConditionOperator.EqualBusinessId => "eq-businessid",
            ConditionOperator.NotEqualBusinessId => "neq-businessid",
            ConditionOperator.BeginsWith => "begins-with",
            ConditionOperator.DoesNotBeginWith => "not-begin-with",
            ConditionOperator.EndsWith => "ends-with",
            ConditionOperator.DoesNotEndWith => "not-end-with",
            ConditionOperator.Contains => "like",
            ConditionOperator.DoesNotContain => "not-like",
            ConditionOperator.InFiscalYear => "in-fiscal-year",
            ConditionOperator.OlderThanXMinutes => "olderthan-x-minutes",
            ConditionOperator.OlderThanXHours => "olderthan-x-hours",
            ConditionOperator.OlderThanXDays => "olderthan-x-days",
            ConditionOperator.OlderThanXWeeks => "olderthan-x-weeks",
            ConditionOperator.OlderThanXYears => "olderthan-x-years",
            ConditionOperator.ContainValues => "contain-values",
            ConditionOperator.DoesNotContainValues => "not-contain-values",
            _ => op.ToString().ToLowerInvariant()
        };

    private static void WriteColumnSet(XmlWriter writer, ColumnSet? columnSet)
    {
        if (columnSet == null)
        {
            return;
        }

        if (columnSet.AllColumns)
        {
            writer.WriteStartElement("all-attributes");
            writer.WriteEndElement();
            return;
        }

        foreach (var column in columnSet.Columns)
        {
            writer.WriteStartElement("attribute");
            writer.WriteAttributeString("name", column);
            writer.WriteEndElement();
        }
    }

    private static void WriteCondition(XmlWriter writer, ConditionExpression condition)
    {
        writer.WriteStartElement("condition");

        if (!string.IsNullOrEmpty(condition.EntityName))
        {
            writer.WriteAttributeString("entityname", condition.EntityName);
        }

        writer.WriteAttributeString("attribute", condition.AttributeName);
        writer.WriteAttributeString("operator", MapOperator(condition.Operator));

        var values = condition.Values?.Where(v => v != null).ToList() ?? [];

        if (values.Count == 1)
        {
            writer.WriteAttributeString("value", FormatValue(values[0]));
        }
        else if (values.Count > 1)
        {
            foreach (var value in values)
            {
                writer.WriteStartElement("value");
                writer.WriteString(FormatValue(value));
                writer.WriteEndElement();
            }
        }

        writer.WriteEndElement();
    }

    private static void WriteFilter(XmlWriter writer, FilterExpression filter)
    {
        writer.WriteStartElement("filter");
        writer.WriteAttributeString("type", filter.FilterOperator == LogicalOperator.Or ? "or" : "and");

        foreach (var condition in filter.Conditions)
        {
            WriteCondition(writer, condition);
        }

        foreach (var nested in filter.Filters)
        {
            if (HasContent(nested))
            {
                WriteFilter(writer, nested);
            }
        }

        writer.WriteEndElement();
    }

    private static void WriteLinkEntity(XmlWriter writer, LinkEntity link)
    {
        writer.WriteStartElement("link-entity");
        writer.WriteAttributeString("name", link.LinkToEntityName);
        writer.WriteAttributeString("from", link.LinkToAttributeName);
        writer.WriteAttributeString("to", link.LinkFromAttributeName);

        if (!string.IsNullOrEmpty(link.EntityAlias))
        {
            writer.WriteAttributeString("alias", link.EntityAlias);
        }

        writer.WriteAttributeString("link-type", link.JoinOperator == JoinOperator.LeftOuter ? "outer" : "inner");

        WriteColumnSet(writer, link.Columns);

        foreach (var order in link.Orders)
        {
            WriteOrder(writer, order);
        }

        if (link.LinkCriteria != null && HasContent(link.LinkCriteria))
        {
            WriteFilter(writer, link.LinkCriteria);
        }

        foreach (var nested in link.LinkEntities)
        {
            WriteLinkEntity(writer, nested);
        }

        writer.WriteEndElement();
    }

    private static void WriteOrder(XmlWriter writer, OrderExpression order)
    {
        writer.WriteStartElement("order");
        writer.WriteAttributeString("attribute", order.AttributeName);
        writer.WriteAttributeString("descending", (order.OrderType == OrderType.Descending).ToString().ToLowerInvariant());
        writer.WriteEndElement();
    }
}
