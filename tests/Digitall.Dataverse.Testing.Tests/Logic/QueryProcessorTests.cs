// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for QueryProcessor orchestration: paging, TopCount, Distinct, ordering, and column projection.
/// </summary>
public class QueryProcessorTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    #region TopCount

    [Test]
    public async Task TopCount_LimitsResults()
    {
        for (var i = 0; i < 10; i++)
            _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = $"Account {i}" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true), TopCount = 3 };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(3);
    }

    [Test]
    public async Task TopCount_FewerRecordsThanTop_ReturnsAll()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Only" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true), TopCount = 100 };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
    }

    #endregion

    #region Paging

    [Test]
    public async Task Paging_FirstPage_ReturnsCorrectCount()
    {
        for (var i = 0; i < 5; i++)
            _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = $"A{i}" });

        var qe = new QueryExpression("account")
        {
            ColumnSet = new ColumnSet(true),
            PageInfo = new PagingInfo { PageNumber = 1, Count = 2 }
        };

        var result = _sut.RetrieveMultiple(qe);
        await Assert.That(result.Entities).Count().IsEqualTo(2);
        await Assert.That(result.MoreRecords).IsTrue();
    }

    [Test]
    public async Task Paging_LastPage_MoreRecordsFalse()
    {
        for (var i = 0; i < 3; i++)
            _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = $"A{i}" });

        var qe = new QueryExpression("account")
        {
            ColumnSet = new ColumnSet(true),
            PageInfo = new PagingInfo { PageNumber = 2, Count = 2 }
        };

        var result = _sut.RetrieveMultiple(qe);
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.MoreRecords).IsFalse();
    }

    #endregion

    #region Distinct

    [Test]
    public async Task Distinct_RemovesDuplicateProjections()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Berlin" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Berlin" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Munich" });

        var qe = new QueryExpression("account")
        {
            ColumnSet = new ColumnSet("address1_city"),
            Distinct = true
        };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    #endregion

    #region Ordering

    [Test]
    public async Task OrderBy_Ascending_SortsCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddOrder("name", OrderType.Ascending);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results[0]["name"]).IsEqualTo("A");
        await Assert.That(results[1]["name"]).IsEqualTo("B");
        await Assert.That(results[2]["name"]).IsEqualTo("C");
    }

    [Test]
    public async Task OrderBy_Descending_SortsCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddOrder("name", OrderType.Descending);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results[0]["name"]).IsEqualTo("C");
        await Assert.That(results[1]["name"]).IsEqualTo("B");
        await Assert.That(results[2]["name"]).IsEqualTo("A");
    }

    [Test]
    public async Task OrderBy_MultipleColumns_ThenByApplied()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Berlin", ["name"] = "B" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Berlin", ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["address1_city"] = "Munich", ["name"] = "Z" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.AddOrder("address1_city", OrderType.Ascending);
        qe.AddOrder("name", OrderType.Ascending);

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results[0]["name"]).IsEqualTo("A");
        await Assert.That(results[1]["name"]).IsEqualTo("B");
        await Assert.That(results[2]["name"]).IsEqualTo("Z");
    }

    #endregion
    #region Root-level OrderExpression.EntityName

    /// <summary>
    /// A root-level OrderExpression with EntityName set (e.g. produced by FetchXML
    /// &lt;order entityname="alias" attribute="field" /&gt;) should sort by the aliased
    /// linked-entity attribute rather than a bare root attribute.
    /// </summary>
    [Test]
    public async Task RootOrder_WithEntityName_Ascending_SortsByLinkedAttribute()
    {
        // Arrange: contacts linked to accounts; order contacts by linked account name ascending
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        var accountId3 = Guid.NewGuid();

        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "C-Account" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "A-Account" });
        _sut.Add(new Entity("account") { Id = accountId3, ["name"] = "B-Account" });

        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "Contact-C" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["fullname"] = "Contact-A" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId3, ["fullname"] = "Contact-B" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
        link.EntityAlias = "acc";
        link.Columns = new ColumnSet("name");

        // Root-level order that references the linked entity alias
        qe.Orders.Add(new OrderExpression { AttributeName = "name", EntityName = "acc", OrderType = OrderType.Ascending });

        var results = _sut.RetrieveMultiple(qe).Entities;

        // Ascending by linked account name: Contact-A, Contact-B, Contact-C
        await Assert.That(results).Count().IsEqualTo(3);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Contact-A");
        await Assert.That(results[1]["fullname"]).IsEqualTo("Contact-B");
        await Assert.That(results[2]["fullname"]).IsEqualTo("Contact-C");
    }

    [Test]
    public async Task RootOrder_WithEntityName_Descending_SortsByLinkedAttributeDescending()
    {
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        var accountId3 = Guid.NewGuid();

        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "C-Account" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "A-Account" });
        _sut.Add(new Entity("account") { Id = accountId3, ["name"] = "B-Account" });

        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "Contact-C" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["fullname"] = "Contact-A" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId3, ["fullname"] = "Contact-B" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };
        var link = qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
        link.EntityAlias = "acc";
        link.Columns = new ColumnSet("name");

        qe.Orders.Add(new OrderExpression { AttributeName = "name", EntityName = "acc", OrderType = OrderType.Descending });

        var results = _sut.RetrieveMultiple(qe).Entities;

        // Descending: Contact-C, Contact-B, Contact-A
        await Assert.That(results).Count().IsEqualTo(3);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Contact-C");
        await Assert.That(results[1]["fullname"]).IsEqualTo("Contact-B");
        await Assert.That(results[2]["fullname"]).IsEqualTo("Contact-A");
    }

    [Test]
    public async Task RootOrder_WithEntityName_TopCount_ReturnsCorrectRow()
    {
        // TopCount = 1 + descending order on linked attribute → contact linked to alphabetically-last account
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        var accountId3 = Guid.NewGuid();

        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "B-Account" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "A-Account" });
        _sut.Add(new Entity("account") { Id = accountId3, ["name"] = "C-Account" });

        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "Contact-B" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["fullname"] = "Contact-A" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId3, ["fullname"] = "Contact-C" });

        var qe = new QueryExpression("contact") { ColumnSet = new ColumnSet(true), TopCount = 1 };
        var link = qe.AddLink("account", "parentcustomerid", "accountid", JoinOperator.Inner);
        link.EntityAlias = "acc";
        link.Columns = new ColumnSet("name");

        qe.Orders.Add(new OrderExpression { AttributeName = "name", EntityName = "acc", OrderType = OrderType.Descending });

        var results = _sut.RetrieveMultiple(qe).Entities;

        // Only the contact linked to "C-Account" (alphabetically last) should be returned
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Contact-C");
    }

    [Test]
    public async Task FetchXml_RootOrder_WithEntityName_SortsByLinkedAttribute()
    {
        // FetchXML path: <order entityname="acc" attribute="name" /> at root entity level
        var accountId1 = Guid.NewGuid();
        var accountId2 = Guid.NewGuid();
        var accountId3 = Guid.NewGuid();

        _sut.Add(new Entity("account") { Id = accountId1, ["name"] = "C-Account" });
        _sut.Add(new Entity("account") { Id = accountId2, ["name"] = "A-Account" });
        _sut.Add(new Entity("account") { Id = accountId3, ["name"] = "B-Account" });

        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId1, ["fullname"] = "Contact-C" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId2, ["fullname"] = "Contact-A" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId3, ["fullname"] = "Contact-B" });

        var fetchXml = """
            <fetch>
              <entity name="contact">
                <attribute name="fullname" />
                <link-entity name="account" from="accountid" to="parentcustomerid" link-type="inner" alias="acc">
                  <attribute name="name" />
                </link-entity>
                <order entityname="acc" attribute="name" descending="false" />
              </entity>
            </fetch>
            """;

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;

        // Ascending by linked account name: Contact-A, Contact-B, Contact-C
        await Assert.That(results).Count().IsEqualTo(3);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Contact-A");
        await Assert.That(results[1]["fullname"]).IsEqualTo("Contact-B");
        await Assert.That(results[2]["fullname"]).IsEqualTo("Contact-C");
    }

    #endregion


    #region Column Projection

    [Test]
    public async Task ColumnSet_SpecificColumns_OnlyThoseReturned()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Test", ["address1_city"] = "Berlin", ["revenue"] = 100 });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet("name") };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results[0].Contains("name")).IsTrue();
        await Assert.That(results[0].Contains("address1_city")).IsFalse();
        await Assert.That(results[0].Contains("revenue")).IsFalse();
    }

    [Test]
    public async Task ColumnSet_AllColumns_AllReturned()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Test", ["address1_city"] = "Berlin" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results[0].Contains("name")).IsTrue();
        await Assert.That(results[0].Contains("address1_city")).IsTrue();
    }

    [Test]
    public async Task ColumnSet_NoColumns_EmptyAttributes()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Test" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(false) };

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].Contains("name")).IsFalse();
    }

    #endregion

    #region Empty Results

    [Test]
    public async Task NoMatchingRecords_ReturnsEmptyCollection()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "X" });

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "NonExisting");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).IsEmpty();
    }

    [Test]
    public async Task EmptyTable_ReturnsEmptyCollection()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });
        // query different entity type not in state... let's query account with impossible filter
        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet(true) };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "impossible");

        var results = _sut.RetrieveMultiple(qe).Entities;
        await Assert.That(results).IsEmpty();
    }

    #endregion
}
