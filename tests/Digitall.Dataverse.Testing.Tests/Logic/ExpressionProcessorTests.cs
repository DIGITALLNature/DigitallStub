// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for ExpressionProcessor filter expression building.
/// Covers And/Or/Not combinations, nested filters, and alias prefix idempotency.
/// </summary>
public class ExpressionProcessorTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    #region Logical Operators

    [Test]
    public async Task AndFilter_AllConditionsMustMatch()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A", ["address1_city"] = "Berlin" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A", ["address1_city"] = "Munich" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B", ["address1_city"] = "Berlin" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.FilterOperator = LogicalOperator.And;
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "A");
        qe.Criteria.AddCondition("address1_city", ConditionOperator.Equal, "Berlin");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("A");
        await Assert.That(results[0]["address1_city"]).IsEqualTo("Berlin");
    }

    [Test]
    public async Task OrFilter_AnyConditionCanMatch()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.FilterOperator = LogicalOperator.Or;
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "A");
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "C");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    #endregion

    #region Nested Filters

    [Test]
    public async Task NestedFilters_AndWithOrSubfilter()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A", ["address1_city"] = "Berlin", ["active"] = true });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B", ["address1_city"] = "Munich", ["active"] = true });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C", ["address1_city"] = "Berlin", ["active"] = false });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.FilterOperator = LogicalOperator.And;
        qe.Criteria.AddCondition("active", ConditionOperator.Equal, true);

        var subFilter = new FilterExpression(LogicalOperator.Or);
        subFilter.AddCondition("address1_city", ConditionOperator.Equal, "Berlin");
        subFilter.AddCondition("address1_city", ConditionOperator.Equal, "Munich");
        qe.Criteria.AddFilter(subFilter);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    [Test]
    public async Task NestedFilters_MultipleDepths()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A", ["x"] = 1, ["y"] = 10 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B", ["x"] = 2, ["y"] = 20 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C", ["x"] = 1, ["y"] = 20 });
        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true), Criteria = { FilterOperator = LogicalOperator.Or } };
        qe.Criteria.FilterOperator = LogicalOperator.Or;

        var andFilter1 = new FilterExpression(LogicalOperator.And);
        andFilter1.AddCondition("x", ConditionOperator.Equal, 1);
        andFilter1.AddCondition("y", ConditionOperator.Equal, 10);
        qe.Criteria.AddFilter(andFilter1);

        var andFilter2 = new FilterExpression(LogicalOperator.And);
        andFilter2.AddCondition("x", ConditionOperator.Equal, 2);
        andFilter2.AddCondition("y", ConditionOperator.Equal, 20);
        qe.Criteria.AddFilter(andFilter2);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    #endregion

    #region Multiple Conditions on Same Attribute

    [Test]
    public async Task MultipleConditions_SameAttribute_BothApplied()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 50 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 150 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 250 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.GreaterThan, 100);
        qe.Criteria.AddCondition("score", ConditionOperator.LessThan, 200);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["score"]).IsEqualTo(150);
    }

    #endregion

    #region Empty Filters

    [Test]
    public async Task EmptyFilter_ReturnsAllRecords()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    [Test]
    public async Task EmptySubFilter_IsIgnored()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "A");
        qe.Criteria.AddFilter(new FilterExpression(LogicalOperator.And)); // empty sub-filter

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
    }

    #endregion

    #region LinkedEntity Filters (alias prefix idempotency)

    [Test]
    public async Task LinkedEntityFilter_QueryReuse_IdempotentAliasPrefixing()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["accountid"] = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Child" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("account", "parentcustomerid", "accountid");
        link.EntityAlias = "acc";
        link.LinkCriteria.AddCondition("name", ConditionOperator.Equal, "Parent");

        var results1 = _sut.RetrieveMultiple(qe).Entities;
        var results2 = _sut.RetrieveMultiple(qe).Entities;

        await Assert.That(results1).Count().IsEqualTo(1);
        await Assert.That(results2).Count().IsEqualTo(1);
    }

    #endregion
}
