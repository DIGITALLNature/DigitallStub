// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class UpsertFakeDeepInsertTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService
        {
            Options = { UserId = Guid.NewGuid() }
        };

        // Register OneToMany relationship: account -> contacts
        _sut.State.Relationships["contact_customer_accounts"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "contact_customer_accounts",
            ReferencedEntity = "account",
            ReferencedAttribute = "accountid",
            ReferencingEntity = "contact",
            ReferencingAttribute = "parentcustomerid"
        };

        await Task.CompletedTask;
    }

    [Test]
    public async Task Upsert_CreatePath_WithRelatedEntities_CreatesChildren()
    {
        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        var contact = new Entity("contact") { ["lastname"] = "Smith" };

        account.RelatedEntities[new Relationship("contact_customer_accounts")] =
            new EntityCollection([contact]);

        var response = (UpsertResponse)_sut.Execute(new UpsertRequest { Target = account });

        await Assert.That((bool)response["RecordCreated"]).IsTrue();

        var contacts = _sut.CreateQuery("contact").ToList();
        await Assert.That(contacts).HasCount().EqualTo(1);

        var createdContact = contacts.Single();
        var parentRef = createdContact.GetAttributeValue<EntityReference>("parentcustomerid");
        await Assert.That(parentRef!.Id).IsEqualTo(account.Id);
    }

    [Test]
    public async Task Upsert_UpdatePath_WithRelatedEntities_CreatesChildren()
    {
        var accountId = Guid.NewGuid();

        // Pre-create the account so upsert takes the update path
        _sut.Create(new Entity("account") { Id = accountId, ["name"] = "Contoso" });

        var updatedAccount = new Entity("account") { Id = accountId, ["name"] = "Contoso Ltd." };
        var contact = new Entity("contact") { ["lastname"] = "Jones" };

        updatedAccount.RelatedEntities[new Relationship("contact_customer_accounts")] =
            new EntityCollection([contact]);

        var response = (UpsertResponse)_sut.Execute(new UpsertRequest { Target = updatedAccount });

        await Assert.That((bool)response["RecordCreated"]).IsFalse();

        // Parent was updated
        var account = _sut.CreateQuery("account").Single(e => e.Id == accountId);
        await Assert.That(account.GetAttributeValue<string>("name")).IsEqualTo("Contoso Ltd.");

        // Child was created with correct FK
        var contacts = _sut.CreateQuery("contact").ToList();
        await Assert.That(contacts).HasCount().EqualTo(1);

        var createdContact = contacts.Single();
        var parentRef = createdContact.GetAttributeValue<EntityReference>("parentcustomerid");
        await Assert.That(parentRef!.Id).IsEqualTo(accountId);
    }

    [Test]
    public async Task Upsert_UpdatePath_WithoutRelatedEntities_DoesNotCreateChildren()
    {
        var accountId = Guid.NewGuid();
        _sut.Create(new Entity("account") { Id = accountId, ["name"] = "Contoso" });

        var updatedAccount = new Entity("account") { Id = accountId, ["name"] = "Contoso Updated" };

        _sut.Execute(new UpsertRequest { Target = updatedAccount });

        var contacts = _sut.CreateQuery("contact").ToList();
        await Assert.That(contacts).IsEmpty();
    }
}
