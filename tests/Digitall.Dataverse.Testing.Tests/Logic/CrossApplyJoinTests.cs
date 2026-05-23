// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for JoinOperator.MatchFirstRowUsingCrossApply which returns only the first
/// matching linked entity per parent row (CROSS APPLY TOP 1 semantics).
/// This feature is NOT YET IMPLEMENTED — these tests document the expected behavior.
/// </summary>
public class CrossApplyJoinTests
{
    [Test]
    public async Task CrossApply_ReturnsSingleLinkedRowPerParent()
    {
        // corpB has 2 contacts; with CrossApply only 1 should be returned per parent
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.MatchFirstRowUsingCrossApply)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // corpB has 2 contacts but CrossApply should return only 1 row for corpB
        var corpBRows = result.Entities
            .Where(e => e.Id == Guid.Parse("00000000-0000-0000-0001-000000000002"))
            .ToList();

        await Assert.That(corpBRows).Count().IsEqualTo(1);
    }

    [Test]
    public async Task CrossApply_DoesNotReturnParentsWithNoLinkedRecords()
    {
        // corpA has no contacts - should be excluded (like Inner join)
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.MatchFirstRowUsingCrossApply)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        var corpARows = result.Entities
            .Where(e => e.Id == Guid.Parse("00000000-0000-0000-0001-000000000001"))
            .ToList();

        await Assert.That(corpARows).IsEmpty();
    }

    [Test]
    public async Task CrossApply_ProjectsLinkedEntityColumns()
    {
        // Unlike Any/Exists/In, CrossApply DOES return linked columns
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.MatchFirstRowUsingCrossApply)
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
