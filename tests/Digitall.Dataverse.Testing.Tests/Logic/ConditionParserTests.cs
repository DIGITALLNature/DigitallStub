// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for ConditionParser operator translations via RetrieveMultiple.
/// Each test isolates a specific ConditionOperator to verify correct expression tree generation.
/// </summary>
public class ConditionParserTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    #region Equality Operators

    [Test]
    public async Task Equal_FiltersExactMatch()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Match" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Other" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "Match");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Match");
    }

    [Test]
    public async Task NotEqual_ExcludesMatch()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Excluded" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Kept" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.NotEqual, "Excluded");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Kept");
    }

    #endregion

    #region Comparison Operators

    [Test]
    public async Task GreaterThan_FiltersCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 100 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 200 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 300 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.GreaterThan, 150);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(2);
    }

    [Test]
    public async Task LessThan_FiltersCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 100 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 200 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.LessThan, 150);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["score"]).IsEqualTo(100);
    }

    [Test]
    public async Task GreaterEqual_IncludesBoundary()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 100 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 200 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.GreaterEqual, 200);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["score"]).IsEqualTo(200);
    }

    [Test]
    public async Task LessEqual_IncludesBoundary()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 100 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 200 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.LessEqual, 100);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["score"]).IsEqualTo(100);
    }

    #endregion

    #region String Operators

    [Test]
    public async Task Like_WithWildcards_FiltersCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Like, "%Ltd");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Contoso Ltd");
    }

    [Test]
    public async Task BeginsWith_FiltersPrefix()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.BeginsWith, "Con");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Contoso Ltd");
    }

    [Test]
    public async Task EndsWith_FiltersSuffix()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.EndsWith, "Inc");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Fabrikam Inc");
    }

    [Test]
    public async Task Contains_FiltersSubstring()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Contains, "oso");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Contoso Ltd");
    }

    [Test]
    public async Task DoesNotContain_ExcludesSubstring()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.DoesNotContain, "oso");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Fabrikam Inc");
    }

    #endregion

    #region Collection Operators

    [Test]
    public async Task In_FiltersMultipleValues()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 1 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 2 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 3 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("customfield1", ConditionOperator.In, 1, 3);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(2);
    }

    [Test]
    public async Task NotIn_ExcludesMultipleValues()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 1 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 2 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["customfield1"] = 3 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("customfield1", ConditionOperator.NotIn, 1, 3);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["customfield1"]).IsEqualTo(2);
    }

    [Test]
    public async Task Between_FiltersRange()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 50 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 150 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 250 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.Between, 100, 200);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["score"]).IsEqualTo(150);
    }

    [Test]
    public async Task NotBetween_ExcludesRange()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 50 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 150 });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["score"] = 250 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("score", ConditionOperator.NotBetween, 100, 200);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(2);
    }

    #endregion

    #region Null Operators

    [Test]
    public async Task Null_FiltersNullValues()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "HasValue" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() }); // no "name" attribute

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Null);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0].Contains("name")).IsFalse();
    }

    [Test]
    public async Task NotNull_FiltersNonNullValues()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "HasValue" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.NotNull);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("HasValue");
    }

    #endregion

    #region EntityReference Operators

    [Test]
    public async Task Equal_EntityReference_MatchesById()
    {
        var refId = Guid.NewGuid();
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = new EntityReference("account", refId) });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = new EntityReference("account", Guid.NewGuid()) });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("parentcustomerid", ConditionOperator.Equal, refId);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
    }

    #endregion

    #region OptionSetValue Operators

    [Test]
    public async Task Equal_OptionSetValue_MatchesByIntValue()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["statecode"] = new OptionSetValue(0) });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["statecode"] = new OptionSetValue(1) });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("statecode", ConditionOperator.Equal, 0);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
    }

    [Test]
    public async Task In_OptionSetValues_MatchesMultiple()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["statecode"] = new OptionSetValue(0) });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["statecode"] = new OptionSetValue(1) });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["statecode"] = new OptionSetValue(2) });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("statecode", ConditionOperator.In, 0, 2);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(2);
    }

    #endregion

    #region Query Reuse (idempotency)

    [Test]
    public async Task QueryReuse_SameQueryExecutedTwice_ProducesSameResults()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Test" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Other" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "Test");

        var results1 = _sut.RetrieveMultiple(qe).Entities;
        var results2 = _sut.RetrieveMultiple(qe).Entities;

        await Assert.That(results1).HasCount().EqualTo(1);
        await Assert.That(results2).HasCount().EqualTo(1);
        await Assert.That(results1[0]["name"]).IsEqualTo(results2[0]["name"]);
    }

    #endregion
}
