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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(1)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
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

        var stub = new DataverseStub(new MockClock(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
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

    #endregion
}
