// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Logic.Queries
{
    public static class ConditionParser
    {
        public static Expression TranslateConditionExpression(QueryExpression qe, DataverseStub context, TypedConditionExpression c, ParameterExpression entity)
        {
            Expression attributesProperty = Expression.Property(
                entity,
                "Attributes"
                );



            string attributeName = "";

            //Do not prepend the entity name if the EntityLogicalName is the same as the QueryExpression main logical name

            if (!string.IsNullOrWhiteSpace(c.CondExpression.EntityName) && !c.CondExpression.EntityName.Equals(qe.EntityName))
            {
                attributeName = c.CondExpression.EntityName + "." + c.CondExpression.AttributeName;
            }
            else
                attributeName = c.CondExpression.AttributeName;

            Expression containsAttributeExpression = Expression.Call(
                attributesProperty,
                typeof(AttributeCollection).GetMethod("ContainsKey", new Type[] { typeof(string) }),
                Expression.Constant(attributeName)
                );

            Expression getAttributeValueExpr = Expression.Property(
                            attributesProperty, "Item",
                            Expression.Constant(attributeName, typeof(string))
                            );



            Expression getNonBasicValueExpr = getAttributeValueExpr;

            Expression operatorExpression = null;

            switch (c.CondExpression.Operator)
            {
                case ConditionOperator.Equal:
                case ConditionOperator.Today:
                case ConditionOperator.Yesterday:
                case ConditionOperator.Tomorrow:
                case ConditionOperator.EqualUserId:
                    operatorExpression = TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotEqualUserId:
                    operatorExpression = Expression.Not(TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.EqualBusinessId:
                    operatorExpression = TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotEqualBusinessId:
                    operatorExpression = Expression.Not(TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.BeginsWith:
                case ConditionOperator.Like:
                    operatorExpression = TranslateConditionExpressionLike(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.EndsWith:
                    operatorExpression = TranslateConditionExpressionEndsWith(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.Contains:
                    operatorExpression = TranslateConditionExpressionContains(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotEqual:
                    operatorExpression = Expression.Not(TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.DoesNotBeginWith:
                case ConditionOperator.DoesNotEndWith:
                case ConditionOperator.NotLike:
                case ConditionOperator.DoesNotContain:
                    operatorExpression = Expression.Not(TranslateConditionExpressionLike(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.Null:
                    operatorExpression = TranslateConditionExpressionNull(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotNull:
                    operatorExpression = Expression.Not(TranslateConditionExpressionNull(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.GreaterThan:
                    operatorExpression = TranslateConditionExpressionGreaterThan(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.GreaterEqual:
                    operatorExpression = TranslateConditionExpressionGreaterThanOrEqual(context, c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.LessThan:
                    operatorExpression = TranslateConditionExpressionLessThan(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.LessEqual:
                    operatorExpression = TranslateConditionExpressionLessThanOrEqual(context, c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.In:
                    operatorExpression = TranslateConditionExpressionIn(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotIn:
                    operatorExpression = Expression.Not(TranslateConditionExpressionIn(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.On:
                    operatorExpression = TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotOn:
                    operatorExpression = Expression.Not(TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.OnOrAfter:
                    operatorExpression = Expression.Or(
                               TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression),
                               TranslateConditionExpressionGreaterThan(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;
                case ConditionOperator.LastXHours:
                case ConditionOperator.LastXDays:
                case ConditionOperator.Last7Days:
                case ConditionOperator.LastXWeeks:
                case ConditionOperator.LastXMonths:
                case ConditionOperator.LastXYears:
                    operatorExpression = TranslateConditionExpressionLast(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.OnOrBefore:
                    operatorExpression = Expression.Or(
                                TranslateConditionExpressionEqual(context, c, getNonBasicValueExpr, containsAttributeExpression),
                                TranslateConditionExpressionLessThan(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;

                case ConditionOperator.Between:
                    if (c.CondExpression.Values.Count != 2)
                    {
                        throw new Exception("Between operator requires exactly 2 values.");
                    }
                    operatorExpression = TranslateConditionExpressionBetween(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NotBetween:
                    if (c.CondExpression.Values.Count != 2)
                    {
                        throw new Exception("Not-Between operator requires exactly 2 values.");
                    }
                    operatorExpression = Expression.Not(TranslateConditionExpressionBetween(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;
                case ConditionOperator.OlderThanXMinutes:
                case ConditionOperator.OlderThanXHours:
                case ConditionOperator.OlderThanXDays:
                case ConditionOperator.OlderThanXWeeks:
                case ConditionOperator.OlderThanXYears:
                case ConditionOperator.OlderThanXMonths:
                    operatorExpression = TranslateConditionExpressionOlderThan(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.NextXHours:
                case ConditionOperator.NextXDays:
                case ConditionOperator.Next7Days:
                case ConditionOperator.NextXWeeks:
                case ConditionOperator.NextXMonths:
                case ConditionOperator.NextXYears:
                    operatorExpression = TranslateConditionExpressionNext(c, getNonBasicValueExpr, containsAttributeExpression);
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
                    operatorExpression = TranslateConditionExpressionBetweenDates(c, getNonBasicValueExpr, containsAttributeExpression, context);
                    break;

                case ConditionOperator.ContainValues:
                    operatorExpression = TranslateConditionExpressionContainValues(c, getNonBasicValueExpr, containsAttributeExpression);
                    break;

                case ConditionOperator.DoesNotContainValues:
                    operatorExpression = Expression.Not(TranslateConditionExpressionContainValues(c, getNonBasicValueExpr, containsAttributeExpression));
                    break;


                default:
                    throw new PullRequestException(string.Format("Operator {0} not yet implemented for condition expression", c.CondExpression.Operator.ToString()));


            }

            if (c.IsOuter)
            {
                //If outer join, filter is optional, only if there was a value
                return Expression.Constant(true);
            }
            else
                return operatorExpression;

        }

        static Expression TranslateConditionExpressionEqual(DataverseStub context, TypedConditionExpression c, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {

            BinaryExpression expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));

            object unaryOperatorValue = null;

            switch (c.CondExpression.Operator)
            {
                case ConditionOperator.Today:
                    unaryOperatorValue = DateTime.Today;
                    break;
                case ConditionOperator.Yesterday:
                    unaryOperatorValue = DateTime.Today.AddDays(-1);
                    break;
                case ConditionOperator.Tomorrow:
                    unaryOperatorValue = DateTime.Today.AddDays(1);
                    break;
                case ConditionOperator.EqualUserId:
                case ConditionOperator.NotEqualUserId:
                    unaryOperatorValue = context.CallerId;
                    break;

                case ConditionOperator.EqualBusinessId:
                case ConditionOperator.NotEqualBusinessId:
                    unaryOperatorValue = context.BusinessUnitId;
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
                var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(conditionValue, isOptionSetValueCollectionAccepted: false));

                expOrValues = Expression.Equal(
                    Expression.Call(leftHandSideExpression, typeof(HashSet<int>).GetMethod("SetEquals"), rightHandSideExpression),
                    Expression.Constant(true));
            }

            else
            {
                foreach (object value in c.CondExpression.Values)
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

        internal static void ValidateSupportedTypedExpression(TypedConditionExpression typedExpression)
        {
            Expression validateOperatorTypeExpression = Expression.Empty();
            ConditionOperator[] supportedOperators = (ConditionOperator[])Enum.GetValues(typeof(ConditionOperator));


            if (typedExpression.AttributeType == typeof(OptionSetValueCollection))
            {
                supportedOperators = new[]
                {
                    ConditionOperator.ContainValues,
                    ConditionOperator.DoesNotContainValues,
                    ConditionOperator.Equal,
                    ConditionOperator.NotEqual,
                    ConditionOperator.NotNull,
                    ConditionOperator.Null,
                    ConditionOperator.In,
                    ConditionOperator.NotIn,
                };
            }


            if (!supportedOperators.Contains(typedExpression.CondExpression.Operator))
            {
                var errorMsg = "The operator is not valid or it is not supported.";
                throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault() { ErrorCode = (int)ErrorCodes.InvalidOperatorCode, Message = errorMsg }, errorMsg);
            }
        }

        static Expression TranslateConditionExpressionLike(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            BinaryExpression expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
            Expression convertedValueToStr = Expression.Convert(GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, c.Values[0]), typeof(string));

            Expression convertedValueToStrAndToLower = GetCaseInsensitiveExpression(convertedValueToStr);

            string sLikeOperator = "%";
            foreach (object value in c.Values)
            {
                var strValue = value.ToString();
                string sMethod = "";

                if (strValue.EndsWith(sLikeOperator) && strValue.StartsWith(sLikeOperator))
                    sMethod = "Contains";

                else if (strValue.StartsWith(sLikeOperator))
                    sMethod = "EndsWith";

                else
                    sMethod = "StartsWith";

                expOrValues = Expression.Or(expOrValues, Expression.Call(
                    convertedValueToStrAndToLower,
                    typeof(string).GetMethod(sMethod, new Type[] { typeof(string) }),
                    Expression.Constant(value.ToString().ToLowerInvariant().Replace("%", "")) //Linq2CRM adds the percentage value to be executed as a LIKE operator, here we are replacing it to just use the appropiate method
                ));
            }

            return Expression.AndAlso(
                containsAttributeExpr,
                expOrValues);
        }

        static Expression TranslateConditionExpressionEndsWith(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            //Append a ´%´at the end of each condition value
            var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x.ToString()).ToList());
            var typedComputedCondition = new TypedConditionExpression(computedCondition);
            typedComputedCondition.AttributeType = tc.AttributeType;

            return TranslateConditionExpressionLike(typedComputedCondition, getAttributeValueExpr, containsAttributeExpr);
        }

         static Expression TranslateConditionExpressionContains(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            //Append a ´%´at the end of each condition value
            var computedCondition = new ConditionExpression(c.AttributeName, c.Operator, c.Values.Select(x => "%" + x.ToString() + "%").ToList());
            var computedTypedCondition = new TypedConditionExpression(computedCondition);
            computedTypedCondition.AttributeType = tc.AttributeType;

            return TranslateConditionExpressionLike(computedTypedCondition, getAttributeValueExpr, containsAttributeExpr);

        }

        static Expression TranslateConditionExpressionNull(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            return Expression.Or(Expression.AndAlso(
                    containsAttributeExpr,
                    Expression.Equal(
                        getAttributeValueExpr,
                        Expression.Constant(null))),   //Attribute is null
                Expression.AndAlso(
                    Expression.Not(containsAttributeExpr),
                    Expression.Constant(true)));   //Or attribute is not defined (null)
        }

             static Expression TranslateConditionExpressionGreaterThanOrEqual(XrmFakedContext context, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            //var c = tc.CondExpression;

            return Expression.Or(
                                TranslateConditionExpressionEqual(context, tc, getAttributeValueExpr, containsAttributeExpr),
                                TranslateConditionExpressionGreaterThan(tc, getAttributeValueExpr, containsAttributeExpr));

        }
        static Expression TranslateConditionExpressionGreaterThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
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
            else if (GetAppropiateTypeForValue(c.Values[0]) == typeof(string))
            {
                return TranslateConditionExpressionGreaterThanString(tc, getAttributeValueExpr, containsAttributeExpr);
            }
            else
            {
                BinaryExpression expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
                foreach (object value in c.Values)
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

        }

               static Expression TranslateConditionExpressionLessThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
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
            else if (GetAppropiateTypeForValue(c.Values[0]) == typeof(string))
            {
                return TranslateConditionExpressionLessThanString(tc, getAttributeValueExpr, containsAttributeExpr);
            }
            else
            {
                BinaryExpression expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));
                foreach (object value in c.Values)
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

        }

        static Expression TranslateConditionExpressionLessThanOrEqual(XrmFakedContext context, TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            //var c = tc.CondExpression;

            return Expression.Or(
                TranslateConditionExpressionEqual(context, tc, getAttributeValueExpr, containsAttributeExpr),
                TranslateConditionExpressionLessThan(tc, getAttributeValueExpr, containsAttributeExpr));

        }

         static Expression TranslateConditionExpressionIn(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            BinaryExpression expOrValues = Expression.Or(Expression.Constant(false), Expression.Constant(false));


            {
                foreach (object value in c.Values)
                {
                    if (value is Array)
                    {
                        foreach (var a in ((Array)value))
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

        static Expression TranslateConditionExpressionLast(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            var beforeDateTime = default(DateTime);
            var currentDateTime = DateTime.UtcNow;
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

        static Expression TranslateConditionExpressionBetween(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
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

         static Expression TranslateConditionExpressionOlderThan(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
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

            DateTime toDate = default(DateTime);

            switch (c.Operator)
            {
                case ConditionOperator.OlderThanXMonths:
                    toDate = DateTime.UtcNow.AddMonths(-valueToAdd);
                    break;
#if !FAKE_XRM_EASY && !FAKE_XRM_EASY_2013
                case ConditionOperator.OlderThanXMinutes:
                    toDate = DateTime.UtcNow.AddMinutes(-valueToAdd);
                    break;
                case ConditionOperator.OlderThanXHours:
                    toDate = DateTime.UtcNow.AddHours(-valueToAdd);
                    break;
                case ConditionOperator.OlderThanXDays:
                    toDate = DateTime.UtcNow.AddDays(-valueToAdd);
                    break;
                case ConditionOperator.OlderThanXWeeks:
                    toDate = DateTime.UtcNow.AddDays(-7 * valueToAdd);
                    break;
                case ConditionOperator.OlderThanXYears:
                    toDate = DateTime.UtcNow.AddYears(-valueToAdd);
                    break;
#endif
            }

            return TranslateConditionExpressionOlderThan(tc, getAttributeValueExpr, containsAttributeExpr, toDate);
        }

         protected static Expression TranslateConditionExpressionNext(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var c = tc.CondExpression;

            var nextDateTime = default(DateTime);
            var currentDateTime = DateTime.UtcNow;
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


        protected static Expression TranslateConditionExpressionContainValues(TypedConditionExpression tc, Expression getAttributeValueExpr, Expression containsAttributeExpr)
        {
            var leftHandSideExpression = GetAppropiateCastExpressionBasedOnType(tc.AttributeType, getAttributeValueExpr, null);
            var rightHandSideExpression = Expression.Constant(ConvertToHashSetOfInt(tc.CondExpression.Values, isOptionSetValueCollectionAccepted: false));

            return Expression.AndAlso(
                       containsAttributeExpr,
                       Expression.AndAlso(
                           Expression.NotEqual(getAttributeValueExpr, Expression.Constant(null)),
                           Expression.Equal(
                               Expression.Call(leftHandSideExpression, typeof(HashSet<int>).GetMethod("Overlaps"), rightHandSideExpression),
                               Expression.Constant(true))));
        }


 }
}
