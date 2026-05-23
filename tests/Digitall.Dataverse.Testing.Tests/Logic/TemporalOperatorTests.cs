// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Logic.Queries;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for temporal ConditionOperators (Last/Next/This/OlderThan) that were
/// implemented but lacked dedicated test coverage.
/// </summary>
public class TemporalOperatorTests
{
    private static readonly DateTime Now = new(2024, 6, 12, 12, 0, 0, DateTimeKind.Utc); // Wednesday

    private static FakeOrganizationService CreateService(params Entity[] entities)
    {
        var sut = new FakeOrganizationService(new FakeTimeProvider(Now));
        sut.AddRange(entities);
        return sut;
    }

    private static Account AccountAt(DateTime date) => new(Guid.NewGuid()) { OverriddenCreatedOn = date };

    #region LastX Operators

    [Test]
    public async Task LastXHours_MatchesRecordWithinLastXHours()
    {
        var inside = AccountAt(Now.AddHours(-2));
        var outside = AccountAt(Now.AddHours(-10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXHours, 5);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastXDays_MatchesRecordWithinLastXDays()
    {
        var inside = AccountAt(Now.AddDays(-3));
        var outside = AccountAt(Now.AddDays(-10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXDays, 5);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task Last7Days_MatchesRecordWithinLast7Days()
    {
        var inside = AccountAt(Now.AddDays(-5));
        var outside = AccountAt(Now.AddDays(-10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Last7Days);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastXWeeks_MatchesRecordWithinLastXWeeks()
    {
        var inside = AccountAt(Now.AddDays(-10)); // ~1.4 weeks ago
        var outside = AccountAt(Now.AddDays(-30)); // ~4.3 weeks ago
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXWeeks, 2);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastXMonths_MatchesRecordWithinLastXMonths()
    {
        var inside = AccountAt(Now.AddMonths(-2));
        var outside = AccountAt(Now.AddMonths(-6));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXMonths, 3);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastXYears_MatchesRecordWithinLastXYears()
    {
        var inside = AccountAt(Now.AddYears(-1));
        var outside = AccountAt(Now.AddYears(-5));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXYears, 2);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    #endregion

    #region NextX Operators

    [Test]
    public async Task NextXHours_MatchesRecordWithinNextXHours()
    {
        var inside = AccountAt(Now.AddHours(3));
        var outside = AccountAt(Now.AddHours(10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextXHours, 5);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextXDays_MatchesRecordWithinNextXDays()
    {
        var inside = AccountAt(Now.AddDays(3));
        var outside = AccountAt(Now.AddDays(10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextXDays, 5);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task Next7Days_MatchesRecordWithinNext7Days()
    {
        var inside = AccountAt(Now.AddDays(5));
        var outside = AccountAt(Now.AddDays(10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.Next7Days);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextXWeeks_MatchesRecordWithinNextXWeeks()
    {
        var inside = AccountAt(Now.AddDays(10));
        var outside = AccountAt(Now.AddDays(30));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextXWeeks, 2);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextXMonths_MatchesRecordWithinNextXMonths()
    {
        var inside = AccountAt(Now.AddMonths(2));
        var outside = AccountAt(Now.AddMonths(6));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextXMonths, 3);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextXYears_MatchesRecordWithinNextXYears()
    {
        var inside = AccountAt(Now.AddYears(1));
        var outside = AccountAt(Now.AddYears(5));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextXYears, 2);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    #endregion

    #region OlderThanX Operators

    [Test]
    public async Task OlderThanXMinutes_MatchesRecordOlderThanXMinutes()
    {
        var old = AccountAt(Now.AddMinutes(-30));
        var recent = AccountAt(Now.AddMinutes(-5));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXMinutes, 15);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    [Test]
    public async Task OlderThanXHours_MatchesRecordOlderThanXHours()
    {
        var old = AccountAt(Now.AddHours(-10));
        var recent = AccountAt(Now.AddHours(-1));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXHours, 5);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    [Test]
    public async Task OlderThanXDays_MatchesRecordOlderThanXDays()
    {
        var old = AccountAt(Now.AddDays(-20));
        var recent = AccountAt(Now.AddDays(-2));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXDays, 10);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    [Test]
    public async Task OlderThanXWeeks_MatchesRecordOlderThanXWeeks()
    {
        var old = AccountAt(Now.AddDays(-30));
        var recent = AccountAt(Now.AddDays(-5));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXWeeks, 3);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    [Test]
    public async Task OlderThanXMonths_MatchesRecordOlderThanXMonths()
    {
        var old = AccountAt(Now.AddMonths(-6));
        var recent = AccountAt(Now.AddMonths(-1));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXMonths, 3);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    [Test]
    public async Task OlderThanXYears_MatchesRecordOlderThanXYears()
    {
        var old = AccountAt(Now.AddYears(-5));
        var recent = AccountAt(Now.AddYears(-1));
        var sut = CreateService(old, recent);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OlderThanXYears, 3);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(old.Id);
    }

    #endregion

    #region Fixed Period Operators (This/Last/Next Year/Month/Week)

    [Test]
    public async Task ThisYear_MatchesRecordInCurrentYear()
    {
        var inside = AccountAt(new DateTime(2024, 3, 1));
        var outside = AccountAt(new DateTime(2023, 6, 1));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.ThisYear);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastYear_MatchesRecordInPreviousYear()
    {
        var inside = AccountAt(new DateTime(2023, 6, 1));
        var outside = AccountAt(new DateTime(2024, 3, 1));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastYear);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextYear_MatchesRecordInNextYear()
    {
        var inside = AccountAt(new DateTime(2025, 3, 1));
        var outside = AccountAt(new DateTime(2024, 3, 1));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextYear);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task ThisMonth_MatchesRecordInCurrentMonth()
    {
        var inside = AccountAt(new DateTime(2024, 6, 10));
        var outside = AccountAt(new DateTime(2024, 5, 10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.ThisMonth);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastMonth_MatchesRecordInPreviousMonth()
    {
        var inside = AccountAt(new DateTime(2024, 5, 15));
        var outside = AccountAt(new DateTime(2024, 6, 10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastMonth);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextMonth_MatchesRecordInNextMonth()
    {
        var inside = AccountAt(new DateTime(2024, 7, 10));
        var outside = AccountAt(new DateTime(2024, 6, 10));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextMonth);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task ThisWeek_MatchesRecordInCurrentWeek()
    {
        // Now = 2024-06-12 (Wednesday). Same day should always be in "this week".
        var inside = AccountAt(new DateTime(2024, 6, 12));
        var outside = AccountAt(new DateTime(2024, 5, 1)); // clearly different week
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.ThisWeek);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task LastWeek_MatchesRecordInPreviousWeek()
    {
        // Now = 2024-06-12 (Wednesday). 7 days before (Jun 5) should be "last week".
        var inside = AccountAt(Now.AddDays(-7).Date);
        var outside = AccountAt(Now.Date); // this week
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastWeek);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    [Test]
    public async Task NextWeek_MatchesRecordInNextWeek()
    {
        // Now = 2024-06-12 (Wednesday). 7 days after (Jun 19) should be "next week".
        var inside = AccountAt(Now.AddDays(7).Date);
        var outside = AccountAt(Now.Date); // this week
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NextWeek);

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inside.Id);
    }

    #endregion

    #region OnOrAfter / OnOrBefore

    [Test]
    public async Task OnOrAfter_MatchesRecordOnOrAfterDate()
    {
        var onDate = AccountAt(new DateTime(2024, 6, 15));
        var after = AccountAt(new DateTime(2024, 6, 20));
        var before = AccountAt(new DateTime(2024, 6, 10));
        var sut = CreateService(onDate, after, before);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OnOrAfter, new DateTime(2024, 6, 15));

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(2);
    }

    [Test]
    public async Task OnOrBefore_MatchesRecordOnOrBeforeDate()
    {
        var onDate = AccountAt(new DateTime(2024, 6, 15));
        var before = AccountAt(new DateTime(2024, 6, 10));
        var after = AccountAt(new DateTime(2024, 6, 20));
        var sut = CreateService(onDate, before, after);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.OnOrBefore, new DateTime(2024, 6, 15));

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(2);
    }

    #endregion

    #region NotBetween / NotIn

    [Test]
    public async Task NotBetween_ExcludesRecordsBetweenValues()
    {
        var inside = AccountAt(new DateTime(2024, 3, 15));
        var outside = AccountAt(new DateTime(2024, 8, 15));
        var sut = CreateService(inside, outside);

        var query = new QueryExpression(Account.EntityLogicalName);
        query.Criteria.AddCondition(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.NotBetween,
            new DateTime(2024, 1, 1), new DateTime(2024, 6, 30));

        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(outside.Id);
    }

    [Test]
    public async Task NotIn_ExcludesRecordsInList()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var query = new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name)
        };
        query.Criteria.AddCondition(Account.LogicalNames.Name, ConditionOperator.NotIn, "A Corp");

        var result = sut.RetrieveMultiple(query);
        // TestData has 2 accounts: "A Corp" excluded, "B Corp" remains
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].GetAttributeValue<string>("name")).IsEqualTo("B Corp");
    }

    #endregion
}
