// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries;

internal class FetchProcessor(DataverseStub state)
{
    private readonly IEnumerable<ConditionOperator> _operatorsNotToConvertArray =
    [
        ConditionOperator.OlderThanXWeeks,
        ConditionOperator.OlderThanXYears,
        ConditionOperator.OlderThanXDays,
        ConditionOperator.OlderThanXHours,
        ConditionOperator.OlderThanXMinutes,
        ConditionOperator.OlderThanXMonths,
        ConditionOperator.LastXDays,
        ConditionOperator.LastXHours,
        ConditionOperator.LastXMonths,
        ConditionOperator.LastXWeeks,
        ConditionOperator.LastXYears,
        ConditionOperator.NextXHours,
        ConditionOperator.NextXDays,
        ConditionOperator.NextXWeeks,
        ConditionOperator.NextXMonths,
        ConditionOperator.NextXYears,
        ConditionOperator.NextXWeeks,
        ConditionOperator.InFiscalYear
    ];

    public static string GetAssociatedEntityNameForConditionExpression(XElement el)
    {
        while (el != null)
        {
            var parent = el.Parent;
            if (parent.Name.LocalName.Equals("entity") || parent.Name.LocalName.Equals("link-entity"))
            {
                return parent.GetAttribute("name").Value;
            }

            el = parent;
        }

        return null;
    }

    private static object GetValueBasedOnType(Type t, string value)
    {
        if (t == typeof(int) || t == typeof(int?) || t.IsOptionSet() || t.IsOptionSetValueCollection())
        {
            var intValue = 0;

            if (int.TryParse(value, out intValue))
            {
                if (t.IsOptionSet())
                {
                    return new OptionSetValue(intValue);
                }

                return intValue;
            }

            throw new Exception("Integer value expected");
        }

        if (t == typeof(Guid) || t == typeof(Guid?) || t == typeof(EntityReference))
        {
            if (Guid.TryParse(value, out var result))
            {
                if (t == typeof(EntityReference))
                {
                    return new EntityReference { Id = result };
                }

                return result;
            }

            throw new Exception("Guid value expected");
        }

        if (t == typeof(decimal) || t == typeof(decimal?) || t == typeof(Money))
        {
            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                if (t == typeof(Money))
                {
                    return new Money(result);
                }

                return result;
            }

            throw new Exception("Decimal value expected");
        }

        if (t == typeof(double) || t == typeof(double?)) {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new Exception("Double value expected");
        }

        if (t == typeof(float) || t == typeof(float?))
        {
            if (float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new Exception("Float value expected");
        }

        if (t == typeof(DateTime) || t == typeof(DateTime?))
        {
            if (DateTime.TryParse(value, out var result))
            {
                return result;
            }

            throw new Exception("DateTime value expected");
        }
        //fix Issue #141

        if (t == typeof(bool) || t == typeof(bool?))
        {
            if (bool.TryParse(value, out var result))
            {
                return result;
            }

            switch (value)
            {
                case "0": return false;
                case "1": return true;
                default:
                    throw new Exception("Boolean value expected");
            }
        }

        //Otherwise, return the string
        return value;
    }

    private bool ValueNeedsConverting(ConditionOperator conditionOperator) => !_operatorsNotToConvertArray.Contains(conditionOperator);

    private FilterExpression ExractFilterExpression(XElement elem)
    {
        var filterExpression = new FilterExpression();

        var filterType = elem.GetAttribute("type");
        if (filterType == null)
        {
            filterExpression.FilterOperator = LogicalOperator.And; //By default
        }
        else
        {
            filterExpression.FilterOperator = filterType.Value.Equals("and") ? LogicalOperator.And : LogicalOperator.Or;
        }

        //Process other filters recursively
        var otherFilters = elem
            .Elements() //child nodes of this filter
            .Where(el => el.Name.LocalName.Equals("filter"))
            .Select(ExractFilterExpression)
            .ToList();


        //Process conditions
        var conditions = elem
            .Elements() //child nodes of this filter
            .Where(el => el.Name.LocalName.Equals("condition"))
            .Select(ExtractConditionExpression)
            .ToList();

        foreach (var c in conditions)
        {
            filterExpression.AddCondition(c);
        }

        foreach (var f in otherFilters)
        {
            filterExpression.AddFilter(f);
        }

        return filterExpression;
    }

    public ConditionExpression ExtractConditionExpression(XElement elem)
    {
        var conditionEntityName = "";

        var attributeName = elem.GetAttribute("attribute").Value;
        var op = ConditionOperator.Equal;

        string value = null;
        if (elem.GetAttribute("value") != null)
        {
            value = elem.GetAttribute("value").Value;
        }

        if (elem.GetAttribute("entityname") != null)
        {
            conditionEntityName = elem.GetAttribute("entityname").Value;
        }

        switch (elem.GetAttribute("operator").Value)
        {
            case "eq":
                op = ConditionOperator.Equal;
                break;
            case "ne":
            case "neq":
                op = ConditionOperator.NotEqual;
                break;
            case "begins-with":
                op = ConditionOperator.BeginsWith;
                break;
            case "not-begin-with":
                op = ConditionOperator.DoesNotBeginWith;
                break;
            case "ends-with":
                op = ConditionOperator.EndsWith;
                break;
            case "not-end-with":
                op = ConditionOperator.DoesNotEndWith;
                break;
            case "in":
                op = ConditionOperator.In;
                break;
            case "not-in":
                op = ConditionOperator.NotIn;
                break;
            case "null":
                op = ConditionOperator.Null;
                break;
            case "not-null":
                op = ConditionOperator.NotNull;
                break;
            case "like":
                op = ConditionOperator.Like;

                if (value != null)
                {
                    if (value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        op = ConditionOperator.EndsWith;
                    }
                    else if (!value.StartsWith("%") && value.EndsWith("%"))
                    {
                        op = ConditionOperator.BeginsWith;
                    }
                    else if (value.StartsWith("%") && value.EndsWith("%"))
                    {
                        op = ConditionOperator.Contains;
                    }

                    value = value.Replace("%", "");
                }

                break;
            case "not-like":
                op = ConditionOperator.NotLike;
                if (value != null)
                {
                    if (value.StartsWith("%") && !value.EndsWith("%"))
                    {
                        op = ConditionOperator.DoesNotEndWith;
                    }
                    else if (!value.StartsWith("%") && value.EndsWith("%"))
                    {
                        op = ConditionOperator.DoesNotBeginWith;
                    }
                    else if (value.StartsWith("%") && value.EndsWith("%"))
                    {
                        op = ConditionOperator.DoesNotContain;
                    }

                    value = value.Replace("%", "");
                }

                break;
            case "gt":
                op = ConditionOperator.GreaterThan;
                break;
            case "ge":
                op = ConditionOperator.GreaterEqual;
                break;
            case "lt":
                op = ConditionOperator.LessThan;
                break;
            case "le":
                op = ConditionOperator.LessEqual;
                break;
            case "on":
                op = ConditionOperator.On;
                break;
            case "on-or-before":
                op = ConditionOperator.OnOrBefore;
                break;
            case "on-or-after":
                op = ConditionOperator.OnOrAfter;
                break;
            case "today":
                op = ConditionOperator.Today;
                break;
            case "yesterday":
                op = ConditionOperator.Yesterday;
                break;
            case "tomorrow":
                op = ConditionOperator.Tomorrow;
                break;
            case "between":
                op = ConditionOperator.Between;
                break;
            case "not-between":
                op = ConditionOperator.NotBetween;
                break;
            case "eq-userid":
                op = ConditionOperator.EqualUserId;
                break;
            case "ne-userid":
                op = ConditionOperator.NotEqualUserId;
                break;
            case "olderthan-x-months":
                op = ConditionOperator.OlderThanXMonths;
                break;
            case "last-seven-days":
                op = ConditionOperator.Last7Days;
                break;
            case "eq-businessid":
                op = ConditionOperator.EqualBusinessId;
                break;
            case "neq-businessid":
                op = ConditionOperator.NotEqualBusinessId;
                break;
            case "next-x-weeks":
                op = ConditionOperator.NextXWeeks;
                break;
            case "next-seven-days":
                op = ConditionOperator.Next7Days;
                break;
            case "this-year":
                op = ConditionOperator.ThisYear;
                break;
            case "last-year":
                op = ConditionOperator.LastYear;
                break;
            case "next-year":
                op = ConditionOperator.NextYear;
                break;
            case "last-x-hours":
                op = ConditionOperator.LastXHours;
                break;
            case "last-x-days":
                op = ConditionOperator.LastXDays;
                break;
            case "last-x-weeks":
                op = ConditionOperator.LastXWeeks;
                break;
            case "last-x-months":
                op = ConditionOperator.LastXMonths;
                break;
            case "last-x-years":
                op = ConditionOperator.LastXYears;
                break;
            case "next-x-hours":
                op = ConditionOperator.NextXHours;
                break;
            case "next-x-days":
                op = ConditionOperator.NextXDays;
                break;
            case "next-x-months":
                op = ConditionOperator.NextXMonths;
                break;
            case "next-x-years":
                op = ConditionOperator.NextXYears;
                break;
            case "this-month":
                op = ConditionOperator.ThisMonth;
                break;
            case "last-month":
                op = ConditionOperator.LastMonth;
                break;
            case "next-month":
                op = ConditionOperator.NextMonth;
                break;
            case "last-week":
                op = ConditionOperator.LastWeek;
                break;
            case "this-week":
                op = ConditionOperator.ThisWeek;
                break;
            case "next-week":
                op = ConditionOperator.NextWeek;
                break;
            case "in-fiscal-year":
                op = ConditionOperator.InFiscalYear;
                break;
            case "olderthan-x-minutes":
                op = ConditionOperator.OlderThanXMinutes;
                break;
            case "olderthan-x-hours":
                op = ConditionOperator.OlderThanXHours;
                break;
            case "olderthan-x-days":
                op = ConditionOperator.OlderThanXDays;
                break;
            case "olderthan-x-weeks":
                op = ConditionOperator.OlderThanXWeeks;
                break;
            case "olderthan-x-years":
                op = ConditionOperator.OlderThanXYears;
                break;

            case "contain-values":
                op = ConditionOperator.ContainValues;
                break;
            case "not-contain-values":
                op = ConditionOperator.DoesNotContainValues;
                break;

            default:
                throw new ArgumentOutOfRangeException(elem.GetAttribute("operator").Value);
        }

        //Process values
        object[] values = null;


        var entityName = GetAssociatedEntityNameForConditionExpression(elem);

        //Find values inside the condition expression, if apply
        values = elem
            .Elements() //child nodes of this filter
            .Where(el => el.Name.LocalName.Equals("value"))
            .Select(el => GetConditionExpressionValueCast(el.Value, entityName, attributeName, op))
            .ToArray();


        //Otherwise, a single value was used
        if (value != null)
        {
            if (string.IsNullOrWhiteSpace(conditionEntityName))
            {
                return new ConditionExpression(attributeName, op, GetConditionExpressionValueCast(value, entityName, attributeName, op));
            }

            return new ConditionExpression(conditionEntityName, attributeName, op, GetConditionExpressionValueCast(value, entityName, attributeName, op));
        }


        if (string.IsNullOrWhiteSpace(conditionEntityName))
        {
            return new ConditionExpression(attributeName, op, values);
        }

        return new ConditionExpression(conditionEntityName, attributeName, op, values);
    }

    public FilterExpression ExtractCriteria(XDocument xmlDocument) =>
        xmlDocument.Elements() //fetch
            .Elements() //entity
            .Elements() //child nodes of entity
            .Where(el => el.Name.LocalName.Equals("filter"))
            .Select(ExractFilterExpression)
            .FirstOrDefault();

    public IEnumerable<LinkEntity> ExtractLinkEntities(XDocument xmlDocument) =>
        xmlDocument.Elements() //fetch
            .Elements() //entity
            .Elements() //child nodes of entity
            .Where(el => el.Name.LocalName.Equals("link-entity", StringComparison.Ordinal))
            .Select(el => ExtractLinkLinkEntity(el))
            .ToList();

    public LinkEntity ExtractLinkLinkEntity(XElement el)
    {
        //Create this node
        var linkEntity = new LinkEntity();

        linkEntity.LinkFromEntityName = el.Parent.GetAttribute("name").Value;
        linkEntity.LinkFromAttributeName = el.GetAttribute("to").Value;
        linkEntity.LinkToAttributeName = el.GetAttribute("from").Value;
        linkEntity.LinkToEntityName = el.GetAttribute("name").Value;

        if (el.GetAttribute("alias") != null)
        {
            linkEntity.EntityAlias = el.GetAttribute("alias").Value;
        }

        //Join operator
        if (el.GetAttribute("link-type") != null)
        {
            switch (el.GetAttribute("link-type").Value)
            {
                case "outer":
                    linkEntity.JoinOperator = JoinOperator.LeftOuter;
                    break;
                default:
                    linkEntity.JoinOperator = JoinOperator.Inner;
                    break;
            }
        }

        //Process other link entities recursively
        var convertedLinkEntityNodes = el.Elements()
            .Where(e => e.Name.LocalName.Equals("link-entity", StringComparison.Ordinal))
            .Select(ExtractLinkLinkEntity)
            .ToList();

        foreach (var le in convertedLinkEntityNodes)
        {
            linkEntity.LinkEntities.Add(le);
        }

        //Process column sets
        linkEntity.Columns = el.ToColumnSet();

        //Process filter
        linkEntity.LinkCriteria = el.Elements()
            .Where(e => e.Name.LocalName.Equals("filter"))
            .Select(ExractFilterExpression)
            .FirstOrDefault();

        return linkEntity;
    }

    public void ValidateXmlDocument(XDocument xmlDocument)
    {
        //Validate nodes
        if (!xmlDocument.Descendants().All(el => el.IsFetchXmlNodeValid()))
        {
            throw new Exception("At least some node is not valid");
        }

        //Root node
        if (!xmlDocument.Root.Name.LocalName.Equals("fetch", StringComparison.Ordinal))
        {
            throw new Exception("Root node must be fetch");
        }
    }

    private object GetConditionExpressionValueCast(string value, string entityName, string sAttributeName, ConditionOperator op)
    {
        if (state.IsKnownAttributeForType(entityName, sAttributeName, out var attributeType))
        {
            try
            {
                if (ValueNeedsConverting(op))
                {
                    return GetValueBasedOnType(attributeType.PropertyType, value);
                }

                return int.Parse(value);
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("When trying to parse value for entity {0} and attribute {1}: {2}", entityName, sAttributeName, e.Message));
            }
        }


        //Try parsing a guid
        var gOut = Guid.Empty;
        if (Guid.TryParse(value, out gOut))
        {
            return gOut;
        }

        //Try checking if it is a numeric value, cause, from the fetchxml it
        //would be impossible to know the real typed based on the string value only
        // ex: "123" might compared as a string, or, as an int, it will depend on the attribute
        //    data type, therefore, in this case we do need to use proxy types

        var bIsNumeric = false;
        var bIsDateTime = false;
        var dblValue = 0.0;
        var decValue = 0.0m;
        var intValue = 0;

        if (double.TryParse(value, out dblValue))
        {
            bIsNumeric = true;
        }

        if (decimal.TryParse(value, out decValue))
        {
            bIsNumeric = true;
        }

        if (int.TryParse(value, out intValue))
        {
            bIsNumeric = true;
        }

        var dtValue = DateTime.MinValue;
        if (DateTime.TryParse(value, out dtValue))
        {
            bIsDateTime = true;
        }

        if (bIsNumeric || bIsDateTime)
        {
            throw new Exception("When using arithmetic values in Fetch a ProxyTypesAssembly must be used in order to know which types to cast values to.");
        }

        //Default value
        return value;
    }
}
