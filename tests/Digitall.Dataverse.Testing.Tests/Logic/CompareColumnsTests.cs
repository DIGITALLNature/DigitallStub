// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for ConditionExpression.CompareColumns = true which enables column-to-column
/// comparison within the same row (e.g. WHERE column1 = column2).
/// This feature is NOT YET IMPLEMENTED — these tests document the expected behavior.
/// </summary>
public class CompareColumnsTests
{
    [Test]
    public async Task CompareColumns_Equal_MatchesWhenColumnsHaveSameValue()
    {
        var match = new Entity("custom_entity", Guid.NewGuid())
        {
            ["field_a"] = "hello",
            ["field_b"] = "hello"
        };
        var noMatch = new Entity("custom_entity", Guid.NewGuid())
        {
            ["field_a"] = "hello",
            ["field_b"] = "world"
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression("field_a", ConditionOperator.Equal, "field_b")
        {
            CompareColumns = true
        };

        var query = new QueryExpression("custom_entity")
        {
            ColumnSet = new ColumnSet(true)
        };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_NotEqual_MatchesWhenColumnsHaveDifferentValues()
    {
        var match = new Entity("custom_entity", Guid.NewGuid())
        {
            ["field_a"] = "hello",
            ["field_b"] = "world"
        };
        var noMatch = new Entity("custom_entity", Guid.NewGuid())
        {
            ["field_a"] = "same",
            ["field_b"] = "same"
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression("field_a", ConditionOperator.NotEqual, "field_b")
        {
            CompareColumns = true
        };

        var query = new QueryExpression("custom_entity") { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_GreaterThan_MatchesWhenLeftColumnIsGreater()
    {
        var match = new Entity("custom_entity", Guid.NewGuid())
        {
            ["amount_a"] = 100m,
            ["amount_b"] = 50m
        };
        var noMatch = new Entity("custom_entity", Guid.NewGuid())
        {
            ["amount_a"] = 30m,
            ["amount_b"] = 50m
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([match, noMatch]);

        var condition = new ConditionExpression("amount_a", ConditionOperator.GreaterThan, "amount_b")
        {
            CompareColumns = true
        };

        var query = new QueryExpression("custom_entity") { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(match.Id);
    }

    [Test]
    public async Task CompareColumns_WithNullColumn_DoesNotMatch()
    {
        var entity = new Entity("custom_entity", Guid.NewGuid())
        {
            ["field_a"] = "hello"
            // field_b is not set (null)
        };

        var sut = new FakeOrganizationService();
        sut.Add(entity);

        var condition = new ConditionExpression("field_a", ConditionOperator.Equal, "field_b")
        {
            CompareColumns = true
        };

        var query = new QueryExpression("custom_entity") { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).IsEmpty();
    }

    [Test]
    public async Task CompareColumns_BothNull_MatchesForEqual()
    {
        var entity = new Entity("custom_entity", Guid.NewGuid());
        // Both field_a and field_b are null

        var sut = new FakeOrganizationService();
        sut.Add(entity);

        var condition = new ConditionExpression("field_a", ConditionOperator.Equal, "field_b")
        {
            CompareColumns = true
        };

        var query = new QueryExpression("custom_entity") { ColumnSet = new ColumnSet(true) };
        query.Criteria.AddCondition(condition);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
    }
}
