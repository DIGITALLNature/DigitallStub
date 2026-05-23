// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for ConditionExpression.CompareColumns = true which enables column-to-column
/// comparison within the same row (e.g. WHERE column1 = column2).
/// </summary>
public class CompareColumnsTests
{
    [Test]
    public async Task CompareColumns_Equal_MatchesWhenColumnsHaveSameValue()
    {
        var match = new Account(Guid.NewGuid()) { Name = "Test", [Account.LogicalNames.Description] = "Test" };
        var noMatch = new Account(Guid.NewGuid()) { Name = "Hello", [Account.LogicalNames.Description] = "World" };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, Account.LogicalNames.Description)
        {
            CompareColumns = true
        };

        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_NotEqual_MatchesWhenColumnsHaveDifferentValues()
    {
        var match = new Account(Guid.NewGuid()) { Name = "Hello", [Account.LogicalNames.Description] = "World" };
        var noMatch = new Account(Guid.NewGuid()) { Name = "Same", [Account.LogicalNames.Description] = "Same" };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.NotEqual, Account.LogicalNames.Description)
        {
            CompareColumns = true
        };

        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_GreaterThan_MatchesWhenLeftColumnIsGreater()
    {
        var match = new Account(Guid.NewGuid()) { Revenue = new Money(100m), [Account.LogicalNames.MarketCap] = new Money(50m) };
        var noMatch = new Account(Guid.NewGuid()) { Revenue = new Money(30m), [Account.LogicalNames.MarketCap] = new Money(50m) };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression(Account.LogicalNames.Revenue, ConditionOperator.GreaterThan, Account.LogicalNames.MarketCap)
        {
            CompareColumns = true
        };

        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_WithNullColumn_DoesNotMatch()
    {
        // field_b (description) is not set → null; Equal should not match
        var entity = new Account(Guid.NewGuid()) { Name = "Hello" };

        var sut = new FakeOrganizationService();
        sut.Add(entity);

        var condition = new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, Account.LogicalNames.Description)
        {
            CompareColumns = true
        };

        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).IsEmpty();
    }

    [Test]
    public async Task CompareColumns_BothNull_MatchesForEqual()
    {
        // Both description and websiteurl are not set → null == null should match for Equal
        var entity = new Account(Guid.NewGuid()) { Name = "Test" };

        var sut = new FakeOrganizationService();
        sut.Add(entity);

        var condition = new ConditionExpression(Account.LogicalNames.Description, ConditionOperator.Equal, Account.LogicalNames.WebSiteURL)
        {
            CompareColumns = true
        };

        var query = new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
    }
}
