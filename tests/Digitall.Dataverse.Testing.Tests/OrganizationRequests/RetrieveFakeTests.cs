// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class RetrieveFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveFake());
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task RelatedEntitiesQuery_Null_DoesNotFail()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Account(accountId) { Name = "Test" });

        var request = new RetrieveRequest
        {
            Target    = new EntityReference(Account.EntityLogicalName, accountId),
            ColumnSet = new ColumnSet(true)
        };

        var response = (RetrieveResponse)_sut.Execute(request);

        await Assert.That(response.Entity).IsNotNull();
        await Assert.That(response.Entity.RelatedEntities.Count).IsEqualTo(0);
    }

    [Test]
    public async Task RelatedEntitiesQuery_OneToMany_PopulatesRelatedEntities()
    {
        // Arrange
        var accountId  = Guid.NewGuid();
        var contactId1 = Guid.NewGuid();
        var contactId2 = Guid.NewGuid();

        _sut.AddRelationship(new OneToManyRelationshipMetadata
        {
            SchemaName          = "contact_customer_accounts",
            ReferencedEntity    = Account.EntityLogicalName,
            ReferencedAttribute = "accountid",
            ReferencingEntity   = Contact.EntityLogicalName,
            ReferencingAttribute = "parentcustomerid"
        });

        _sut.Add(new Account(accountId) { Name = "Contoso" });
        _sut.Add(new Contact(contactId1) { LastName = "Doe", ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountId) });
        _sut.Add(new Contact(contactId2) { LastName = "Smith", ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountId) });
        _sut.Add(new Contact(Guid.NewGuid()) { LastName = "Other" }); // unrelated contact

        var request = new RetrieveRequest
        {
            Target    = new EntityReference(Account.EntityLogicalName, accountId),
            ColumnSet = new ColumnSet(true),
            RelatedEntitiesQuery = new RelationshipQueryCollection
            {
                { new Relationship("contact_customer_accounts"), new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) } }
            }
        };

        // Act
        var response = (RetrieveResponse)_sut.Execute(request);

        // Assert
        var rel = new Relationship("contact_customer_accounts");
        await Assert.That(response.Entity.RelatedEntities.ContainsKey(rel)).IsTrue();
        var related = response.Entity.RelatedEntities[rel];
        await Assert.That(related.Entities.Count).IsEqualTo(2);
        await Assert.That(related.Entities.Any(e => e.Id == contactId1)).IsTrue();
        await Assert.That(related.Entities.Any(e => e.Id == contactId2)).IsTrue();
    }

    [Test]
    public async Task RelatedEntitiesQuery_OneToMany_ReturnsEmptyCollection_WhenNoRelatedRecords()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _sut.AddRelationship(new OneToManyRelationshipMetadata
        {
            SchemaName           = "contact_customer_accounts",
            ReferencedEntity     = Account.EntityLogicalName,
            ReferencedAttribute  = "accountid",
            ReferencingEntity    = Contact.EntityLogicalName,
            ReferencingAttribute = "parentcustomerid"
        });

        _sut.Add(new Account(accountId) { Name = "Contoso" });

        var request = new RetrieveRequest
        {
            Target    = new EntityReference(Account.EntityLogicalName, accountId),
            ColumnSet = new ColumnSet(true),
            RelatedEntitiesQuery = new RelationshipQueryCollection
            {
                { new Relationship("contact_customer_accounts"), new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) } }
            }
        };

        // Act
        var response = (RetrieveResponse)_sut.Execute(request);

        // Assert
        var rel = new Relationship("contact_customer_accounts");
        await Assert.That(response.Entity.RelatedEntities.ContainsKey(rel)).IsTrue();
        await Assert.That(response.Entity.RelatedEntities[rel].Entities.Count).IsEqualTo(0);
    }

    [Test]
    public async Task RelatedEntitiesQuery_ManyToMany_PopulatesRelatedEntities()
    {
        // Arrange
        var accountId  = Guid.NewGuid();
        var contactId1 = Guid.NewGuid();
        var contactId2 = Guid.NewGuid();
        var unrelatedContactId = Guid.NewGuid();

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName              = "account_contacts",
            Entity1LogicalName      = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName      = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName     = "account_contacts_assoc"
        });
        _sut.AddMetadata(new EntityMetadata { LogicalName = "account_contacts_assoc" });

        _sut.Add(new Account(accountId));
        _sut.Add(new Contact(contactId1));
        _sut.Add(new Contact(contactId2));
        _sut.Add(new Contact(unrelatedContactId));

        // Create intersect records
        _sut.Add(new Entity("account_contacts_assoc") { Id = Guid.NewGuid(), ["accountid"] = accountId, ["contactid"] = contactId1 });
        _sut.Add(new Entity("account_contacts_assoc") { Id = Guid.NewGuid(), ["accountid"] = accountId, ["contactid"] = contactId2 });

        var request = new RetrieveRequest
        {
            Target    = new EntityReference(Account.EntityLogicalName, accountId),
            ColumnSet = new ColumnSet(true),
            RelatedEntitiesQuery = new RelationshipQueryCollection
            {
                { new Relationship("account_contacts"), new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) } }
            }
        };

        // Act
        var response = (RetrieveResponse)_sut.Execute(request);

        // Assert
        var rel = new Relationship("account_contacts");
        await Assert.That(response.Entity.RelatedEntities.ContainsKey(rel)).IsTrue();
        var related = response.Entity.RelatedEntities[rel];
        await Assert.That(related.Entities.Count).IsEqualTo(2);
        await Assert.That(related.Entities.Any(e => e.Id == contactId1)).IsTrue();
        await Assert.That(related.Entities.Any(e => e.Id == contactId2)).IsTrue();
    }

    [Test]
    public async Task RelatedEntitiesQuery_UnknownRelationship_IsSkipped()
    {
        // Arrange
        var accountId = Guid.NewGuid();
        _sut.Add(new Account(accountId) { Name = "Test" });

        var request = new RetrieveRequest
        {
            Target    = new EntityReference(Account.EntityLogicalName, accountId),
            ColumnSet = new ColumnSet(true),
            RelatedEntitiesQuery = new RelationshipQueryCollection
            {
                { new Relationship("unknown_relationship"), new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) } }
            }
        };

        // Act
        var response = (RetrieveResponse)_sut.Execute(request);

        // Assert — unknown relationship is silently skipped, no exception
        await Assert.That(response.Entity).IsNotNull();
        await Assert.That(response.Entity.RelatedEntities.Count).IsEqualTo(0);
    }
}
