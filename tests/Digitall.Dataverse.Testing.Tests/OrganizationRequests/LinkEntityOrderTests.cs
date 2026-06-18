// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

/// <summary>
/// Tests for ordering by <see cref="LinkEntity.Orders"/> (ordering by linked-entity attributes),
/// matching real Dataverse / FetchXml behavior where <c>&lt;order&gt;</c> can appear inside a
/// <c>&lt;link-entity&gt;</c>.
/// </summary>
public class LinkEntityOrderTests
{
    private static readonly Guid s_accountXId = Guid.Parse("00000000-0000-0000-00a0-000000000001");
    private static readonly Guid s_accountYId = Guid.Parse("00000000-0000-0000-00a0-000000000002");

    private static readonly Guid s_contactXId = Guid.Parse("00000000-0000-0000-00c0-000000000001");
    private static readonly Guid s_contactYId = Guid.Parse("00000000-0000-0000-00c0-000000000002");

    /// <summary>
    /// Two accounts, each with exactly one contact carrying a numeric column.
    /// AccountX -> contact (numberofchildren = 5), AccountY -> contact (numberofchildren = 10).
    /// </summary>
    private static IEnumerable<Entity> SingleLinkData()
    {
        var accountX = new Account(s_accountXId) { Name = "Account X" };
        var accountY = new Account(s_accountYId) { Name = "Account Y" };

        var contactX = new Contact(s_contactXId)
        {
            ParentCustomerId = accountX.ToEntityReference(),
            NumberOfChildren = 5
        };
        var contactY = new Contact(s_contactYId)
        {
            ParentCustomerId = accountY.ToEntityReference(),
            NumberOfChildren = 10
        };

        return [accountX, accountY, contactX, contactY];
    }

    private static LinkEntity ContactLink(string? alias)
    {
        var link = new LinkEntity(
            linkFromEntityName: Account.EntityLogicalName,
            linkToEntityName: Contact.EntityLogicalName,
            linkFromAttributeName: Account.LogicalNames.AccountId,
            linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
            joinOperator: JoinOperator.Inner)
        {
            Columns = new ColumnSet(Contact.LogicalNames.NumberOfChildren)
        };

        if (alias != null)
        {
            link.EntityAlias = alias;
        }

        return link;
    }

    #region Single inner link order + TopCount

    [Test]
    public async Task SingleLinkOrder_Descending_WithTopCount1_ReturnsRowWithMaxLinkedValue()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(SingleLinkData());

        var link = ContactLink("classification");
        link.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Descending));

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            TopCount = 1,
            LinkEntities = { link }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(s_accountYId);
    }

    [Test]
    public async Task SingleLinkOrder_Ascending_WithTopCount1_ReturnsRowWithMinLinkedValue()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(SingleLinkData());

        var link = ContactLink("classification");
        link.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Ascending));

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            TopCount = 1,
            LinkEntities = { link }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(s_accountXId);
    }

    #endregion

    #region Aliased vs non-aliased link

    [Test]
    public async Task SingleLinkOrder_WithoutEntityAlias_StillOrdersByLinkedValue()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(SingleLinkData());

        // No EntityAlias set -> the engine assigns one internally; ordering must still work.
        var link = ContactLink(alias: null);
        link.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Descending));

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            LinkEntities = { link }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(2);
        await Assert.That(result.Entities[0].Id).IsEqualTo(s_accountYId);
        await Assert.That(result.Entities[1].Id).IsEqualTo(s_accountXId);
    }

    #endregion

    #region Combined root + link orders (precedence / tie-breaking)

    [Test]
    public async Task CombinedRootAndLinkOrder_RootOrderTakesPrecedence_LinkOrderBreaksTies()
    {
        var sut = new FakeOrganizationService();

        // account1 and account2 share the same name "A" -> tie broken by linked numberofchildren desc.
        var account1Id = Guid.Parse("00000000-0000-0000-00b0-000000000001");
        var account2Id = Guid.Parse("00000000-0000-0000-00b0-000000000002");
        var account3Id = Guid.Parse("00000000-0000-0000-00b0-000000000003");

        var account1 = new Account(account1Id) { Name = "A" };
        var account2 = new Account(account2Id) { Name = "A" };
        var account3 = new Account(account3Id) { Name = "B" };

        var contact1 = new Contact(Guid.NewGuid()) { ParentCustomerId = account1.ToEntityReference(), NumberOfChildren = 1 };
        var contact2 = new Contact(Guid.NewGuid()) { ParentCustomerId = account2.ToEntityReference(), NumberOfChildren = 9 };
        var contact3 = new Contact(Guid.NewGuid()) { ParentCustomerId = account3.ToEntityReference(), NumberOfChildren = 5 };

        sut.AddRange([account1, account2, account3, contact1, contact2, contact3]);

        var link = ContactLink("classification");
        link.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Descending));

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Orders = { new OrderExpression(Account.LogicalNames.Name, OrderType.Ascending) },
            LinkEntities = { link }
        });

        // Root order (name asc) first: A's before B. Within "A" tie, link order (children desc):
        // account2 (9) before account1 (1). Then account3 ("B").
        await Assert.That(result.Entities).Count().IsEqualTo(3);
        await Assert.That(result.Entities[0].Id).IsEqualTo(account2Id);
        await Assert.That(result.Entities[1].Id).IsEqualTo(account1Id);
        await Assert.That(result.Entities[2].Id).IsEqualTo(account3Id);
    }

    #endregion

    #region Nested link order (two levels deep)

    [Test]
    public async Task NestedLinkOrder_OrdersByAttributeTwoLinkLevelsDeep()
    {
        var sut = new FakeOrganizationService();

        // Account self-relationship chain: child -> parent -> grandparent (via parentaccountid).
        var grandA = new Account(Guid.Parse("00000000-0000-0000-00d0-000000000001")) { Name = "Grand A", NumberOfEmployees = 10 };
        var grandB = new Account(Guid.Parse("00000000-0000-0000-00d0-000000000002")) { Name = "Grand B", NumberOfEmployees = 20 };

        var parentA = new Account(Guid.Parse("00000000-0000-0000-00d0-000000000011")) { Name = "Parent A", ParentAccountId = grandA.ToEntityReference() };
        var parentB = new Account(Guid.Parse("00000000-0000-0000-00d0-000000000012")) { Name = "Parent B", ParentAccountId = grandB.ToEntityReference() };

        var childAId = Guid.Parse("00000000-0000-0000-00d0-000000000021");
        var childBId = Guid.Parse("00000000-0000-0000-00d0-000000000022");
        var childA = new Account(childAId) { Name = "Child A", ParentAccountId = parentA.ToEntityReference() };
        var childB = new Account(childBId) { Name = "Child B", ParentAccountId = parentB.ToEntityReference() };

        sut.AddRange([grandA, grandB, parentA, parentB, childA, childB]);

        var grandLink = new LinkEntity(
            linkFromEntityName: Account.EntityLogicalName,
            linkToEntityName: Account.EntityLogicalName,
            linkFromAttributeName: Account.LogicalNames.ParentAccountId,
            linkToAttributeName: Account.LogicalNames.AccountId,
            joinOperator: JoinOperator.Inner)
        {
            EntityAlias = "grandparent",
            Columns = new ColumnSet(Account.LogicalNames.NumberOfEmployees),
            Orders = { new OrderExpression(Account.LogicalNames.NumberOfEmployees, OrderType.Descending) }
        };

        var parentLink = new LinkEntity(
            linkFromEntityName: Account.EntityLogicalName,
            linkToEntityName: Account.EntityLogicalName,
            linkFromAttributeName: Account.LogicalNames.ParentAccountId,
            linkToAttributeName: Account.LogicalNames.AccountId,
            joinOperator: JoinOperator.Inner)
        {
            EntityAlias = "parent",
            LinkEntities = { grandLink }
        };

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            TopCount = 1,
            LinkEntities = { parentLink }
        });

        // Only childA and childB survive both inner joins. Ordered by grandparent employees desc,
        // childB (grand B = 20) comes first.
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(childBId);
    }

    #endregion

    #region FetchXml parity

    [Test]
    public async Task FetchXml_LinkEntityOrder_MatchesQueryExpressionResult()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(SingleLinkData());

        const string fetchXml = """
            <fetch top="1">
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="parentcustomerid" to="accountid" alias="classification" link-type="inner">
                  <attribute name="numberofchildren" />
                  <order attribute="numberofchildren" descending="true" />
                </link-entity>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(s_accountYId);
    }

    #endregion
}

