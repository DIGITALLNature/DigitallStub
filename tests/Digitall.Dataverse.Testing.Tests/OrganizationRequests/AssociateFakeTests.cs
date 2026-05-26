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

public class AssociateFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new AssociateFake());
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_ManyToMany_CreatesIntersectRecord()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId });

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contact_mm",
            Entity1LogicalName = "account",
            Entity2LogicalName = "contact",
            Entity1IntersectAttribute = "accountid",
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contact"
        });

        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = [new("contact", contactId)]
        });

        var intersect = _sut.CreateQuery("account_contact").ToList();
        await Assert.That(intersect).Count().IsEqualTo(1);
        await Assert.That(intersect[0].GetAttributeValue<Guid>("accountid")).IsEqualTo(accountId);
        await Assert.That(intersect[0].GetAttributeValue<Guid>("contactid")).IsEqualTo(contactId);
    }

    [Test]
    public async Task Execute_OneToMany_SetsLookupOnRelatedEntity()
    {
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId });

        _sut.AddRelationship(new OneToManyRelationshipMetadata
        {
            SchemaName = "account_contacts",
            ReferencedEntity = "account",
            ReferencingEntity = "contact",
            ReferencingAttribute = "parentcustomerid"
        });

        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contacts"),
            RelatedEntities = [new("contact", contactId)]
        });

        var contact = _sut.Retrieve("contact", contactId, new ColumnSet(true));
        var lookup = contact.GetAttributeValue<EntityReference>("parentcustomerid");
        await Assert.That(lookup).IsNotNull();
        await Assert.That(lookup!.Id).IsEqualTo(accountId);
        await Assert.That(lookup.LogicalName).IsEqualTo("account");
    }

    [Test]
    public async Task Execute_ManyToMany_MultipleRelatedEntities_CreatesMultipleIntersects()
    {
        var accountId = Guid.NewGuid();
        var contactId1 = Guid.NewGuid();
        var contactId2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });
        _sut.Add(new Entity("contact") { Id = contactId1 });
        _sut.Add(new Entity("contact") { Id = contactId2 });

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contact_mm",
            Entity1LogicalName = "account",
            Entity2LogicalName = "contact",
            Entity1IntersectAttribute = "accountid",
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contact"
        });

        _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = [new("contact", contactId1), new("contact", contactId2)]
        });

        var intersects = _sut.CreateQuery("account_contact").ToList();
        await Assert.That(intersects).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Execute_UnknownRelationship_ThrowsFault()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        void Action() => _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("nonexistent_relationship"),
            RelatedEntities = [new("contact", Guid.NewGuid())]
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Execute_ManyToMany_TargetDoesNotExist_ThrowsFault()
    {
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid() });

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contact_mm",
            Entity1LogicalName = "account",
            Entity2LogicalName = "contact",
            Entity1IntersectAttribute = "accountid",
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contact"
        });

        void Action() => _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", Guid.NewGuid()),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = [new("contact", Guid.NewGuid())]
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Execute_ManyToMany_RelatedDoesNotExist_ThrowsFault()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contact_mm",
            Entity1LogicalName = "account",
            Entity2LogicalName = "contact",
            Entity1IntersectAttribute = "accountid",
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contact"
        });

        void Action() => _sut.Execute(new AssociateRequest
        {
            Target = new EntityReference("account", accountId),
            Relationship = new Relationship("account_contact_mm"),
            RelatedEntities = [new("contact", Guid.NewGuid())]
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }
}
