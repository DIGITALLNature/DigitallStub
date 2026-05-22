// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

/// <summary>
/// Tests for EXISTS-style semi-join operators: <see cref="JoinOperator.Any"/>,
/// <see cref="JoinOperator.NotAny"/>, <see cref="JoinOperator.Exists"/>.
/// These operators filter the outer entity based on the existence (or absence) of a matching
/// related entity, without projecting inner attributes and without duplicating outer rows.
/// </summary>
public class ExistsJoinTests
{
    // TestData.Default:
    //   corpA (id: ...0001): no contacts
    //   corpB (id: ...0002): conB ("John B") + conC ("John C")
    //   conA : no parentcustomerid

    private static readonly Guid CorpAId = Guid.Parse("00000000-0000-0000-0001-000000000001");
    private static readonly Guid CorpBId = Guid.Parse("00000000-0000-0000-0001-000000000002");

    #region JoinOperator.Any (QueryExpression)

    /// <summary>
    /// JoinOperator.Any must include only accounts that have at least one matching contact.
    /// corpA has none → excluded. corpB has two → included exactly once.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_ReturnsOnlyAccountsWithAtLeastOneRelatedEntity()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
            }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// JoinOperator.Any must NOT produce duplicate outer rows even when multiple inner
    /// entities match. corpB has two contacts but must appear exactly once.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_DoesNotDuplicateOuterRowsForMultipleMatches()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
            }
        });

        // corpB matches conB AND conC, but must only appear once
        var corpBRows = result.Entities.Where(e => e.Id == CorpBId).ToList();
        await Assert.That(corpBRows).Count().IsEqualTo(1);
    }

    /// <summary>
    /// JoinOperator.Any must not project any linked-entity attributes into the result rows.
    /// The result must contain only the outer entity's own attributes.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_DoesNotProjectLinkedEntityAttributes()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // No aliased contact attributes must appear in any result row
        await Assert.That(result.Entities.Any(e => e.Attributes.Keys.Any(k => k.StartsWith("c.")))).IsFalse();
    }

    /// <summary>
    /// JoinOperator.Any with LinkCriteria must apply the criteria as a subquery filter:
    /// only accounts that have a contact with FirstName = "John B" should be returned.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_WithLinkCriteria_AppliesSubqueryFilter()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
                {
                    LinkCriteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        // Only corpB has a contact named "John B"
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// JoinOperator.Any with LinkCriteria that matches no contacts must return no accounts.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_WithLinkCriteriaThatMatchesNone_ReturnsEmpty()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
                {
                    LinkCriteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "Nobody")
                        }
                    }
                }
            }
        });

        await Assert.That(result.Entities).IsEmpty();
    }

    #endregion

    #region JoinOperator.NotAny (QueryExpression)

    /// <summary>
    /// JoinOperator.NotAny must return only accounts that have NO matching contact.
    /// corpA has none → included. corpB has two → excluded.
    /// </summary>
    [Test]
    public async Task QueryExpression_NotAnyJoin_ReturnsOnlyAccountsWithNoRelatedEntity()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.NotAny)
            }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpAId);
    }

    /// <summary>
    /// JoinOperator.NotAny must not project any linked-entity attributes.
    /// </summary>
    [Test]
    public async Task QueryExpression_NotAnyJoin_DoesNotProjectLinkedEntityAttributes()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.NotAny)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        await Assert.That(result.Entities.Any(e => e.Attributes.Keys.Any(k => k.StartsWith("c.")))).IsFalse();
    }

    #endregion

    #region JoinOperator.Exists (QueryExpression)

    /// <summary>
    /// JoinOperator.Exists behaves like Any: only accounts that have at least one related
    /// contact are returned, without duplicating rows or projecting inner attributes.
    /// </summary>
    [Test]
    public async Task QueryExpression_ExistsJoin_ReturnsOnlyAccountsWithAtLeastOneRelatedEntity()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Exists)
            }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// JoinOperator.Exists must not duplicate outer rows for multiple inner matches.
    /// </summary>
    [Test]
    public async Task QueryExpression_ExistsJoin_DoesNotDuplicateOuterRows()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Exists)
            }
        });

        var corpBRows = result.Entities.Where(e => e.Id == CorpBId).ToList();
        await Assert.That(corpBRows).Count().IsEqualTo(1);
    }

    #endregion

    #region FetchXml parsing

    /// <summary>
    /// FetchXml link-type="any" must be parsed as JoinOperator.Any.
    /// </summary>
    [Test]
    public async Task FetchXml_AnyLinkType_IsParsedAsJoinOperatorAny()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="contactid" to="accountid" alias="c" link-type="any">
                </link-entity>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(
            new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        var link = response.Query.LinkEntities[0];
        await Assert.That(link.JoinOperator).IsEqualTo(JoinOperator.Any);
    }

    /// <summary>
    /// FetchXml link-type="not-any" must be parsed as JoinOperator.NotAny.
    /// </summary>
    [Test]
    public async Task FetchXml_NotAnyLinkType_IsParsedAsJoinOperatorNotAny()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="contactid" to="accountid" alias="c" link-type="not-any">
                </link-entity>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(
            new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        var link = response.Query.LinkEntities[0];
        await Assert.That(link.JoinOperator).IsEqualTo(JoinOperator.NotAny);
    }

    /// <summary>
    /// FetchXml link-type="exists" must be parsed as JoinOperator.Exists.
    /// </summary>
    [Test]
    public async Task FetchXml_ExistsLinkType_IsParsedAsJoinOperatorExists()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="contactid" to="accountid" alias="c" link-type="exists">
                </link-entity>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(
            new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        var link = response.Query.LinkEntities[0];
        await Assert.That(link.JoinOperator).IsEqualTo(JoinOperator.Exists);
    }

    /// <summary>
    /// End-to-end: FetchXml with link-type="any" must return only accounts with at least one
    /// matching contact — not all accounts, not duplicate rows.
    /// </summary>
    [Test]
    public async Task FetchXml_AnyJoin_ReturnsOnlyAccountsWithAtLeastOneContact()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // account → contact via parentcustomerid:
        //   from="parentcustomerid" (on contact) to="accountid" (on account)
        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="any">
                </link-entity>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// End-to-end: FetchXml with link-type="not-any" must return only accounts with no
    /// matching contact.
    /// </summary>
    [Test]
    public async Task FetchXml_NotAnyJoin_ReturnsOnlyAccountsWithNoContact()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="not-any">
                </link-entity>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpAId);
    }

    #endregion

    #region Combined: outer Criteria + EXISTS join

    /// <summary>
    /// An outer QueryExpression filter combined with an Any join must respect both conditions
    /// independently: the outer filter narrows the outer set, the Any join then keeps only
    /// those that have at least one matching related entity.
    /// This verifies that the ExpressionProcessor guard does not suppress the outer Criteria.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_WithOuterCriteria_AppliesBothFilters()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // Outer filter: Name = "B Corp" (matches only corpB)
        // Any join: must have at least one contact
        // Expected: corpB qualifies on both counts → 1 result
        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, "B Corp")
                }
            },
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
            }
        });

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    /// <summary>
    /// Outer criteria that matches no account combined with Any join must return empty,
    /// even when the Any condition would have been satisfied for some accounts.
    /// </summary>
    [Test]
    public async Task QueryExpression_AnyJoin_WithOuterCriteriaThatMatchesNone_ReturnsEmpty()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, "No Such Corp")
                }
            },
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.Any)
            }
        });

        await Assert.That(result.Entities).IsEmpty();
    }

    #endregion

    #region JoinOperator.NotAny + LinkCriteria

    /// <summary>
    /// NotAny with LinkCriteria must return accounts that have NO contact satisfying the criteria.
    /// corpA has no contacts at all → included.
    /// corpB has contacts but none named "Nobody" → included.
    /// Both qualify because neither has a contact with FirstName="Nobody".
    /// </summary>
    [Test]
    public async Task QueryExpression_NotAnyJoin_WithLinkCriteria_ReturnsAccountsWithNoMatchingContact()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.NotAny)
                {
                    LinkCriteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "Nobody")
                        }
                    }
                }
            }
        });

        // Neither account has a contact called "Nobody", so both pass the NOT EXISTS check
        await Assert.That(result.Entities).Count().IsEqualTo(2);
    }

    /// <summary>
    /// NotAny with LinkCriteria for "John B" must exclude corpB (has "John B") and include
    /// corpA (has no contacts, so definitely no "John B").
    /// </summary>
    [Test]
    public async Task QueryExpression_NotAnyJoin_WithLinkCriteria_ExcludesAccountsWithMatchingContact()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.NotAny)
                {
                    LinkCriteria = new FilterExpression
                    {
                        Conditions =
                        {
                            new ConditionExpression(Contact.LogicalNames.FirstName, ConditionOperator.Equal, "John B")
                        }
                    }
                }
            }
        });

        // corpB has "John B" → excluded. corpA has no contacts → included.
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpAId);
    }

    #endregion

    #region FetchXml: link-type="in" parsing + Any with filter E2E

    /// <summary>
    /// FetchXml link-type="in" must be parsed as JoinOperator.In.
    /// </summary>
    [Test]
    public async Task FetchXml_InLinkType_IsParsedAsJoinOperatorIn()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="contactid" to="accountid" alias="c" link-type="in">
                </link-entity>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(
            new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        var link = response.Query.LinkEntities[0];
        await Assert.That(link.JoinOperator).IsEqualTo(JoinOperator.In);
    }

    /// <summary>
    /// End-to-end: FetchXml with link-type="any" and a &lt;filter&gt; inside the link-entity
    /// must apply the filter as a subquery constraint.
    /// Only corpB has a contact with FirstName="John B", so only corpB is returned.
    /// </summary>
    [Test]
    public async Task FetchXml_AnyJoin_WithFilter_AppliesSubqueryFilter()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="parentcustomerid" to="accountid" link-type="any">
                  <filter>
                    <condition attribute="firstname" operator="eq" value="John B" />
                  </filter>
                </link-entity>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(CorpBId);
    }

    #endregion

    #region Unsupported operators (All, NotAll) — must throw ArgumentException

    /// <summary>
    /// JoinOperator.All is parsed from FetchXml but not yet implemented in LinkedEntitiesProcessor.
    /// It must throw a clear ArgumentException rather than a NullReferenceException or silent wrong result.
    /// </summary>
    [Test]
    public async Task QueryExpression_AllJoin_ThrowsArgumentException()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        await Assert.That(() => sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.All)
            }
        })).Throws<ArgumentException>();
    }

    /// <summary>
    /// JoinOperator.NotAll is parsed from FetchXml but not yet implemented.
    /// It must throw a clear ArgumentException.
    /// </summary>
    [Test]
    public async Task QueryExpression_NotAllJoin_ThrowsArgumentException()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        await Assert.That(() => sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            LinkEntities =
            {
                new LinkEntity(
                    Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.NotAll)
            }
        })).Throws<ArgumentException>();
    }

    #endregion
}
