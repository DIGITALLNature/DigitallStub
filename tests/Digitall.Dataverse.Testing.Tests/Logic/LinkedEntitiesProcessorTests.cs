// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Expanded tests for LinkedEntitiesProcessor covering all JoinOperator types,
/// nested linked entities, auto-alias generation, and empty collection handling.
/// </summary>
public class LinkedEntitiesProcessorTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    #region Inner Join

    [Test]
    public async Task InnerJoin_OnlyMatchingRecordsReturned()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Linked" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Unlinked" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Linked");
    }

    #endregion

    #region Left Outer Join

    [Test]
    public async Task LeftOuterJoin_AllOuterRecordsReturned()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Linked" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Unlinked" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.LeftOuter);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(2);
    }

    [Test]
    public async Task LeftOuterJoin_NoMatchingInner_StillReturnsOuter()
    {
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Orphan" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.LeftOuter);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
    }

    #endregion

    #region Natural Join

    [Test]
    public async Task NaturalJoin_BehavesLikeInnerJoin()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Child" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Orphan" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Natural);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Child");
    }

    #endregion

    #region Exists (Any) Join

    [Test]
    public async Task AnyJoin_ReturnsOuterWhereInnerExists()
    {
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "WithContacts" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "NoContacts" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "C1" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.Any);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("WithContacts");
    }

    [Test]
    public async Task NotAnyJoin_ReturnsOuterWhereInnerNotExists()
    {
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "WithContacts" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "NoContacts" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "C1" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.NotAny);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("NoContacts");
    }

    #endregion

    #region All Join

    [Test]
    public async Task AllJoin_ReturnsOuterWhereNoInnerMatchesCriteria()
    {
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        var accountId3 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "AllInactive" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "MixedStatus" });
        _sut.Add(new Entity("account") { Id = accountId3, ["name"] = "NoContacts" });
        // AllInactive: both contacts are inactive → no contact matches active=true
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["active"] = false });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["active"] = false });
        // MixedStatus: one active contact matches active=true
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["active"] = true });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["active"] = false });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.All);
        link.LinkCriteria.AddCondition("active", ConditionOperator.Equal, true);

        // All = "has linked records AND none match criteria"
        // Only AllInactive qualifies (has contacts but none match active=true)
        // NoContacts excluded (no linked records)
        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("AllInactive");
    }

    [Test]
    public async Task NotAllJoin_ReturnsOuterWhereSomeInnerMatchesCriteria()
    {
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "AllInactive" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "HasActiveContact" });
        // AllInactive: no contact matches active=true
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["active"] = false });
        // HasActiveContact: one contact matches active=true
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["active"] = true });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["active"] = false });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.NotAll);
        link.LinkCriteria.AddCondition("active", ConditionOperator.Equal, true);

        // NotAll = EXISTS(matching filtered inner) → at least one contact is active
        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("HasActiveContact");
    }

    #endregion

    #region MatchFirstRowUsingCrossApply

    [Test]
    public async Task CrossApply_ReturnsSingleMatchPerOuter()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "First" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Second" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.MatchFirstRowUsingCrossApply);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
    }

    [Test]
    public async Task CrossApply_NoInnerMatch_ExcludesOuter()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "NoContacts" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddLink("contact", "accountid", "parentcustomerid", JoinOperator.MatchFirstRowUsingCrossApply);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).IsEmpty();
    }

    #endregion

    #region Nested Link Entities

    [Test]
    public async Task NestedLinkEntity_ThreeLevelJoin()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Acme" });
        _sut.Add(new Entity("contact") { Id = contactId, ["parentcustomerid"] = accountId, ["fullname"] = "John" });
        _sut.Add(new Entity("annotation") { Id = Guid.NewGuid(), ["objectid"] = contactId, ["subject"] = "Follow up" });

        var qe = new QueryExpression("annotation") { ColumnSet = new ColumnSet(true) };
        var contactLink = qe.AddLink("contact", "objectid", "contactid", JoinOperator.Inner);
        var accountLink = contactLink.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
        accountLink.LinkCriteria.AddCondition("name", ConditionOperator.Equal, "Acme");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        await Assert.That(results[0]["subject"]).IsEqualTo("Follow up");
    }

    #endregion

    #region Auto-Alias Generation

    [Test]
    public async Task AutoAlias_NoExplicitAlias_GeneratesAutomatically()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
        link.Columns = new ColumnSet("name");
        // No explicit EntityAlias set

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);
        // Linked entity attributes should be aliased
        var hasAliasedValue = results[0].Attributes.Values.OfType<AliasedValue>().Any(a => a.Value?.ToString() == "Parent");
        await Assert.That(hasAliasedValue).IsTrue();
    }

    [Test]
    public async Task MultipleLinksToSameEntity_UniqueAliasesGenerated()
    {
        var contact1Id = Guid.NewGuid();
        var contact2Id = Guid.NewGuid();
        _sut.Add(new Entity("contact") { Id = contact1Id, ["fullname"] = "Primary" });
        _sut.Add(new Entity("contact") { Id = contact2Id, ["fullname"] = "Secondary" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["primarycontactid"] = contact1Id, ["secondarycontactid"] = contact2Id });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        var link1 = qe.AddLink("contact", "primarycontactid", "contactid", JoinOperator.LeftOuter);
        link1.Columns = new ColumnSet("fullname");
        link1.EntityAlias = "primary";
        var link2 = qe.AddLink("contact", "secondarycontactid", "contactid", JoinOperator.LeftOuter);
        link2.Columns = new ColumnSet("fullname");
        link2.EntityAlias = "secondary";

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).HasCount().EqualTo(1);

        var primaryName = results[0].GetAttributeValue<AliasedValue>("primary.fullname");
        var secondaryName = results[0].GetAttributeValue<AliasedValue>("secondary.fullname");
        await Assert.That(primaryName).IsNotNull();
        await Assert.That(secondaryName).IsNotNull();
        await Assert.That(primaryName!.Value).IsEqualTo("Primary");
        await Assert.That(secondaryName!.Value).IsEqualTo("Secondary");
    }

    #endregion
}
