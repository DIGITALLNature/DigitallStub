// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests verifying that LinkEntity.Columns is respected during join projection.
/// The implementation uses a two-phase approach: JoinAttributes merges all columns,
/// then ProjectLinkedEntitiesAttributes filters based on LinkEntity.Columns.
/// </summary>
public class LinkEntityColumnProjectionTests
{
    [Test]
    public async Task InnerJoin_OnlySpecifiedLinkEntityColumnsAreProjected()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // Request only FirstName from linked contact (not LastName)
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Inner)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        await Assert.That(result.Entities).IsNotEmpty();

        // Should have c.firstname
        var row = result.Entities[0];
        await Assert.That(row.Attributes.ContainsKey("c." + Contact.LogicalNames.FirstName)).IsTrue();

        // Should NOT have c.lastname because it was not in the Columns
        await Assert.That(row.Attributes.ContainsKey("c." + Contact.LogicalNames.LastName)).IsFalse();
    }

    [Test]
    public async Task LeftOuterJoin_OnlySpecifiedLinkEntityColumnsAreProjected()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.LeftOuter)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // corpB has contacts - should have aliased firstname but not lastname
        var corpBRow = result.Entities.First(e => e.Attributes.ContainsKey("c." + Contact.LogicalNames.FirstName));
        await Assert.That(corpBRow.Attributes.ContainsKey("c." + Contact.LogicalNames.LastName)).IsFalse();
    }

    [Test]
    public async Task InnerJoin_AllColumnsWhenColumnSetIsAll()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // When Columns = ColumnSet(true), all linked attributes should be projected
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Inner)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(true)
                }
            }
        });

        await Assert.That(result.Entities).IsNotEmpty();
        var row = result.Entities[0];
        // Should have both firstname and lastname
        await Assert.That(row.Attributes.ContainsKey("c." + Contact.LogicalNames.FirstName)).IsTrue();
        await Assert.That(row.Attributes.ContainsKey("c." + Contact.LogicalNames.LastName)).IsTrue();
    }

    [Test]
    public async Task InnerJoin_EmptyColumnSet_NoLinkedAttributesProjected()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // Empty ColumnSet (default) means no columns from the linked entity
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Inner)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet() // empty - no columns
                }
            }
        });

        await Assert.That(result.Entities).IsNotEmpty();
        var row = result.Entities[0];
        // Should NOT have any aliased contact attributes
        var aliasedKeys = row.Attributes.Keys.Where(k => k.StartsWith("c.")).ToList();
        await Assert.That(aliasedKeys).IsEmpty();
    }
}
