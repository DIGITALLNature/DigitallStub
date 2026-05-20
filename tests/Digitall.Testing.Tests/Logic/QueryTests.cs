// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Digitall.Testing.Logic.Queries;
using Digitall.Testing.Tests.Fixtures;
using DotNetEnv;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests.Logic;

public class QueryTests
{
    [Before(Class)]
    public static async Task MyClassInitialize()
    {
        Environment.SetEnvironmentVariable("MaxRetrieveCount", "10");
        Env.Load();
        await Task.CompletedTask;
    }

    #region equal

    [Test]
    public async Task GenerateQuery_Equal_string()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Equal, "A Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_guid()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountId, ConditionOperator.Equal, Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_entityref()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.ParentAccountId, ConditionOperator.Equal, new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001")));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_int()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Address1UTCOffset, ConditionOperator.Equal, -120);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }


    [Test]
    public async Task GenerateQuery_Equal_money()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.Equal, new Money(123));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_bool()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketingOnly, ConditionOperator.Equal, true);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_DateTime()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Equal, new DateTime(2000,1,2));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Equal_Optionsetvalue()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.Equal, new OptionSetValue(Account.Options.AccountCategoryCode.PreferredCustomer));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_On()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.On, new DateTime(2000,1,2));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Today()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Today);

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Yesterday()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Yesterday);

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(1)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Tomorrow()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Tomorrow);

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_EqualBusinessId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwningBusinessUnit, ConditionOperator.EqualBusinessId);

        Environment.SetEnvironmentVariable("BusinessUnitId", TestData.BusinessUnitId.ToString("N"));

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }
    [Test]
    public async Task GenerateQuery_EqualUserId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwnerId, ConditionOperator.EqualUserId);

        Environment.SetEnvironmentVariable("UserId", TestData.UserId.ToString("N"));

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    #endregion

    #region not equal
    [Test]
    public async Task GenerateQuery_NotEqual_String()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotEqual, "A Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_Guid()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountId, ConditionOperator.NotEqual, Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_EntityRef()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.ParentAccountId, ConditionOperator.NotEqual, new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001")));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_Integer()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Address1UTCOffset, ConditionOperator.NotEqual, -120);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }


    [Test]
    public async Task GenerateQuery_NotEqual_Money()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.NotEqual, new Money(123));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_Bool()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketingOnly, ConditionOperator.NotEqual, true);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_DateTime()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NotEqual, new DateTime(2000,1,2));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqual_Optionsetvalue()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.NotEqual, new OptionSetValue(Account.Options.AccountCategoryCode.PreferredCustomer));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotOn()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NotOn, new DateTime(2000,1,2));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotEqualBusinessId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwningBusinessUnit, ConditionOperator.NotEqualBusinessId);

        Environment.SetEnvironmentVariable("BusinessUnitId", TestData.BusinessUnitId.ToString("N"));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }
    [Test]
    public async Task GenerateQuery_NotEqualUserId()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OwnerId, ConditionOperator.NotEqualUserId);

        Environment.SetEnvironmentVariable("UserId", TestData.UserId.ToString("N"));

        var dataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTime(1999, 12, 31,0,5,0, DateTimeKind.Utc).AddDays(-1)));
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }
    #endregion

    #region like

    [Test]
    public async Task GenerateQuery_Like_Left()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "%Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    [Test]
    public async Task GenerateQuery_Like_Right()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "A C%");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_Beetween()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Like, "%Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    [Test]
    public async Task GenerateQuery_BeginsWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.BeginsWith, "A ");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_EndsWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.EndsWith, "Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    [Test]
    public async Task GenerateQuery_Contains()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.Contains, "Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    #endregion

    #region not like
    [Test]
    public async Task GenerateQuery_NotLike_Left()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "%Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(0);
    }

    [Test]
    public async Task GenerateQuery_NotLike_Right()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "A C%");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotLike_Beetween()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotLike, "%Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(0);
    }

    [Test]
    public async Task GenerateQuery_DoesNotBeginWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotBeginWith, "A ");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_DoesNotEndWith()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotEndWith, "Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(0);
    }

    [Test]
    public async Task GenerateQuery_DoesNotContain()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.DoesNotContain, "Corp");

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(0);
    }
    #endregion

    #region null and not null
    [Test]
    public async Task GenerateQuery_Null()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.Null);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_NotNull()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.AccountCategoryCode, ConditionOperator.NotNull);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    #endregion

    #region greater than and less than

    [Test]
    public async Task GenerateQuery_GreaterThan()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.GreaterThan, new Money(320));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_GreaterEqual()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.GreaterEqual, new Money(321));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_LessThan()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.LessThan, new Money(124));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    [Test]
    public async Task GenerateQuery_LessEqual()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.LessEqual, new Money(123));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
    }

    #endregion

    #region Array Operations

    [Test]
    public async Task GenerateQuery_In()
    {
        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.MarketCap, ConditionOperator.In, new Money(123),new Money(321), new Money(111));

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    [Test]
    public async Task GenerateQuery_ContainValues()
    {

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition("new_accountcategorycodemultiple", ConditionOperator.ContainValues, 3);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(2);
    }

    [Test]
    public async Task GenerateQuery_DoesNotContainValues()
    {

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition("new_accountcategorycodemultiple", ConditionOperator.DoesNotContainValues, 1 , 2);

        var dataverse = new FakeOrganizationService();
        dataverse.AddRange(TestData.Default);
        var sut = new ExpressionProcessor(dataverse);

        var result = sut.Generate(query);
        await Assert.That(result).IsNotNull();

        var queryResult = dataverse.CreateQuery<Account>().Where(result);
        await Assert.That(queryResult).IsNotNull();
        await Assert.That(queryResult).Count().IsEqualTo(1);
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
