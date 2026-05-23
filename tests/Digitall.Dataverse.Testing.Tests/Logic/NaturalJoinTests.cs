// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for JoinOperator.Natural which behaves like Inner join but only returns
/// one row when the equal-join produces identical attribute values.
/// </summary>
public class NaturalJoinTests
{
    [Test]
    public async Task NaturalJoin_BehavesLikeInnerJoin_ReturnsMatchingRows()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // Natural join should work like Inner - only include parents with matching linked entities
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Natural)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // corpA has no contacts → excluded; corpB has 2 contacts → included
        await Assert.That(result.Entities).IsNotEmpty();

        var corpARows = result.Entities
            .Where(e => e.Id == Guid.Parse("00000000-0000-0000-0001-000000000001"))
            .ToList();
        await Assert.That(corpARows).IsEmpty();
    }

    [Test]
    public async Task NaturalJoin_ProducesRowsForEachMatch()
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
                    JoinOperator.Natural)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // corpB has 2 contacts → should produce 2 rows
        var corpBRows = result.Entities
            .Where(e => e.Id == Guid.Parse("00000000-0000-0000-0001-000000000002"))
            .ToList();
        await Assert.That(corpBRows).Count().IsEqualTo(2);
    }

    [Test]
    public async Task NaturalJoin_ProjectsLinkedEntityColumns()
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
                    JoinOperator.Natural)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        await Assert.That(result.Entities).IsNotEmpty();
        var row = result.Entities[0];
        await Assert.That(row.Attributes.ContainsKey("c." + Contact.LogicalNames.FirstName)).IsTrue();
    }
}
