// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

/// <summary>
/// Tests for <see cref="FilterExpression.AnyAllFilterLinkEntity"/> — EXISTS/NOT EXISTS subqueries
/// on the filter level (not as top-level LinkEntities on QueryExpression).
/// </summary>
public class AnyAllFilterLinkEntityTests
{
    private static readonly Guid CorpAId = Guid.Parse("00000000-0000-0000-0001-000000000001");
    private static readonly Guid CorpBId = Guid.Parse("00000000-0000-0000-0001-000000000002");

    #region JoinOperator.Any via AnyAllFilterLinkEntity

    /// <summary>
    /// AnyAllFilterLinkEntity with JoinOperator.Any should return accounts
    /// that have at least one contact with FirstName = "John B".
    /// Only corpB has such a contact.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_Any_ReturnsParentWithMatchingLinkedRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.Any)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// AnyAllFilterLinkEntity with JoinOperator.Any combined with Or and another condition.
    /// Returns contacts that are EITHER primary contact of an account named "A Corp"
    /// OR whose own name starts with "John C".
    /// </summary>
    [Test]
    public async Task AnyAllFilter_Any_CombinedWithOrCondition()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.Or)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.Any)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                },
                // Also match accounts with name "A Corp"
                Conditions =
                {
                    new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, "A Corp")
                }
            }
        });

        // corpA matches the name condition, corpB matches the exists condition
        await Assert.That(result.Entities).Count().IsEqualTo(2);
    }

    #endregion

    #region JoinOperator.NotAny via AnyAllFilterLinkEntity

    /// <summary>
    /// AnyAllFilterLinkEntity with JoinOperator.NotAny returns accounts that
    /// do NOT have any contacts with FirstName = "John B".
    /// corpB has "John B" → excluded. corpA has no such contact → included.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_NotAny_ReturnsParentWithoutMatchingLinkedRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.NotAny)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        // corpA has no contacts pointing to it at all → NOT EXISTS is true → included
        // corpB has "John B" → EXISTS is true → NOT EXISTS is false → excluded
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpAId);
    }

    #endregion

    #region JoinOperator.NotAll via AnyAllFilterLinkEntity (equivalent to Any)

    [Test]
    public async Task AnyAllFilter_NotAll_BehavesLikeAny()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.NotAll)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        // NotAll = Any: corpB has "John B" contact → included
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    #endregion

    #region JoinOperator.All via AnyAllFilterLinkEntity

    /// <summary>
    /// JoinOperator.All: returns parents where linked records exist but NONE satisfy criteria.
    /// corpA: no contacts → excluded (must have at least one linked record).
    /// corpB: contacts exist, "John B" matches criteria → corpB excluded.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_All_ExcludesWhenLinkedRecordsMatchCriteria()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.All)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        // corpA: no contacts → excluded. corpB: "John B" matches → excluded.
        await Assert.That(result.Entities).IsEmpty();
    }

    /// <summary>
    /// JoinOperator.All with criteria that no linked record matches → parent included.
    /// corpB has contacts but none named "Nobody" → corpB included.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_All_IncludesWhenNoLinkedRecordMatchesCriteria()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.All)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "Nobody")
                        }
                    }
                }
            }
        });

        // corpA: no contacts → excluded. corpB: contacts exist but none match → included.
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    #endregion

    #region AnyAllFilterLinkEntity without criteria

    /// <summary>
    /// AnyAllFilterLinkEntity with Any and no LinkCriteria acts as a pure EXISTS join check.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_Any_NoCriteria_ReturnsParentsWithAnyLinkedRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.Any)
            }
        });

        // corpA: no contacts → excluded. corpB: has contacts → included.
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    #endregion

    #region No duplication

    /// <summary>
    /// AnyAllFilterLinkEntity must never produce duplicate parent rows,
    /// even when multiple linked records match.
    /// </summary>
    [Test]
    public async Task AnyAllFilter_Any_NoDuplicateParentRows()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // corpB has 2 contacts (conB, conC). Both match "starts with John".
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet("name"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                AnyAllFilterLinkEntity = new LinkEntity(
                    linkFromEntityName: Account.EntityLogicalName,
                    linkToEntityName: Contact.EntityLogicalName,
                    linkFromAttributeName: Account.LogicalNames.AccountId,
                    linkToAttributeName: Contact.LogicalNames.ParentCustomerId,
                    joinOperator: JoinOperator.Any)
                {
                    LinkCriteria = new FilterExpression(LogicalOperator.And)
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.BeginsWith, "John")
                        }
                    }
                }
            }
        });

        // corpB should appear exactly once even though 2 contacts match
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    #endregion
}
