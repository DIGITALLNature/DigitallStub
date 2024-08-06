// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using Digitall.Stub.Errors;
using DotNetEnv;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries;

public static class ConditionParser
{
    public static HashSet<int> ConvertToHashSetOfInt(object input, bool isOptionSetValueCollectionAccepted)
    {
        var set = new HashSet<int>();

        var faultReason = $"The formatter threw an exception while trying to deserialize the message: There was an error while trying to deserialize parameter" +
                          $" http://schemas.microsoft.com/xrm/2011/Contracts/Services:query. The InnerException message was 'Error in line 1 position 8295. Element " +
                          $"'http://schemas.microsoft.com/2003/10/Serialization/Arrays:anyType' contains data from a type that maps to the name " +
                          $"'http://schemas.microsoft.com/xrm/2011/Contracts:{input?.GetType()}'. The deserializer has no knowledge of any type that maps to this name. " +
                          $"Consider changing the implementation of the ResolveName method on your DataContractResolver to return a non-null value for name " +
                          $"'{input?.GetType()}' and namespace 'http://schemas.microsoft.com/xrm/2011/Contracts'.'.  Please see InnerException for more details.";

        if (input is int)
        {
            set.Add((int)input);
        }
        else if (input is string)
        {
            set.Add(int.Parse(input as string));
        }
        else if (input is int[])
        {
            set.UnionWith(input as int[]);
        }
        else if (input is string[])
        {
            set.UnionWith((input as string[]).Select(s => int.Parse(s)));
        }
        else if (input is DataCollection<object>)
        {
            var collection = input as DataCollection<object>;

            if (collection.All(o => o is int))
            {
                set.UnionWith(collection.Cast<int>());
            }
            else if (collection.All(o => o is string))
            {
                set.UnionWith(collection.Select(o => int.Parse(o as string)));
            }
            else if (collection.Count == 1 && collection[0] is int[])
            {
                set.UnionWith(collection[0] as int[]);
            }
            else if (collection.Count == 1 && collection[0] is string[])
            {
                set.UnionWith((collection[0] as string[]).Select(s => int.Parse(s)));
            }
            else
            {
                throw new FaultException(new FaultReason(faultReason));
            }
        }
        else if (isOptionSetValueCollectionAccepted && input is OptionSetValueCollection)
        {
            set.UnionWith((input as OptionSetValueCollection).Select(osv => osv.Value));
        }
        else
        {
            throw new FaultException(new FaultReason(faultReason));
        }

        return set;
    }

    public static Expression TranslateConditionExpression(QueryExpression queryExpression, DataverseStub context, TypedConditionExpression condition, ParameterExpression entity)
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

        Expression containsAttributeExpression = Expression.Call(attributesProperty, typeof(AttributeCollection).GetMethod(nameof(AttributeCollection.ContainsKey) , new[] { typeof(string) }), Expression.Constant(attributeName)
        );

        Expression getAttributeValueExpr = Expression.Property(
            attributesProperty, "Item",
            Expression.Constant(attributeName, typeof(string))
        );


        var getNonBasicValueExpr = getAttributeValueExpr;

        Expression operatorExpression = null;

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
                operatorExpression = TranslateConditionExpressionEqual(context.Clock, condition, getAttributeValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotOn:
            case ConditionOperator.NotEqual:
            case ConditionOperator.NotEqualUserId:
            case ConditionOperator.NotEqualBusinessId:
                operatorExpression = Expression.Not(TranslateConditionExpressionEqual(context.Clock, condition, getAttributeValueExpr, containsAttributeExpression));
                break;
#endregion

#region like and not like
            case ConditionOperator.BeginsWith:
            case ConditionOperator.Like:
                operatorExpression = TranslateConditionExpressionLike(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.EndsWith:
                operatorExpression = TranslateConditionExpressionEndsWith(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.Contains:
                operatorExpression = TranslateConditionExpressionContains(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.DoesNotBeginWith:
            case ConditionOperator.DoesNotEndWith:
            case ConditionOperator.NotLike:
            case ConditionOperator.DoesNotContain:
                operatorExpression = Expression.Not(TranslateConditionExpressionLike(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;
#endregion

            case ConditionOperator.Null:
                operatorExpression = TranslateConditionExpressionNull(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotNull:
                operatorExpression = Expression.Not(TranslateConditionExpressionNull(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;

            case ConditionOperator.GreaterThan:
                operatorExpression = TranslateConditionExpressionGreaterThan(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.GreaterEqual:
                operatorExpression = TranslateConditionExpressionGreaterThanOrEqual(context, condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.LessThan:
                operatorExpression = TranslateConditionExpressionLessThan(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.LessEqual:
                operatorExpression = TranslateConditionExpressionLessThanOrEqual(context, condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.In:
                operatorExpression = TranslateConditionExpressionIn(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotIn:
                operatorExpression = Expression.Not(TranslateConditionExpressionIn(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;


            case ConditionOperator.OnOrAfter:
                operatorExpression = Expression.Or(
                    TranslateConditionExpressionEqual(context.Clock, condition, getNonBasicValueExpr, containsAttributeExpression),
                    TranslateConditionExpressionGreaterThan(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.LastXHours:
            case ConditionOperator.LastXDays:
            case ConditionOperator.Last7Days:
            case ConditionOperator.LastXWeeks:
            case ConditionOperator.LastXMonths:
            case ConditionOperator.LastXYears:
                operatorExpression = TranslateConditionExpressionLast(context.Clock, condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.OnOrBefore:
                operatorExpression = Expression.Or(
                    TranslateConditionExpressionEqual(context.Clock, condition, getNonBasicValueExpr, containsAttributeExpression),
                    TranslateConditionExpressionLessThan(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;

            case ConditionOperator.Between:
                if (condition.CondExpression.Values.Count != 2)
                {
                    throw new Exception("Between operator requires exactly 2 values.");
                }

                operatorExpression = TranslateConditionExpressionBetween(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NotBetween:
                if (condition.CondExpression.Values.Count != 2)
                {
                    throw new Exception("Not-Between operator requires exactly 2 values.");
                }

                operatorExpression = Expression.Not(TranslateConditionExpressionBetween(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.OlderThanXMinutes:
            case ConditionOperator.OlderThanXHours:
            case ConditionOperator.OlderThanXDays:
            case ConditionOperator.OlderThanXWeeks:
            case ConditionOperator.OlderThanXYears:
            case ConditionOperator.OlderThanXMonths:
                operatorExpression = TranslateConditionExpressionOlderThan(context.Clock,condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NextXHours:
            case ConditionOperator.NextXDays:
            case ConditionOperator.Next7Days:
            case ConditionOperator.NextXWeeks:
            case ConditionOperator.NextXMonths:
            case ConditionOperator.NextXYears:
                operatorExpression = TranslateConditionExpressionNext(context.Clock,condition, getNonBasicValueExpr, containsAttributeExpression);
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
                operatorExpression = TranslateConditionExpressionBetweenDates(context.Clock,condition, getNonBasicValueExpr, containsAttributeExpression, context);
                break;

            case ConditionOperator.ContainValues:
                operatorExpression = TranslateConditionExpressionContainValues(condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.DoesNotContainValues:
                operatorExpression = Expression.Not(TranslateConditionExpressionContainValues(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;


            default:
                throw new ArgumentOutOfRangeException($"Operator {condition.CondExpression.Operator.ToString()} not yet implemented for condition expression");
        }

        if (condition.IsOuter)
        {
            //If outer join, filter is optional, only if there was a value
            return Expression.Constant(true);
        }

        return operatorExpression;
    }

    private static Expression GetCaseInsensitiveExpression(Expression e) =>
        Expression.Call(e,
            typeof(string).GetMethod("ToLowerInvariant", new Type[] { }));

    private static Expression GetCompareToExpression<T>(Expression left, Expression right) => Expression.Call(left, typeof(T).GetMethod("CompareTo", new[] { typeof(string) }), right);


    private static object GetSingleConditionValue(TypedConditionExpression c)
    {
        if (c.CondExpression.Values.Count != 1)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument,
                $"The {c.CondExpression.Operator} requires 1 value/s, not {c.CondExpression.Values.Count}.Parameter name: {c.CondExpression.AttributeName}");
        }

        var conditionValue = c.CondExpression.Values.Single();

        if (!(conditionValue is string) && conditionValue is IEnumerable)
        {
            var conditionValueEnumerable = conditionValue as IEnumerable;
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
        }

        return conditionValue;
    }

    private static Expression TransformExpressionGetDateOnlyPart(Expression input) => Expression.Call(input, typeof(DateTime).GetMethod("get_Date"));

    private static Expression TransformExpressionValueBasedOnOperator(ConditionOperator op, Expression input)
    {
        switch (op)
        {
            case ConditionOperator.Today:
            case ConditionOperator.Yesterday:
            case ConditionOperator.Tomorrow:
            case ConditionOperator.On:
            case ConditionOperator.OnOrAfter:
            case ConditionOperator.OnOrBefore:
                return TransformExpressionGetDateOnlyPart(input);

            default:
                return input; //No transformation
        }
    }

    private static Expression TranslateConditionExpressionBetween(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        object value1, value2;
        value1 = c.Values[0];
        value2 = c.Values[1];

        //Between the range...
        var exp = Expression.And(
            Expression.GreaterThanOrEqual(
                GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value1),
                GetAppropiateTypedValueAndType(value1, tc.AttributeType)),
            Expression.LessThanOrEqual(
                GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value2),
                GetAppropiateTypedValueAndType(value2, tc.AttributeType)));


        //and... attribute exists too
        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                exp));
    }

    /// <summary>
    ///     Takes a condition expression which needs translating into a 'between two dates' expression and works out the relevant dates
    /// </summary>
    private static Expression TranslateConditionExpressionBetweenDates(IStubClock clock, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr, DataverseStub context)
    {
        var c = tc.CondExpression;

        DateTime? fromDate = null;
        DateTime? toDate = null;

        var today =clock.Today;
        var thisYear = today.Year;
        var thisMonth = today.Month;


        switch (c.Operator)
        {
            case ConditionOperator.ThisYear: // From first day of this year to last day of this year
                fromDate = new DateTime(thisYear, 1, 1);
                toDate = new DateTime(thisYear, 12, 31);
                break;
            case ConditionOperator.LastYear: // From first day of last year to last day of last year
                fromDate = new DateTime(thisYear - 1, 1, 1);
                toDate = new DateTime(thisYear - 1, 12, 31);
                break;
            case ConditionOperator.NextYear: // From first day of next year to last day of next year
                fromDate = new DateTime(thisYear + 1, 1, 1);
                toDate = new DateTime(thisYear + 1, 12, 31);
                break;
            case ConditionOperator.ThisMonth: // From first day of this month to last day of this month
                fromDate = new DateTime(thisYear, thisMonth, 1);
                // Last day of this month: Add one month to the first of this month, and then remove one day
                toDate = new DateTime(thisYear, thisMonth, 1).AddMonths(1).AddDays(-1);
                break;
            case ConditionOperator.LastMonth: // From first day of last month to last day of last month
                fromDate = new DateTime(thisYear, thisMonth, 1).AddMonths(-1);
                // Last day of last month: One day before the first of this month
                toDate = new DateTime(thisYear, thisMonth, 1).AddDays(-1);
                break;
            case ConditionOperator.NextMonth: // From first day of next month to last day of next month
                fromDate = new DateTime(thisYear, thisMonth, 1).AddMonths(1);
                // LAst day of Next Month: Add two months to the first of this month, and then go back one day
                toDate = new DateTime(thisYear, thisMonth, 1).AddMonths(2).AddDays(-1);
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
                var fiscalYearDate = DateTime.Parse(Env.GetString("FiscalYearStart", $"{fiscalYear}-01-01"));
                fromDate = fiscalYearDate;
                toDate = fiscalYearDate.AddYears(1).AddDays(-1);
                break;
        }

        c.Values.Add(fromDate);
        c.Values.Add(toDate);

        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }

    private static Expression TranslateConditionExpressionContains(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        //Append a ´%´at the end of each condition value
        var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x + "%").ToList());
        var computedTypedCondition = new TypedConditionExpression(computedCondition);
        computedTypedCondition.AttributeType = tc.AttributeType;

        return TranslateConditionExpressionLike(computedTypedCondition, getAttributeValueExpr, containsAttributeExpr);
    }


    private static Expression TranslateConditionExpressionContainValues(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, null);
        var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(tc.CondExpression.Values, false));

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(
                Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                Expression.Equal(
                    Expression.Call(leftHandSideExpression, typeof(HashSet<int>).GetMethod("Overlaps"), rightHandSideExpression),
                    Expression.Constant(true))));
    }

    private static Expression TranslateConditionExpressionEndsWith(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        //Append a ´%´at the end of each condition value
        var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x).ToList());
        var typedComputedCondition = new TypedConditionExpression(computedCondition);
        typedComputedCondition.AttributeType = tc.AttributeType;

        return TranslateConditionExpressionLike(typedComputedCondition, getAttributeValueExpr, containsAttributeExpr);
    }

    private static Expression TranslateConditionExpressionEqual(IStubClock clock, TypedConditionExpression c, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));

        object unaryOperatorValue = null;

        switch (c.CondExpression.Operator)
        {
            case ConditionOperator.Today:
                unaryOperatorValue = clock.Today;
                break;
            case ConditionOperator.Yesterday:
                unaryOperatorValue = clock.Today.AddDays(-1);
                break;
            case ConditionOperator.Tomorrow:
                unaryOperatorValue = clock.Today.AddDays(1);
                break;
            case ConditionOperator.EqualUserId:
            case ConditionOperator.NotEqualUserId:
                unaryOperatorValue = Guid.Parse(Env.GetString("CallerId", Guid.Empty.ToString()));
                break;

            case ConditionOperator.EqualBusinessId:
            case ConditionOperator.NotEqualBusinessId:
                unaryOperatorValue = Guid.Parse(Env.GetString("BusinessUnitId", Guid.Empty.ToString()));
                break;
        }

        if (unaryOperatorValue != null)
        {
            //c.Values empty in this case
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, unaryOperatorValue);
            var transformedExpression = TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Equal(transformedExpression,
                GetAppropiateTypedValueAndType(unaryOperatorValue, c.AttributeType));
        }

        else if (c.AttributeType == typeof(OptionSetValueCollection))
        {
            var conditionValue = GetSingleConditionValue(c);

            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, conditionValue);
            var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(conditionValue, false));

            expOrValues = Expression.Equal(
                Expression.Call(leftHandSideExpression, typeof(HashSet<int>).GetMethod(nameof(HashSet<int>.SetEquals)), rightHandSideExpression),
                Expression.Constant(true));
        }

        else
        {
            foreach (var value in c.CondExpression.Values)
            {
                var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(c.AttributeType, getAttributeValueExpr, value);
                var transformedExpression = TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, leftHandSideExpression);

                expOrValues = Expression.Or(expOrValues, Expression.Equal(
                    transformedExpression,
                    TransformExpressionValueBasedOnOperator(c.CondExpression.Operator, GetAppropiateTypedValueAndType(value, c.AttributeType))));
            }
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionGreaterThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        if (c.Values.Count(v => v != null) != 1)
        {
            throw new FaultException(new FaultReason($"The ConditonOperator.{c.Operator} requires 1 value/s, not {c.Values.Count(v => v != null)}. Parameter Name: {c.AttributeName}"));
        }

        if (tc.AttributeType == typeof(string))
        {
            return TranslateConditionExpressionGreaterThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        if (GetAppropiateTypeForValue(c.Values[0]) == typeof(string))
        {
            return TranslateConditionExpressionGreaterThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Or(expOrValues,
                Expression.GreaterThan(
                    transformedExpression,
                    TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropiateTypedValueAndType(value, tc.AttributeType))));
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionGreaterThanOrEqual(DataverseStub context, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr) =>
        Expression.Or(
            TranslateConditionExpressionEqual(context.Clock, tc, getAttributeValueExpr, containsAttributeExpr),
            TranslateConditionExpressionGreaterThan(tc, getAttributeValueExpr, containsAttributeExpr));

    private static Expression TranslateConditionExpressionGreaterThanString(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            var left = transformedExpression;
            var right = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropiateTypedValueAndType(value, tc.AttributeType));

            var methodCallExpr = GetCompareToExpression<string>(left, right);

            expOrValues = Expression.Or(expOrValues,
                Expression.GreaterThan(
                    methodCallExpr,
                    Expression.Constant(0)));
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionIn(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));


        if (tc.AttributeType == typeof(OptionSetValueCollection))
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, null);
            var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(c.Values, false));

            expOrValues = Expression.Equal(
                Expression.Call(leftHandSideExpression, typeof(HashSet<int>).GetMethod(nameof(HashSet<int>.SetEquals)), rightHandSideExpression),
                Expression.Constant(true));
        }
        else


        {
            foreach (var value in c.Values)
            {
                if (value is Array)
                {
                    foreach (var a in (Array)value)
                    {
                        expOrValues = Expression.Or(expOrValues, Expression.Equal(
                            GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, a),
                            GetAppropiateTypedValueAndType(a, tc.AttributeType)));
                    }
                }
                else
                {
                    expOrValues = Expression.Or(expOrValues, Expression.Equal(
                        GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value),
                        GetAppropiateTypedValueAndType(value, tc.AttributeType)));
                }
            }
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionLast(IStubClock clock, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var beforeDateTime = default(DateTime);
        var currentDateTime = clock.Now;
        switch (c.Operator)
        {
            case ConditionOperator.LastXHours:
                beforeDateTime = currentDateTime.AddHours(-(int)c.Values[0]);
                break;
            case ConditionOperator.LastXDays:
                beforeDateTime = currentDateTime.AddDays(-(int)c.Values[0]);
                break;
            case ConditionOperator.Last7Days:
                beforeDateTime = currentDateTime.AddDays(-7);
                break;
            case ConditionOperator.LastXWeeks:
                beforeDateTime = currentDateTime.AddDays(-7 * (int)c.Values[0]);
                break;
            case ConditionOperator.LastXMonths:
                beforeDateTime = currentDateTime.AddMonths(-(int)c.Values[0]);
                break;
            case ConditionOperator.LastXYears:
                beforeDateTime = currentDateTime.AddYears(-(int)c.Values[0]);
                break;
        }

        c.Values.Clear();
        c.Values.Add(beforeDateTime);
        c.Values.Add(currentDateTime);

        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }


    private static Expression TranslateConditionExpressionLessThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        if (c.Values.Count(v => v != null) != 1)
        {
            throw new FaultException(new FaultReason($"The ConditonOperator.{c.Operator} requires 1 value/s, not {c.Values.Count(v => v != null)}. Parameter Name: {c.AttributeName}"));
        }

        if (tc.AttributeType == typeof(string))
        {
            return TranslateConditionExpressionLessThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        if (GetAppropiateTypeForValue(c.Values[0]) == typeof(string))
        {
            return TranslateConditionExpressionLessThanString(tc, getAttributeValueExpr, containsAttributeExpr);
        }

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            expOrValues = Expression.Or(expOrValues,
                Expression.LessThan(
                    transformedExpression,
                    TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropiateTypedValueAndType(value, tc.AttributeType))));
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionLessThanOrEqual(DataverseStub context, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr) =>
        Expression.Or(
            TranslateConditionExpressionEqual(context.Clock, tc, getAttributeValueExpr, containsAttributeExpr),
            TranslateConditionExpressionLessThan(tc, getAttributeValueExpr, containsAttributeExpr));

    private static Expression TranslateConditionExpressionLessThanString(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        foreach (var value in c.Values)
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, value);
            var transformedLeftHandSideExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, leftHandSideExpression);

            var rightHandSideExpression = TransformExpressionValueBasedOnOperator(tc.CondExpression.Operator, GetAppropiateTypedValueAndType(value, tc.AttributeType));

            //var compareToMethodCall = Expression.Call(transformedLeftHandSideExpression, typeof(string).GetMethod("CompareTo", new Type[] { typeof(string) }), new[] { rightHandSideExpression });
            var compareToMethodCall = GetCompareToExpression<string>(transformedLeftHandSideExpression, rightHandSideExpression);

            expOrValues = Expression.Or(expOrValues,
                Expression.LessThan(compareToMethodCall, Expression.Constant(0)));
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                expOrValues));
    }

    private static Expression TranslateConditionExpressionLike(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
        Expression convertedValueToStr = Expression.Convert(GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, c.Values[0]), typeof(string));

        var convertedValueToStrAndToLower = GetCaseInsensitiveExpression(convertedValueToStr);

        var sLikeOperator = "%";
        foreach (var value in c.Values)
        {
            var strValue = value.ToString();
            var sMethod = "";

            if (strValue.EndsWith(sLikeOperator) && strValue.StartsWith(sLikeOperator))
            {
                sMethod = "Contains";
            }

            else if (strValue.StartsWith(sLikeOperator))
            {
                sMethod = "EndsWith";
            }

            else
            {
                sMethod = "StartsWith";
            }

            expOrValues = Expression.Or(expOrValues, Expression.Call(
                convertedValueToStrAndToLower,
                typeof(string).GetMethod(sMethod, new[] { typeof(string) }),
                Expression.Constant(value.ToString().ToLowerInvariant()
                    .Replace("%", "")) //Linq2CRM adds the percentage value to be executed as a LIKE operator, here we are replacing it to just use the appropiate method
            ));
        }

        return Expression.AndAlso(
            containsAttributeExpr,
            expOrValues);
    }

    private static Expression TranslateConditionExpressionNext(IStubClock clock, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var nextDateTime = default(DateTime);
        var currentDateTime = clock.Now;
        switch (c.Operator)
        {
            case ConditionOperator.NextXHours:
                nextDateTime = currentDateTime.AddHours((int)c.Values[0]);
                break;
            case ConditionOperator.NextXDays:
                nextDateTime = currentDateTime.AddDays((int)c.Values[0]);
                break;
            case ConditionOperator.Next7Days:
                nextDateTime = currentDateTime.AddDays(7);
                break;
            case ConditionOperator.NextXWeeks:
                nextDateTime = currentDateTime.AddDays(7 * (int)c.Values[0]);
                break;
            case ConditionOperator.NextXMonths:
                nextDateTime = currentDateTime.AddMonths((int)c.Values[0]);
                break;
            case ConditionOperator.NextXYears:
                nextDateTime = currentDateTime.AddYears((int)c.Values[0]);
                break;
        }

        c.Values.Clear();
        c.Values.Add(currentDateTime);
        c.Values.Add(nextDateTime);


        return TranslateConditionExpressionBetween(tc, getAttributeValueExpr, containsAttributeExpr);
    }

    private static Expression TranslateConditionExpressionNull(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        return Expression.Or(Expression.AndAlso(
                containsAttributeExpr,
                Expression.Equal(
                    getAttributeValueExpr,
                    Expression.Constant(null))), //Attribute is null
            Expression.AndAlso(
                Expression.Not(containsAttributeExpr),
                Expression.Constant(true))); //Or attribute is not defined (null)
    }


    private static Expression TranslateConditionExpressionOlderThan(IStubClock clock, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
    {
        var c = tc.CondExpression;

        var valueToAdd = 0;

        if (!int.TryParse(c.Values[0].ToString(), out valueToAdd))
        {
            throw new Exception(c.Operator + " requires an integer value in the ConditionExpression.");
        }

        if (valueToAdd <= 0)
        {
            throw new Exception(c.Operator + " requires a value greater than 0.");
        }

        var toDate = default(DateTime);

        switch (c.Operator)
        {
            case ConditionOperator.OlderThanXMonths:
                toDate = clock.Now.AddMonths(-valueToAdd);
                break;
            case ConditionOperator.OlderThanXMinutes:
                toDate = clock.Now.AddMinutes(-valueToAdd);
                break;
            case ConditionOperator.OlderThanXHours:
                toDate = clock.Now.AddHours(-valueToAdd);
                break;
            case ConditionOperator.OlderThanXDays:
                toDate = clock.Now.AddDays(-valueToAdd);
                break;
            case ConditionOperator.OlderThanXWeeks:
                toDate = clock.Now.AddDays(-7 * valueToAdd);
                break;
            case ConditionOperator.OlderThanXYears:
                toDate = clock.Now.AddYears(-valueToAdd);
                break;
        }

        return TranslateConditionExpressionOlderThan(tc, getAttributeValueExpr, containsAttributeExpr, toDate);
    }

    private static Expression TranslateConditionExpressionOlderThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr, DateTime olderThanDate)
    {
        var lessThanExpression = Expression.LessThan(
            GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, olderThanDate),
            GetAppropiateTypedValueAndType(olderThanDate, tc.AttributeType));

        return Expression.AndAlso(containsAttributeExpr, Expression.AndAlso(Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)), lessThanExpression));
    }

    #region GetCastExpression

    private static Expression GetAppropiateTypedValueAndType(object value, Type attributeType)
    {
        if (attributeType == null)
        {
            return GetAppropiateTypedValue(value);
        }

        if (Nullable.GetUnderlyingType(attributeType) != null)
        {
            attributeType = Nullable.GetUnderlyingType(attributeType);
        }

        //Basic types conversions
        //Special case => datetime is sent as a string
        if (value is string)
        {
            int iValue;

            DateTime dtDateTimeConversion;
            Guid id;
            if (attributeType.IsDateTime() //Only convert to DateTime if the attribute's type was DateTime
                && DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dtDateTimeConversion))
            {
                return Expression.Constant(dtDateTimeConversion, typeof(DateTime));
            }

            if (attributeType.IsOptionSet() && int.TryParse(value.ToString(), out iValue))
            {
                return Expression.Constant(iValue, typeof(int));
            }

            if ((attributeType == typeof(EntityReference) || attributeType == typeof(Guid)) && Guid.TryParse((string)value, out id))
            {
                return Expression.Constant(id);
            }

            return GetCaseInsensitiveExpression(Expression.Constant(value, typeof(string)));
        }

        if (value is EntityReference)
        {
            var cast = (value as EntityReference).Id;
            return Expression.Constant(cast);
        }

        if (value is OptionSetValue)
        {
            var cast = (value as OptionSetValue).Value;
            return Expression.Constant(cast);
        }

        if (value is Money)
        {
            var cast = (value as Money).Value;
            return Expression.Constant(cast);
        }

        return Expression.Constant(value);
    }


    private static Type GetAppropiateTypeForValue(object value)
    {
        //Basic types conversions
        //Special case => datetime is sent as a string
        if (value is string)
        {
            DateTime dtDateTimeConversion;
            if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dtDateTimeConversion))
            {
                return typeof(DateTime);
            }

            return typeof(string);
        }

        return value.GetType();
    }

    private static Expression GetAppropiateTypedValue(object value)
    {
        //Basic types conversions
        //Special case => datetime is sent as a string
        if (value is string)
        {
            DateTime dtDateTimeConversion;
            if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dtDateTimeConversion))
            {
                return Expression.Constant(dtDateTimeConversion, typeof(DateTime));
            }

            return GetCaseInsensitiveExpression(Expression.Constant(value, typeof(string)));
        }

        if (value is EntityReference)
        {
            var cast = (value as EntityReference).Id;
            return Expression.Constant(cast);
        }

        if (value is OptionSetValue)
        {
            var cast = (value as OptionSetValue).Value;
            return Expression.Constant(cast);
        }

        if (value is Money)
        {
            var cast = (value as Money).Value;
            return Expression.Constant(cast);
        }

        return Expression.Constant(value);
    }

    private static Expression GetAppropiateCastExpressionBasedOnType(Type t, Expression input, object value)
    {
        var typedExpression = GetAppropiateCastExpressionBasedOnAttributeTypeOrValue(input, value, t);

        //Now, any value (entity reference, string, int, etc,... could be wrapped in an AliasedValue object
        //So let's add this
        var getValueFromAliasedValueExp = Expression.Call(Expression.Convert(input, typeof(AliasedValue)),
            typeof(AliasedValue).GetMethod("get_Value"));

        var exp = Expression.Condition(Expression.TypeIs(input, typeof(AliasedValue)),
            GetAppropiateCastExpressionBasedOnAttributeTypeOrValue(getValueFromAliasedValueExp, value, t),
            typedExpression //Not an aliased value
        );

        return exp;
    }


    private static Expression GetAppropiateCastExpressionBasedOnAttributeTypeOrValue(Expression input, object value, Type attributeType)
    {
        if (attributeType != null)
        {
            if (Nullable.GetUnderlyingType(attributeType) != null)
            {
                attributeType = Nullable.GetUnderlyingType(attributeType);
            }

            if (attributeType == typeof(Guid))
            {
                return GetAppropiateCastExpressionBasedGuid(input);
            }

            if (attributeType == typeof(EntityReference))
            {
                return GetAppropiateCastExpressionBasedOnEntityReference(input, value);
            }

            if (attributeType == typeof(int) || attributeType == typeof(int?) || attributeType.IsOptionSet())
            {
                return GetAppropiateCastExpressionBasedOnInt(input);
            }

            if (attributeType == typeof(decimal) || attributeType == typeof(Money))
            {
                return GetAppropiateCastExpressionBasedOnDecimal(input);
            }

            if (attributeType == typeof(bool) || attributeType == typeof(BooleanManagedProperty))
            {
                return GetAppropiateCastExpressionBasedOnBoolean(input);
            }

            if (attributeType == typeof(string))
            {
                return GetAppropiateCastExpressionBasedOnStringAndType(input, value, attributeType);
            }

            if (attributeType.IsDateTime())
            {
                return GetAppropiateCastExpressionBasedOnDateTime(input, value);
            }

            if (attributeType.IsOptionSetValueCollection())
            {
                return GetAppropiateCastExpressionBasedOnOptionSetValueCollection(input);
            }


            return GetAppropiateCastExpressionDefault(input, value); //any other type
        }

        return GetAppropiateCastExpressionBasedOnValueInherentType(input, value); //Dynamic entities
    }

    private static Expression GetAppropiateCastExpressionBasedOnValueInherentType(Expression input, object value)
    {
        if (value is Guid || value is EntityReference)
        {
            return GetAppropiateCastExpressionBasedGuid(input); //Could be compared against an EntityReference
        }

        if (value is int || value is OptionSetValue)
        {
            return GetAppropiateCastExpressionBasedOnInt(input); //Could be compared against an OptionSet
        }

        if (value is decimal || value is Money)
        {
            return GetAppropiateCastExpressionBasedOnDecimal(input); //Could be compared against a Money
        }

        if (value is bool)
        {
            return GetAppropiateCastExpressionBasedOnBoolean(input); //Could be a BooleanManagedProperty
        }

        if (value is string)
        {
            return GetAppropiateCastExpressionBasedOnString(input, value);
        }

        return GetAppropiateCastExpressionDefault(input, value); //any other type
    }


    private static Expression GetAppropiateCastExpressionBasedOnString(Expression input, object value)
    {
        var defaultStringExpression = GetCaseInsensitiveExpression(GetAppropiateCastExpressionDefault(input, value));

        DateTime dtDateTimeConversion;
        if (DateTime.TryParse(value.ToString(), out dtDateTimeConversion))
        {
            return Expression.Convert(input, typeof(DateTime));
        }

        int iValue;
        if (int.TryParse(value.ToString(), out iValue))
        {
            return Expression.Condition(Expression.TypeIs(input, typeof(OptionSetValue)),
                GetToStringExpression<int>(GetAppropiateCastExpressionBasedOnInt(input)),
                defaultStringExpression
            );
        }

        return defaultStringExpression;
    }

    private static Expression GetAppropiateCastExpressionBasedOnStringAndType(Expression input, object value, Type attributeType)
    {
        var defaultStringExpression = GetCaseInsensitiveExpression(GetAppropiateCastExpressionDefault(input, value));

        int iValue;
        if (attributeType.IsOptionSet() && int.TryParse(value.ToString(), out iValue))
        {
            return Expression.Condition(Expression.TypeIs(input, typeof(OptionSetValue)),
                GetToStringExpression<int>(GetAppropiateCastExpressionBasedOnInt(input)),
                defaultStringExpression
            );
        }

        return defaultStringExpression;
    }

    private static Expression GetToStringExpression<T>(Expression e) => Expression.Call(e, typeof(T).GetMethod("ToString", new Type[] { }));

    private static Expression GetAppropiateCastExpressionBasedOnDateTime(Expression input, object value)
    {
        // Convert to DateTime if string
        DateTime _;
        if (value is DateTime || (value is string && DateTime.TryParse(value.ToString(), out _)))
        {
            return Expression.Convert(input, typeof(DateTime));
        }

        return input; // return directly
    }

    private static Expression GetAppropiateCastExpressionDefault(Expression input, object value) => Expression.Convert(input, value.GetType()); //Default type conversion

    private static Expression GetAppropiateCastExpressionBasedGuid(Expression input)
    {
        var getIdFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)),
            typeof(EntityReference).GetMethod("get_Id"));

        return Expression.Condition(
            Expression.TypeIs(input, typeof(EntityReference)), //If input is an entity reference, compare the Guid against the Id property
            Expression.Convert(
                getIdFromEntityReferenceExpr,
                typeof(Guid)),
            Expression.Condition(Expression.TypeIs(input, typeof(Guid)), //If any other case, then just compare it as a Guid directly
                Expression.Convert(input, typeof(Guid)),
                Expression.Constant(Guid.Empty, typeof(Guid))));
    }

    private static Expression GetAppropiateCastExpressionBasedOnEntityReference(Expression input, object value)
    {
        Guid guid;
        if (value is string && !Guid.TryParse((string)value, out guid))
        {
            var getNameFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)),
                typeof(EntityReference).GetMethod("get_Name"));

            return GetCaseInsensitiveExpression(Expression.Condition(Expression.TypeIs(input, typeof(EntityReference)),
                Expression.Convert(getNameFromEntityReferenceExpr, typeof(string)),
                Expression.Constant(string.Empty, typeof(string))));
        }

        var getIdFromEntityReferenceExpr = Expression.Call(Expression.TypeAs(input, typeof(EntityReference)),
            typeof(EntityReference).GetMethod("get_Id"));

        return Expression.Condition(
            Expression.TypeIs(input, typeof(EntityReference)), //If input is an entity reference, compare the Guid against the Id property
            Expression.Convert(
                getIdFromEntityReferenceExpr,
                typeof(Guid)),
            Expression.Condition(Expression.TypeIs(input, typeof(Guid)), //If any other case, then just compare it as a Guid directly
                Expression.Convert(input, typeof(Guid)),
                Expression.Constant(Guid.Empty, typeof(Guid))));
    }

    private static Expression GetAppropiateCastExpressionBasedOnDecimal(Expression input) =>
        Expression.Condition(
            Expression.TypeIs(input, typeof(Money)),
            Expression.Convert(
                Expression.Call(Expression.TypeAs(input, typeof(Money)),
                    typeof(Money).GetMethod("get_Value")),
                typeof(decimal)),
            Expression.Condition(Expression.TypeIs(input, typeof(decimal)),
                Expression.Convert(input, typeof(decimal)),
                Expression.Constant(0.0M)));

    private static Expression GetAppropiateCastExpressionBasedOnBoolean(Expression input) =>
        Expression.Condition(
            Expression.TypeIs(input, typeof(BooleanManagedProperty)),
            Expression.Convert(
                Expression.Call(Expression.TypeAs(input, typeof(BooleanManagedProperty)),
                    typeof(BooleanManagedProperty).GetMethod("get_Value")),
                typeof(bool)),
            Expression.Condition(Expression.TypeIs(input, typeof(bool)),
                Expression.Convert(input, typeof(bool)),
                Expression.Constant(false)));

    private static Expression GetAppropiateCastExpressionBasedOnInt(Expression input) =>
        Expression.Condition(
            Expression.TypeIs(input, typeof(OptionSetValue)),
            Expression.Convert(
                Expression.Call(Expression.TypeAs(input, typeof(OptionSetValue)),
                    typeof(OptionSetValue).GetMethod("get_Value")),
                typeof(int)),
            Expression.Convert(input, typeof(int)));

    private static Expression GetAppropiateCastExpressionBasedOnOptionSetValueCollection(Expression input) =>
        Expression.Call(typeof(ConditionParser).GetMethod("ConvertToHashSetOfInt"), input, Expression.Constant(true));

    #endregion
}
