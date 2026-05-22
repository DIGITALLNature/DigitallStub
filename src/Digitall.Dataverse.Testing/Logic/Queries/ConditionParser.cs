// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Logic.Queries;

public static class ConditionParser
{
    // Cached MethodInfo fields — resolved once, reused across all query evaluations
    private static readonly MethodInfo s_stringToLowerInvariant = typeof(string).GetMethod("ToLowerInvariant", Type.EmptyTypes)!;
    private static readonly MethodInfo s_stringContains = typeof(string).GetMethod("Contains", [typeof(string)])!;
    private static readonly MethodInfo s_stringStartsWith = typeof(string).GetMethod("StartsWith", [typeof(string)])!;
    private static readonly MethodInfo s_stringEndsWith = typeof(string).GetMethod("EndsWith", [typeof(string)])!;
    private static readonly MethodInfo s_stringCompareTo = typeof(string).GetMethod("CompareTo", [typeof(string)])!;
    private static readonly MethodInfo s_dateTimeGetDate = typeof(DateTime).GetMethod("get_Date")!;
    private static readonly MethodInfo s_intToString = typeof(int).GetMethod("ToString", Type.EmptyTypes)!;
    private static readonly MethodInfo s_hashSetIntOverlaps = typeof(HashSet<int>).GetMethod("Overlaps")!;
    private static readonly MethodInfo s_hashSetIntSetEquals = typeof(HashSet<int>).GetMethod(nameof(HashSet<>.SetEquals))!;
    private static readonly MethodInfo s_attributeCollectionContainsKey = typeof(AttributeCollection).GetMethod(nameof(AttributeCollection.ContainsKey), [typeof(string)])!;
    private static readonly MethodInfo s_aliasedValueGetValue = typeof(AliasedValue).GetMethod("get_Value")!;
    private static readonly MethodInfo s_entityReferenceGetId = typeof(EntityReference).GetMethod("get_Id")!;
    private static readonly MethodInfo s_entityReferenceGetName = typeof(EntityReference).GetMethod("get_Name")!;
    private static readonly MethodInfo s_moneyGetValue = typeof(Money).GetMethod("get_Value")!;
    private static readonly MethodInfo s_booleanManagedPropertyGetValue = typeof(BooleanManagedProperty).GetMethod("get_Value")!;
    private static readonly MethodInfo s_optionSetValueGetValue = typeof(OptionSetValue).GetMethod("get_Value")!;
    private static readonly MethodInfo s_convertToHashSetOfIntMethod = typeof(ConditionParser).GetMethod(nameof(ConvertToHashSetOfInt))!;

    public static HashSet<int> ConvertToHashSetOfInt(object input, bool isOptionSetValueCollectionAccepted)
    {
        var set = new HashSet<int>();

        var faultReason = $"The formatter threw an exception while trying to deserialize the message: There was an error while trying to deserialize parameter" +
                          $" http://schemas.microsoft.com/xrm/2011/Contracts/Services:query. The InnerException message was 'Error in line 1 position 8295. Element " +
                          $"'http://schemas.microsoft.com/2003/10/Serialization/Arrays:anyType' contains data from a type that maps to the name " +
                          $"'http://schemas.microsoft.com/xrm/2011/Contracts:{input.GetType()}'. The deserializer has no knowledge of any type that maps to this name. " +
                          $"Consider changing the implementation of the ResolveName method on your DataContractResolver to return a non-null value for name " +
                          $"'{input.GetType()}' and namespace 'http://schemas.microsoft.com/xrm/2011/Contracts'.'.  Please see InnerException for more details.";

        switch (input)
        {
            case int intValue:
                set.Add(intValue);
                break;
            case string stringValue:
                set.Add(int.Parse(stringValue));
                break;
            case int[] intArray:
                set.UnionWith(intArray);
                break;
            case string[] strings:
                set.UnionWith(strings.Select(int.Parse));
                break;
            case DataCollection<object> collection when collection.All(o => o is int):
                set.UnionWith(collection.Cast<int>());
                break;
            case DataCollection<object> collection when collection.All(o => o is string):
                set.UnionWith(collection.Select(o => int.Parse((o as string)!)));
                break;
            case DataCollection<object> and [int[] iArray]:
                set.UnionWith(iArray);
                break;
            case DataCollection<object> and [string[] sArray]:
                set.UnionWith(sArray.Select(int.Parse));
                break;
            case DataCollection<object>:
                ThrowFaultException(faultReason);
                break;
            default:
                {
                    if (isOptionSetValueCollectionAccepted && input is OptionSetValueCollection optionSetValueCollection)
                    {
                        set.UnionWith(optionSetValueCollection.Select(osv => osv.Value));
                    }
                    else
                    {
                        ThrowFaultException(faultReason);
                    }

                    break;
                }
        }

        return set;
    }

    [DoesNotReturn]
    private static void ThrowFaultException(string faultReason)
    {
        throw new FaultException(faultReason);
    }

    public static Expression TranslateConditionExpression(QueryExpression queryExpression, FakeOrganizationService organizationService, TypedConditionExpression condition, ParameterExpression entity)
    {
        Expression attributesProperty = Expression.Property(entity, "Attributes");


        string attributeName;

        //Do not prepend the entity name if the EntityLogicalName is the same as the QueryExpression main logical name

        if (!string.IsNullOrWhiteSpace(condition.CondExpression.EntityName) && !condition.CondExpression.EntityName.Equals(queryExpression.EntityName))
        {
            attributeName = condition.CondExpression.EntityName + "." + condition.CondExpression.AttributeName;
        }
        else
        {
            attributeName = condition.CondExpression.AttributeName;
        }

        Expression containsAttributeExpression = Expression.Call(attributesProperty, s_attributeCollectionContainsKey,
            Expression.Constant(attributeName));

        Expression getAttributeValueExpr = Expression.Property(attributesProperty, "Item", Expression.Constant(attributeName, typeof(string)));


        Expression operatorExpression;

        switch (condition.CondExpression.Operator)
        {
            #region equal and not equal

            case ConditionOperator.Equal:
            case ConditionOperator.On:
            case ConditionOperator.Today:
            case ConditionOperator.Yesterday:
            case ConditionOperator.Tomorrow:
            case ConditionOperator.EqualUserId:
            case ConditionOperator.EqualBusinessId:
                operatorExpression = TranslateConditionExpressionEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotOn:
            case ConditionOperator.NotEqual:
            case ConditionOperator.NotEqualUserId:
            case ConditionOperator.NotEqualBusinessId:
                operatorExpression = Expression.Not(TranslateConditionExpressionEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            #endregion

            #region like and not like

            case ConditionOperator.BeginsWith:
            case ConditionOperator.Like:
                operatorExpression = TranslateConditionExpressionLike(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.EndsWith:
                operatorExpression = TranslateConditionExpressionEndsWith(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.Contains:
                operatorExpression = TranslateConditionExpressionContains(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.DoesNotBeginWith:
            case ConditionOperator.NotLike:
                operatorExpression = Expression.Not(TranslateConditionExpressionLike(condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            case ConditionOperator.DoesNotEndWith:
                operatorExpression = Expression.Not(TranslateConditionExpressionEndsWith(condition, getAttributeValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.DoesNotContain:
                operatorExpression = Expression.Not(TranslateConditionExpressionContains(condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            #endregion

            #region null and not null

            case ConditionOperator.Null:
                operatorExpression = TranslateConditionExpressionNull(getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotNull:
                operatorExpression = Expression.Not(TranslateConditionExpressionNull(getAttributeValueExpr, containsAttributeExpression));
                break;

            #endregion

            #region Greater & Less

            case ConditionOperator.GreaterThan:
                operatorExpression = TranslateConditionExpressionGreaterThan(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.GreaterEqual:
                operatorExpression = TranslateConditionExpressionGreaterThanOrEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.LessThan:
                operatorExpression = TranslateConditionExpressionLessThan(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.LessEqual:
                operatorExpression = TranslateConditionExpressionLessThanOrEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            #endregion

            #region Array Operations

            case ConditionOperator.In:
                operatorExpression = TranslateConditionExpressionIn(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotIn:
                operatorExpression = Expression.Not(TranslateConditionExpressionIn(condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            case ConditionOperator.ContainValues:
                operatorExpression = TranslateConditionExpressionContainValues(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.DoesNotContainValues:
                operatorExpression = Expression.Not(TranslateConditionExpressionContainValues(condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            #endregion

            #region Time Operations

            case ConditionOperator.OnOrAfter:
                operatorExpression = Expression.Or(TranslateConditionExpressionEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression),
                    TranslateConditionExpressionGreaterThan(condition, getAttributeValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.LastXHours:
            case ConditionOperator.LastXDays:
            case ConditionOperator.Last7Days:
            case ConditionOperator.LastXWeeks:
            case ConditionOperator.LastXMonths:
            case ConditionOperator.LastXYears:
                operatorExpression = TranslateConditionExpressionLast(organizationService.TimeProvider, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.OnOrBefore:
                operatorExpression = Expression.Or(TranslateConditionExpressionEqual(organizationService, condition, getAttributeValueExpr, containsAttributeExpression),
                    TranslateConditionExpressionLessThan(condition, getAttributeValueExpr, containsAttributeExpression));
                break;

            case ConditionOperator.Between:
                if (condition.CondExpression.Values.Count != 2)
                {
                    ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "Between operator requires exactly 2 values");
                }

                operatorExpression = TranslateConditionExpressionBetween(condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotBetween:
                if (condition.CondExpression.Values.Count != 2)
                {
                    ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "Not-Between operator requires exactly 2 values");
                }

                operatorExpression = Expression.Not(TranslateConditionExpressionBetween(condition, getAttributeValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.OlderThanXMinutes:
            case ConditionOperator.OlderThanXHours:
            case ConditionOperator.OlderThanXDays:
            case ConditionOperator.OlderThanXWeeks:
            case ConditionOperator.OlderThanXYears:
            case ConditionOperator.OlderThanXMonths:
                operatorExpression = TranslateConditionExpressionOlderThan(organizationService.TimeProvider, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NextXHours:
            case ConditionOperator.NextXDays:
            case ConditionOperator.Next7Days:
            case ConditionOperator.NextXWeeks:
            case ConditionOperator.NextXMonths:
            case ConditionOperator.NextXYears:
                operatorExpression = TranslateConditionExpressionNext(organizationService.TimeProvider, condition, getAttributeValueExpr, containsAttributeExpression);
                break;
            case ConditionOperator.ThisYear:
            case ConditionOperator.LastYear:
            case ConditionOperator.NextYear:
            case ConditionOperator.ThisMonth:
            case ConditionOperator.LastMonth:
            case ConditionOperator.NextMonth:
            case ConditionOperator.LastWeek:
            case ConditionOperator.ThisWeek:
            case ConditionOperator.NextWeek:
            case ConditionOperator.InFiscalYear:
                operatorExpression = TranslateConditionExpressionBetweenDates(organizationService, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            #endregion

            default:
                ErrorFactory.ThrowFault(ErrorCodes.InvalidOperatorCode, $"Operator '{condition.CondExpression.Operator}' is not yet implemented for condition expressions");
                operatorExpression = null!; // unreachable
                break;
        }

        if (condition.IsOuter)
        {
            //If outer join, filter is optional, only if there was a value
            return Expression.Constant(true);
        }

        return operatorExpression;
    }

    private static MethodCallExpression GetCaseInsensitiveExpression(Expression e) => Expression.Call(e, s_stringToLowerInvariant);

    private static MethodCallExpression GetCompareToExpression(Expression left, Expression right) => Expression.Call(left, s_stringCompareTo, right);


    private static object GetSingleConditionValue(TypedConditionExpression c)
    {
        if (c.CondExpression.Values.Count != 1)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument,
                $"The {c.CondExpression.Operator} requires 1 value/s, not {c.CondExpression.Values.Count}.Parameter name: {c.CondExpression.AttributeName}");
        }

        var conditionValue = c.CondExpression.Values.Single();

        if (conditionValue is string || conditionValue is not IEnumerable conditionValueEnumerable) return conditionValue;

        var count = 0;

        foreach (var obj in conditionValueEnumerable)
        {
            count++;
            conditionValue = obj;
        }

        if (count != 1)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, $"The {c.CondExpression.Operator} requires 1 value/s, not {count}.Parameter name: {c.CondExpression.AttributeName}");
        }

        return conditionValue;
    }

    private static MethodCallExpression TransformExpressionGetDateOnlyPart(Expression input) => Expression.Call(input, s_dateTimeGetDate);

    private static Expression TransformExpressionValueBasedOnOperator(ConditionOperator op, Expression input)
    {
        return op switch
        {
            ConditionOperator.Today or ConditionOperator.Yesterday or ConditionOperator.Tomorrow or ConditionOperator.On or ConditionOperator.OnOrAfter or ConditionOperator.OnOrBefore =>
                TransformExpressionGetDateOnlyPart(input),
            _ => input
        };
    }

    private static BinaryExpression TranslateConditionExpressionBetween(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        object value1 = c.Values[0], value2 = c.Values[1];

        //Between the range...
        var exp = Expression.And(
            Expression.GreaterThanOrEqual(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value1), GetAppropriateTypedValueAndType(value1, tc.AttributeType)),
            Expression.LessThanOrEqual(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value2), GetAppropriateTypedValueAndType(value2, tc.AttributeType)));


        //and... attribute exists too
        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), exp));
    }

    /// <summary>
    ///     Takes a condition expression which needs translating into a 'between two dates' expression and works out the relevant dates
    /// </summary>
    private static BinaryExpression TranslateConditionExpressionBetweenDates(FakeOrganizationService organizationService, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        DateTime? fromDate = null;
        DateTime? toDate = null;

        var today = organizationService.TimeProvider.GetLocalNow().Date;
        var thisYear = today.Year;
        var thisMonth = today.Month;


        switch (c.Operator)
        {
            case ConditionOperator.ThisYear: // From first day of this year to last day of this year
                fromDate = new DateTime(thisYear, 1, 1, 0, 0, 0, DateTimeKind.Local);
                toDate = new DateOnly(thisYear, 12, 31).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.LastYear: // From first day of last year to last day of last year
                fromDate = new DateTime(thisYear - 1, 1, 1, 0, 0, 0, DateTimeKind.Local);
                toDate = new DateOnly(thisYear - 1, 12, 31).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.NextYear: // From first day of next year to last day of next year
                fromDate = new DateTime(thisYear + 1, 1, 1, 0, 0, 0, DateTimeKind.Local);
                toDate = new DateOnly(thisYear + 1, 12, 31).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.ThisMonth: // From first day of this month to last day of this month
                fromDate = new DateTime(thisYear, thisMonth, 1, 0, 0, 0, DateTimeKind.Local);
                // Last day of this month: Add one month to the first of this month, and then remove one day
                toDate = DateOnly.FromDateTime(new DateTime(thisYear, thisMonth, 1).AddMonths(1).AddDays(-1)).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.LastMonth: // From first day of last month to last day of last month
                fromDate = new DateTime(thisYear, thisMonth, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(-1);
                // Last day of last month: One day before the first of this month
                toDate = DateOnly.FromDateTime(new DateTime(thisYear, thisMonth, 1).AddDays(-1)).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.NextMonth: // From first day of next month to last day of next month
                fromDate = new DateTime(thisYear, thisMonth, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(1);
                // Last day of Next Month: Add two months to the first of this month, and then go back one day
                toDate = DateOnly.FromDateTime(new DateTime(thisYear, thisMonth, 1).AddMonths(2).AddDays(-1)).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Local);
                break;
            case ConditionOperator.ThisWeek:
                fromDate = today.ToFirstDayOfDeltaWeek();
                toDate = today.ToLastDayOfDeltaWeek().AddDays(1);
                break;
            case ConditionOperator.LastWeek:
                fromDate = today.ToFirstDayOfDeltaWeek(-1);
                toDate = today.ToLastDayOfDeltaWeek(-1).AddDays(1);
                break;
            case ConditionOperator.NextWeek:
                fromDate = today.ToFirstDayOfDeltaWeek(1);
                toDate = today.ToLastDayOfDeltaWeek(1).AddDays(1);
                break;
            case ConditionOperator.InFiscalYear:
                var fiscalYear = (int)c.Values[0];
                c.Values.Clear();
                var fiscalStart = organizationService.Options.FiscalYearStart;
                var fiscalYearDate = fiscalStart.HasValue
                    ? new DateOnly(fiscalYear, fiscalStart.Value.Month, fiscalStart.Value.Day)
                    : new DateOnly(fiscalYear, 1, 1);
                fromDate = fiscalYearDate.ToDateTime(TimeOnly.MinValue);
                toDate = fiscalYearDate.AddYears(1).AddDays(-1).ToDateTime(TimeOnly.MaxValue);
                break;
        }

        c.Values.Add(fromDate);
        c.Values.Add(toDate);

        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }

    private static BinaryExpression TranslateConditionExpressionContains(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        //Append a ´%´at the end of each condition value
        var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x + "%").ToList());
        var computedTypedCondition = new TypedConditionExpression(computedCondition) { AttributeType = tc.AttributeType };

        return TranslateConditionExpressionLike(computedTypedCondition, getAttributeValueExpr, containsAttributeExpr);
    }


    private static BinaryExpression TranslateConditionExpressionContainValues(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, null);
        var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(tc.CondExpression.Values, false));

        return Expression.AndAlso(containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                Expression.Equal(Expression.Call(leftHandSideExpression, s_hashSetIntOverlaps, rightHandSideExpression), Expression.Constant(true))));
    }

    private static BinaryExpression TranslateConditionExpressionEndsWith(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        //Append a ´%´at the end of each condition value
        var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x).ToList());
        var typedComputedCondition = new TypedConditionExpression(computedCondition) { AttributeType = tc.AttributeType };

        return TranslateConditionExpressionLike(typedComputedCondition, getAttributeValueExpr, containsAttributeExpr);
    }

    private static BinaryExpression TranslateConditionExpressionEqual(FakeOrganizationService organizationService, TypedConditionExpression c, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));

        object? unaryOperatorValue = null;

        var today = organizationService.TimeProvider.GetLocalNow().Date;
        unaryOperatorValue = c.CondExpression.Operator switch
        {
            ConditionOperator.Today => today,
            ConditionOperator.Yesterday => today.AddDays(-1),
            ConditionOperator.Tomorrow => today.AddDays(1),
            ConditionOperator.EqualUserId or ConditionOperator.NotEqualUserId => organizationService.Options.UserId,
            ConditionOperator.EqualBusinessId or ConditionOperator.NotEqualBusinessId => organizationService.Options.BusinessUnitId,
            _ => unaryOperatorValue
        };

        if (unaryOperatorValue != null)
        {
            //c.Values empty in this case
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, unaryOperatorValue);
            var transformedExpression = TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Equal(transformedExpression, GetAppropriateTypedValueAndType(unaryOperatorValue, c.AttributeType));
        }

        else if (c.AttributeType == typeof(OptionSetValueCollection))
        {
            var conditionValue = GetSingleConditionValue(c);

            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, conditionValue);
            var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(conditionValue, false));

            expOrValues = Expression.Equal(Expression.Call(leftHandSideExpression, s_hashSetIntSetEquals, rightHandSideExpression), Expression.Constant(true));
        }

        else
        {
            foreach (var value in c.CondExpression.Values)
            {
                var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, value);
                var transformedExpression = TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, leftHandSideExpression);

                expOrValues = Expression.Or(expOrValues,
                    Expression.Equal(transformedExpression, TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, GetAppropriateTypedValueAndType(value, c.AttributeType))));
            }
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression TranslateConditionExpressionGreaterThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        if (c.Values.Count(v => v != null) != 1)
        {
            ThrowFaultException($"The ConditonOperator.{c.Operator} requires 1 value/s, not {c.Values.Count(v => v != null)}. Parameter Name: {c.AttributeName}");
        }

        if (tc.AttributeType == typeof(string) || GetAppropriateTypeForValue(c.Values[0]) == typeof(string))
        {
            return TranslateConditionExpressionGreaterThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Or(expOrValues,
                Expression.GreaterThan(transformedExpression, TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropriateTypedValueAndType(value, tc.AttributeType))));
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression
        TranslateConditionExpressionGreaterThanOrEqual(FakeOrganizationService organizationService, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr) =>
        Expression.Or(TranslateConditionExpressionEqual(organizationService, tc, getAttributeValueExpr, containsAttributeExpr),
            TranslateConditionExpressionGreaterThan(tc, getAttributeValueExpr, containsAttributeExpr));

    private static BinaryExpression TranslateConditionExpressionGreaterThanString(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            var right = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropriateTypedValueAndType(value, tc.AttributeType));

            var methodCallExpr = GetCompareToExpression(transformedExpression, right);

            expOrValues = Expression.Or(expOrValues, Expression.GreaterThan(methodCallExpr, Expression.Constant(0)));
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression TranslateConditionExpressionIn(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));


        if (tc.AttributeType == typeof(OptionSetValueCollection))
        {
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, null);
            var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(c.Values, false));

            expOrValues = Expression.Equal(Expression.Call(leftHandSideExpression, s_hashSetIntSetEquals, rightHandSideExpression), Expression.Constant(true));
        }
        else


        {
            foreach (var value in c.Values)
            {
                if (value is Array array)
                {
                    foreach (var a in array)
                    {
                        expOrValues = Expression.Or(expOrValues,
                            Expression.Equal(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, a), GetAppropriateTypedValueAndType(a, tc.AttributeType)));
                    }
                }
                else
                {
                    expOrValues = Expression.Or(expOrValues,
                        Expression.Equal(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value), GetAppropriateTypedValueAndType(value, tc.AttributeType)));
                }
            }
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression TranslateConditionExpressionLast(TimeProvider timeProvider, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var beforeDateTime = default(DateTime);
        var currentDateTime = timeProvider.GetLocalNow().DateTime;
        beforeDateTime = c.Operator switch
        {
            ConditionOperator.LastXHours => currentDateTime.AddHours(-(int)c.Values[0]),
            ConditionOperator.LastXDays => currentDateTime.AddDays(-(int)c.Values[0]),
            ConditionOperator.Last7Days => currentDateTime.AddDays(-7),
            ConditionOperator.LastXWeeks => currentDateTime.AddDays(-7 * (int)c.Values[0]),
            ConditionOperator.LastXMonths => currentDateTime.AddMonths(-(int)c.Values[0]),
            ConditionOperator.LastXYears => currentDateTime.AddYears(-(int)c.Values[0]),
            _ => beforeDateTime
        };

        c.Values.Clear();
        c.Values.Add(beforeDateTime);
        c.Values.Add(currentDateTime);

        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }


    private static BinaryExpression TranslateConditionExpressionLessThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        if (c.Values.Count(v => v != null) != 1)
        {
            ThrowFaultException($"The ConditonOperator.{c.Operator} requires 1 value/s, not {c.Values.Count(v => v != null)}. Parameter Name: {c.AttributeName}");
        }

        if (tc.AttributeType == typeof(string) || GetAppropriateTypeForValue(c.Values[0]) == typeof(string))
        {
            return TranslateConditionExpressionLessThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Or(expOrValues,
                Expression.LessThan(transformedExpression, TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropriateTypedValueAndType(value, tc.AttributeType))));
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression
        TranslateConditionExpressionLessThanOrEqual(FakeOrganizationService organizationService, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr) =>
        Expression.Or(TranslateConditionExpressionEqual(organizationService, tc, getAttributeValueExpr, containsAttributeExpr),
            TranslateConditionExpressionLessThan(tc, getAttributeValueExpr, containsAttributeExpr));

    private static BinaryExpression TranslateConditionExpressionLessThanString(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedLeftHandSideExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            var rightHandSideExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropriateTypedValueAndType(value, tc.AttributeType));

            //var compareToMethodCall = Expression.Call(transformedLeftHandSideExpression, typeof(string).GetMethod("CompareTo", new Type[] { typeof(string) })!, new[] { rightHandSideExpression });
            var compareToMethodCall = GetCompareToExpression(transformedLeftHandSideExpression, rightHandSideExpression);

            expOrValues = Expression.Or(expOrValues, Expression.LessThan(compareToMethodCall, Expression.Constant(0)));
        }

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), expOrValues));
    }

    private static BinaryExpression TranslateConditionExpressionLike(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        Expression convertedValueToStr = Expression.Convert(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, c.Values[0]), typeof(string));

        var convertedValueToStrAndToLower = GetCaseInsensitiveExpression(convertedValueToStr);

        const string sLikeOperator = "%";
        foreach (var value in c.Values)
        {
            var strValue = value.ToString()!;
            MethodInfo stringMethod;

            if (strValue.EndsWith(sLikeOperator) && strValue.StartsWith(sLikeOperator))
            {
                stringMethod = s_stringContains;
            }

            else if (strValue.StartsWith(sLikeOperator))
            {
                stringMethod = s_stringEndsWith;
            }

            else
            {
                stringMethod = s_stringStartsWith;
            }

            expOrValues = Expression.Or(expOrValues, Expression.Call(convertedValueToStrAndToLower, stringMethod,
                Expression.Constant(strValue.ToLowerInvariant()
                    .Replace("%", "")) //Linq2CRM adds the percentage value to be executed as a LIKE operator, here we are replacing it to just use the Appropriate method
            ));
        }

        return Expression.AndAlso(containsAttributeExpr, expOrValues);
    }

    private static BinaryExpression TranslateConditionExpressionNext(TimeProvider timeProvider, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var nextDateTime = default(DateTime);
        var currentDateTime = timeProvider.GetLocalNow().DateTime;
        nextDateTime = c.Operator switch
        {
            ConditionOperator.NextXHours => currentDateTime.AddHours((int)c.Values[0]),
            ConditionOperator.NextXDays => currentDateTime.AddDays((int)c.Values[0]),
            ConditionOperator.Next7Days => currentDateTime.AddDays(7),
            ConditionOperator.NextXWeeks => currentDateTime.AddDays(7 * (int)c.Values[0]),
            ConditionOperator.NextXMonths => currentDateTime.AddMonths((int)c.Values[0]),
            ConditionOperator.NextXYears => currentDateTime.AddYears((int)c.Values[0]),
            _ => nextDateTime
        };

        c.Values.Clear();
        c.Values.Add(currentDateTime);
        c.Values.Add(nextDateTime);


        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }

    private static BinaryExpression TranslateConditionExpressionNull(Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        return Expression.Or(Expression.AndAlso(containsAttributeExpr, Expression.Equal(getAttributeValueExpr, Expression.Constant(null))), //Attribute is null
            Expression.AndAlso(Expression.Not(containsAttributeExpr), Expression.Constant(true))); //Or attribute is not defined (null)
    }


    private static BinaryExpression TranslateConditionExpressionOlderThan(TimeProvider timeProvider, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        if (!int.TryParse(c.Values[0].ToString(), out var valueToAdd))
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, $"{c.Operator} requires an integer value in the ConditionExpression");
        }

        if (valueToAdd <= 0)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, $"{c.Operator} requires a value greater than 0");
        }

        var toDate = default(DateTime);
        var now = timeProvider.GetLocalNow().DateTime;
        toDate = c.Operator switch
        {
            ConditionOperator.OlderThanXMonths => now.AddMonths(-valueToAdd),
            ConditionOperator.OlderThanXMinutes => now.AddMinutes(-valueToAdd),
            ConditionOperator.OlderThanXHours => now.AddHours(-valueToAdd),
            ConditionOperator.OlderThanXDays => now.AddDays(-valueToAdd),
            ConditionOperator.OlderThanXWeeks => now.AddDays(-7 * valueToAdd),
            ConditionOperator.OlderThanXYears => now.AddYears(-valueToAdd),
            _ => toDate
        };

        return TranslateConditionExpressionOlderThan(tc, getAttributeValueExpr, containsAttributeExpr, toDate);
    }

    private static BinaryExpression TranslateConditionExpressionOlderThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr, DateTime olderThanDate)
    {
        var lessThanExpression = Expression.LessThan(GetAppropriateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, olderThanDate),
            GetAppropriateTypedValueAndType(olderThanDate, tc.AttributeType));

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), lessThanExpression));
    }

    #region GetCastExpression

    private static Expression GetAppropriateTypedValueAndType(object value, Type? attributeType)
    {
        if (attributeType == null)
        {
            return GetAppropriateTypedValue(value);
        }

        if (Nullable.GetUnderlyingType(attributeType) != null)
        {
            attributeType = Nullable.GetUnderlyingType(attributeType)!;
        }

        switch (value)
        {
            //Basic types conversions
            //Special case => datetime is sent as a string
            case string stringValue when attributeType.IsDateTime() //Only convert to DateTime if the attribute's type was DateTime
                                         && DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dtDateTimeConversion):
                return Expression.Constant(dtDateTimeConversion, typeof(DateTime));
            case string stringValue when attributeType.IsOptionSet() && int.TryParse(stringValue, out var iValue):
                return Expression.Constant(iValue, typeof(int));
            case string stringValue when (attributeType == typeof(EntityReference) || attributeType == typeof(Guid)) && Guid.TryParse(stringValue, out var id):
                return Expression.Constant(id);
            case string:
                return GetCaseInsensitiveExpression(Expression.Constant(value, typeof(string)));
            case EntityReference reference:
                {
                    var cast = reference.Id;
                    return Expression.Constant(cast);
                }
            case OptionSetValue optionSetValue:
                {
                    var cast = optionSetValue.Value;
                    return Expression.Constant(cast);
                }
            case Money money:
                {
                    var cast = money.Value;
                    return Expression.Constant(cast);
                }
            default:
                return Expression.Constant(value);
        }
    }


    private static Type GetAppropriateTypeForValue(object value)
    {
        //Basic types conversions
        //Special case => datetime is sent as a string
        if (value is string)
        {
            return DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out _) ? typeof(DateTime) : typeof(string);
        }

        return value.GetType();
    }

    private static Expression GetAppropriateTypedValue(object value)
    {
        switch (value)
        {
            //Basic types conversions
            //Special case => datetime is sent as a string
            case string stringValue when DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dtDateTimeConversion):
                return Expression.Constant(dtDateTimeConversion, typeof(DateTime));
            case string:
                return GetCaseInsensitiveExpression(Expression.Constant(value, typeof(string)));
            case EntityReference reference:
                {
                    var cast = reference.Id;
                    return Expression.Constant(cast);
                }
            case OptionSetValue optionSetValue:
                {
                    var cast = optionSetValue.Value;
                    return Expression.Constant(cast);
                }
            case Money money:
                {
                    var cast = money.Value;
                    return Expression.Constant(cast);
                }
            default:
                return Expression.Constant(value);
        }
    }

    private static ConditionalExpression GetAppropriateCastExpressionBasedOnType(Type? t, Expression input, object? value)
    {
        var typedExpression = GetAppropriateCastExpressionBasedOnAttributeTypeOrValue(input, value, t);

        //Now, any value (entity reference, string, int, etc,... could be wrapped in an AliasedValue object
        //So let's add this
        var getValueFromAliasedValueExp = Expression.Call(Expression.Convert(input, typeof(AliasedValue)), s_aliasedValueGetValue);

        var exp = Expression.Condition(Expression.TypeIs(input, typeof(AliasedValue)), GetAppropriateCastExpressionBasedOnAttributeTypeOrValue(getValueFromAliasedValueExp, value, t),
            typedExpression //Not an aliased value
        );

        return exp;
    }


    private static Expression GetAppropriateCastExpressionBasedOnAttributeTypeOrValue(Expression input, object? value, Type? attributeType)
    {
        if (attributeType == null)
        {
            return GetAppropriateCastExpressionBasedOnValueInherentType(input, value); //Dynamic entities
        }

        if (Nullable.GetUnderlyingType(attributeType) != null)
        {
            attributeType = Nullable.GetUnderlyingType(attributeType)!;
        }

        if (attributeType == typeof(Guid))
        {
            return GetAppropriateCastExpressionBasedGuid(input);
        }

        if (attributeType == typeof(EntityReference))
        {
            return GetAppropriateCastExpressionBasedOnEntityReference(input, value!);
        }

        if (attributeType == typeof(int) || attributeType == typeof(int?) || attributeType.IsOptionSet())
        {
            return GetAppropriateCastExpressionBasedOnInt(input);
        }

        if (attributeType == typeof(decimal) || attributeType == typeof(Money))
        {
            return GetAppropriateCastExpressionBasedOnDecimal(input);
        }

        if (attributeType == typeof(bool) || attributeType == typeof(BooleanManagedProperty))
        {
            return GetAppropriateCastExpressionBasedOnBoolean(input);
        }

        if (attributeType == typeof(string))
        {
            return GetAppropriateCastExpressionBasedOnStringAndType(input, value, attributeType);
        }

        if (attributeType.IsDateTime())
        {
            return GetAppropriateCastExpressionBasedOnDateTime(input, value);
        }

        if (attributeType.IsOptionSetValueCollection())
        {
            return GetAppropriateCastExpressionBasedOnOptionSetValueCollection(input);
        }


        return GetAppropriateCastExpressionDefault(input, value); //any other type

    }

    private static Expression GetAppropriateCastExpressionBasedOnValueInherentType(Expression input, object? value)
    {
        return value switch
        {
            Guid or EntityReference => GetAppropriateCastExpressionBasedGuid(input),
            int or OptionSetValue => GetAppropriateCastExpressionBasedOnInt(input),
            decimal or Money => GetAppropriateCastExpressionBasedOnDecimal(input),
            bool => GetAppropriateCastExpressionBasedOnBoolean(input),
            string => GetAppropriateCastExpressionBasedOnString(input, value),
            _ => GetAppropriateCastExpressionDefault(input, value)
        };
    }


    private static Expression GetAppropriateCastExpressionBasedOnString(Expression input, object? value)
    {
        var defaultStringExpression = GetCaseInsensitiveExpression(GetAppropriateCastExpressionDefault(input, value));

        if (DateTime.TryParse(value?.ToString(), out _))
        {
            return Expression.Convert(input, typeof(DateTime));
        }

        if (int.TryParse(value?.ToString(), out _))
        {
            return Expression.Condition(Expression.TypeIs(input, typeof(OptionSetValue)), GetToStringExpression(GetAppropriateCastExpressionBasedOnInt(input)), defaultStringExpression);
        }

        return defaultStringExpression;
    }

    private static Expression GetAppropriateCastExpressionBasedOnStringAndType(Expression input, object? value, Type? attributeType)
    {
        var defaultStringExpression = GetCaseInsensitiveExpression(GetAppropriateCastExpressionDefault(input, value));

        if (attributeType?.IsOptionSet() == true && int.TryParse(value?.ToString(), out _))
        {
            return Expression.Condition(Expression.TypeIs(input, typeof(OptionSetValue)), GetToStringExpression(GetAppropriateCastExpressionBasedOnInt(input)), defaultStringExpression);
        }

        return defaultStringExpression;
    }

    private static MethodCallExpression GetToStringExpression(Expression e) => Expression.Call(e, s_intToString);

    private static Expression GetAppropriateCastExpressionBasedOnDateTime(Expression input, object? value)
    {
        // Convert to DateTime if string
        if (value is DateTime || (value is string && DateTime.TryParse(value.ToString(), out _)))
        {
            return Expression.Convert(input, typeof(DateTime));
        }

        return input; // return directly
    }

    private static UnaryExpression GetAppropriateCastExpressionDefault(Expression input, object? value) => Expression.Convert(input, value!.GetType()); //Default type conversion

    private static ConditionalExpression GetAppropriateCastExpressionBasedGuid(Expression input)
    {
        var getIdFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)), s_entityReferenceGetId);

        return Expression.Condition(Expression.TypeIs(input, typeof(EntityReference)), //If input is an entity reference, compare the Guid against the Id property
            Expression.Convert(getIdFromEntityReferenceExpr, typeof(Guid)), Expression.Condition(Expression.TypeIs(input, typeof(Guid)), //If any other case, then just compare it as a Guid directly
                Expression.Convert(input, typeof(Guid)), Expression.Constant(Guid.Empty, typeof(Guid))));
    }

    private static Expression GetAppropriateCastExpressionBasedOnEntityReference(Expression input, object value)
    {
        if (value is string strValue && !Guid.TryParse(strValue, out _))
        {
            var getNameFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)), s_entityReferenceGetName);

            return GetCaseInsensitiveExpression(Expression.Condition(Expression.TypeIs(input, typeof(EntityReference)), Expression.Convert(getNameFromEntityReferenceExpr, typeof(string)),
                Expression.Constant(string.Empty, typeof(string))));
        }

        var getIdFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)), s_entityReferenceGetId);

        return Expression.Condition(Expression.TypeIs(input, typeof(EntityReference)), //If input is an entity reference, compare the Guid against the Id property
            Expression.Convert(getIdFromEntityReferenceExpr, typeof(Guid)), Expression.Condition(Expression.TypeIs(input, typeof(Guid)), //If any other case, then just compare it as a Guid directly
                Expression.Convert(input, typeof(Guid)), Expression.Constant(Guid.Empty, typeof(Guid))));
    }

    private static ConditionalExpression GetAppropriateCastExpressionBasedOnDecimal(Expression input) =>
        Expression.Condition(Expression.TypeIs(input, typeof(Money)),
            Expression.Convert(Expression.Call(Expression.TypeAs(input, typeof(Money)), s_moneyGetValue), typeof(decimal)),
            Expression.Condition(Expression.TypeIs(input, typeof(decimal)), Expression.Convert(input, typeof(decimal)), Expression.Constant(0.0M)));

    private static ConditionalExpression GetAppropriateCastExpressionBasedOnBoolean(Expression input) =>
        Expression.Condition(Expression.TypeIs(input, typeof(BooleanManagedProperty)),
            Expression.Convert(Expression.Call(Expression.TypeAs(input, typeof(BooleanManagedProperty)), s_booleanManagedPropertyGetValue), typeof(bool)),
            Expression.Condition(Expression.TypeIs(input, typeof(bool)), Expression.Convert(input, typeof(bool)), Expression.Constant(false)));

    private static ConditionalExpression GetAppropriateCastExpressionBasedOnInt(Expression input) =>
        Expression.Condition(Expression.TypeIs(input, typeof(OptionSetValue)),
            Expression.Convert(Expression.Call(Expression.TypeAs(input, typeof(OptionSetValue)), s_optionSetValueGetValue), typeof(int)), Expression.Convert(input, typeof(int)));

    private static MethodCallExpression GetAppropriateCastExpressionBasedOnOptionSetValueCollection(Expression input) =>
        Expression.Call(s_convertToHashSetOfIntMethod, input, Expression.Constant(true));

    #endregion
}
