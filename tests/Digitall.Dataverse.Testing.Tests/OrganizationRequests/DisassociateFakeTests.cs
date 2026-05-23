// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class DisassociateFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new AssociateFake());
        _sut.AddRequest(new DisassociateFake());
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    private void SetupManyToManyRelationship()
    {
        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contact_mm",
            Entity1LogicalName = "account",
            Entity2LogicalName = "contact",
            Entity1IntersectAttribute = "accountid",
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contact"
        });
        // Register intersect entity metadata so RetrieveMultiple in DisassociateFake doesn't reject it
        _sut.AddMetadata(new EntityMetadata { LogicalName = "account_contact" });
    }

    [Test]
    public async Task Execute_ManyToMany_RemovesIntersectRecord()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId });
        SetupManyToManyRelationship();

        // First associate
        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection { new("contact", contactId) }
        });

        // Verify associated
        var before = _sut.CreateQuery("account_contact").ToList();
        await Assert.That(before).HasCount().EqualTo(1);

        // Disassociate
        _sut.Execute(new DisassociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection { new("contact", contactId) }
        });

        var after = _sut.CreateQuery("account_contact").ToList();
        await Assert.That(after).IsEmpty();
    }

    [Test]
    public async Task Execute_ManyToMany_OnlyRemovesSpecificRelation()
    {
        var accountId = Guid.NewGuid();
        var contactId1 = Guid.NewGuid();
        var contactId2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId1 });
        _sut.Add(new Entity("contact") { Id = contactId2 });
        SetupManyToManyRelationship();

        // Associate both
        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection
            {
                new("contact", contactId1),
                new("contact", contactId2)
            }
        });

        // Disassociate only one
        _sut.Execute(new DisassociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection { new("contact", contactId1) }
        });

        var remaining = _sut.CreateQuery("account_contact").ToList();
        await Assert.That(remaining).HasCount().EqualTo(1);
        await Assert.That(remaining[0].GetAttributeValue<Guid>("contactid")).IsEqualTo(contactId2);
    }

    [Test]
    public async Task Execute_UnknownRelationship_ThrowsFault()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        void Action() => _sut.Execute(new DisassociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("nonexistent"),
            RelatedEntities = new EntityReferenceCollection { new("contact", Guid.NewGuid()) }
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Execute_NonManyToManyRelationship_ThrowsFault()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        _sut.AddRelationship(new OneToManyRelationshipMetadata
        {
            SchemaName = "account_contacts_1n",
            ReferencedEntity = "account",
            ReferencingEntity = "contact",
            ReferencingAttribute = "parentcustomerid"
        });

        void Action() => _sut.Execute(new DisassociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contacts_1n"),
            RelatedEntities = new EntityReferenceCollection { new("contact", Guid.NewGuid()) }
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Execute_ReturnsDisassociateResponse()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId });
        SetupManyToManyRelationship();

        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection { new("contact", contactId) }
        });

        var response = _sut.Execute(new DisassociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = new EntityReferenceCollection { new("contact", contactId) }
        });

        await Assert.That(response).IsTypeOf<DisassociateResponse>();
    }
}
