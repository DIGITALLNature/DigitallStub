// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using Digitall.Stub.Logic.Queries;
using Digitall.Stub.Tests.Fixtures;
using DotNetEnv;
using FluentAssertions;
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
    public void GenerateQuery_equal_string()
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
    public void GenerateQuery_equal_guid()
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
    public void GenerateQuery_equal_entityref()
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
    public void GenerateQuery_equal_int()
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
    public void GenerateQuery_equal_money()
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

    public void GenerateQuery_equal_bool()
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

    public void GenerateQuery_equal_datetime()
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

    public void GenerateQuery_equal_optionsetvalue()
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
    public void GenerateQuery_on()
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

    public void GenerateQuery_today()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Today);

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31)));
        stub.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(stub);

        var result = sut.Generate(query);
        result.Should().NotBeNull();

        var queryResult = stub.CreateQuery<Account>().Where(result);
        queryResult.Should().NotBeNull();
        queryResult.Should().HaveCount(1);
    }

    // case ConditionOperator.Today:
    // case ConditionOperator.Yesterday:
    // case ConditionOperator.Tomorrow:
    // case ConditionOperator.EqualUserId:
    // case ConditionOperator.EqualBusinessId:

    #endregion

    #region not equal
    //            case ConditionOperator.NotOn:
    // case ConditionOperator.NotEqual:
    // case ConditionOperator.NotEqualUserId:
    // case ConditionOperator.NotEqualBusinessId:
    #endregion
}
