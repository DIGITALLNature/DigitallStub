// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Digitall.Stub.Logic.Queries;
using Digitall.Stub.Tests.Fixtures;
using DotNetEnv;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Tests.Logic;

[TestClass]
public class QueryTests
{
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext)
    {
        Environment.SetEnvironmentVariable("MaxRetrieveCount", "10");
        Env.Load();
    }

    #region equal

    [TestMethod]
    public void GenerateQuery_Equal_string()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Equal, "A Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_guid()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountId, ConditionOperator.Equal, Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_entityref()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.ParentAccountId, ConditionOperator.Equal, new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001")));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_int()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Address1UTCOffset, ConditionOperator.Equal, -120);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }


    [TestMethod]
    public void GenerateQuery_Equal_money()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.Equal, new Money(123));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_bool()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketingOnly, ConditionOperator.Equal, true);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_DateTime()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Equal, new DateTime(2000,1,2));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Equal_Optionsetvalue()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.Equal, new OptionSetValue(Account.Options.AccountCategoryCode.PreferredCustomer));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_On()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.On, new DateTime(2000,1,2));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Today()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Today);

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Yesterday()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Yesterday);

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(1)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Tomorrow()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Tomorrow);

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_EqualBusinessId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwningBusinessUnit, ConditionOperator.EqualBusinessId);

        Environment.SetEnvironmentVariable("BusinessUnitId", TestData.BusinessUnitId.ToString("N"));

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }
    [TestMethod]
    public void GenerateQuery_EqualUserId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwnerId, ConditionOperator.EqualUserId);

        Environment.SetEnvironmentVariable("CallerId", TestData.CallerId.ToString("N"));

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    #endregion

    #region not equal
    [TestMethod]
    public void GenerateQuery_NotEqual_String()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotEqual, "A Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_Guid()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountId, ConditionOperator.NotEqual, Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_EntityRef()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.ParentAccountId, ConditionOperator.NotEqual, new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001")));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_Integer()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Address1UTCOffset, ConditionOperator.NotEqual, -120);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }


    [TestMethod]
    public void GenerateQuery_NotEqual_Money()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.NotEqual, new Money(123));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_Bool()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketingOnly, ConditionOperator.NotEqual, true);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_DateTime()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NotEqual, new DateTime(2000,1,2));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqual_Optionsetvalue()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.NotEqual, new OptionSetValue(Account.Options.AccountCategoryCode.PreferredCustomer));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotOn()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NotOn, new DateTime(2000,1,2));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotEqualBusinessId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwningBusinessUnit, ConditionOperator.NotEqualBusinessId);

        Environment.SetEnvironmentVariable("BusinessUnitId", TestData.BusinessUnitId.ToString("N"));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }
    [TestMethod]
    public void GenerateQuery_NotEqualUserId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwnerId, ConditionOperator.NotEqualUserId);

        Environment.SetEnvironmentVariable("CallerId", TestData.CallerId.ToString("N"));

        var stub = new DataverseStub(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }
    #endregion

    #region like

    [TestMethod]
    public void GenerateQuery_Like_Left()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "%Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    [TestMethod]
    public void GenerateQuery_Like_Right()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "A C%");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_Beetween()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "%Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    [TestMethod]
    public void GenerateQuery_BeginsWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.BeginsWith, "A ");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_EndsWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.EndsWith, "Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    [TestMethod]
    public void GenerateQuery_Contains()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Contains, "Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    #endregion

    #region not like
    [TestMethod]
    public void GenerateQuery_NotLike_Left()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "%Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(0);
    }

    [TestMethod]
    public void GenerateQuery_NotLike_Right()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "A C%");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotLike_Beetween()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "%Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(0);
    }

    [TestMethod]
    public void GenerateQuery_DoesNotBeginWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotBeginWith, "A ");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_DoesNotEndWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotEndWith, "Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(0);
    }

    [TestMethod]
    public void GenerateQuery_DoesNotContain()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotContain, "Corp");

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(0);
    }
    #endregion

    #region null and not null
    [TestMethod]
    public void GenerateQuery_Null()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.Null);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_NotNull()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.NotNull);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    #endregion

    #region greater than and less than

    [TestMethod]
    public void GenerateQuery_GreaterThan()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.GreaterThan, new Money(320));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_GreaterEqual()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.GreaterEqual, new Money(321));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_LessThan()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.LessThan, new Money(124));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    [TestMethod]
    public void GenerateQuery_LessEqual()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.LessEqual, new Money(123));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    #endregion

    #region Array Operations

    [TestMethod]
    public void GenerateQuery_In()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.In, new Money(123),new Money(321), new Money(111));

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    [TestMethod]
    public void GenerateQuery_ContainValues()
    {

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition("new_accountcategorycodemultiple", ConditionOperator.ContainValues, 3);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(2);
    }

    [TestMethod]
    public void GenerateQuery_DoesNotContainValues()
    {

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition("new_accountcategorycodemultiple", ConditionOperator.DoesNotContainValues, 1 , 2);

        var stub = new DataverseStub();
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    #endregion

    #region Time Operations
    /**
     * case ConditionOperator.OnOrAfter:
                operatorExpression = Expression.Or(
                    TranslateConditionExpressionEqual(context.TimeProvider, condition, getNonBasicValueExpr, containsAttributeExpression),
                    TranslateConditionExpressionGreaterThan(condition, getNonBasicValueExpr, containsAttributeExpression));
                break;
            case ConditionOperator.LastXHours:
            case ConditionOperator.LastXDays:
            case ConditionOperator.Last7Days:
            case ConditionOperator.LastXWeeks:
            case ConditionOperator.LastXMonths:
            case ConditionOperator.LastXYears:
                operatorExpression = TranslateConditionExpressionLast(context.TimeProvider, condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.OnOrBefore:
                operatorExpression = Expression.Or(
                    TranslateConditionExpressionEqual(context.TimeProvider, condition, getNonBasicValueExpr, containsAttributeExpression),
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
                operatorExpression = TranslateConditionExpressionOlderThan(context.TimeProvider,condition, getNonBasicValueExpr, containsAttributeExpression);
                break;

            case ConditionOperator.NextXHours:
            case ConditionOperator.NextXDays:
            case ConditionOperator.Next7Days:
            case ConditionOperator.NextXWeeks:
            case ConditionOperator.NextXMonths:
            case ConditionOperator.NextXYears:
                operatorExpression = TranslateConditionExpressionNext(context.TimeProvider,condition, getNonBasicValueExpr, containsAttributeExpression);
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
     */
    #endregion
}
