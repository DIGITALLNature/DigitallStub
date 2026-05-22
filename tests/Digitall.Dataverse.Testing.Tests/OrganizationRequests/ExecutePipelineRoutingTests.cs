// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

/// <summary>
/// Tests that verify IOrganizationService façade methods (Create, Update, Delete, Retrieve,
/// Associate, Disassociate) route through Execute so that registered request fakes can intercept them.
/// </summary>
public class ExecutePipelineRoutingTests
{
    #region Create routes through Execute

    [Test]
    public async Task Create_TriggersRegisteredCreateRequestSpy()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<CreateRequest, CreateResponse>(
            (_, _) => new CreateResponse
            {
                ResponseName = "Create",
                Results = new ParameterCollection { { "id", Guid.NewGuid() } }
            });
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        var entity = new Account { Name = "Pipeline Test" };

        // Act
        service.Create(entity);

        // Assert — the spy must have seen the CreateRequest
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(spy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
    }

    [Test]
    public async Task Create_ViaExecute_AndCreate_ViaFacade_TriggerSameSpyOnce()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<CreateRequest, CreateResponse>(
            (_, _) => new CreateResponse
            {
                ResponseName = "Create",
                Results = new ParameterCollection { { "id", Guid.NewGuid() } }
            });
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        // Act — one via façade, one via Execute
        service.Create(new Account { Name = "Façade" });
        service.Execute(new CreateRequest { Target = new Account { Name = "Execute" } });

        // Assert — spy saw both
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(2);
    }

    #endregion

    #region Update routes through Execute

    [Test]
    public async Task Update_TriggersRegisteredUpdateRequestSpy()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<UpdateRequest, UpdateResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        var id = Guid.NewGuid();
        service.Add(new Account(id) { Name = "Original" });

        // Act
        service.Update(new Account(id) { Name = "Updated" });

        // Assert
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(spy.ReceivedRequests[0].Target.Id).IsEqualTo(id);
        await Assert.That(spy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
    }

    #endregion

    #region Delete routes through Execute

    [Test]
    public async Task Delete_TriggersRegisteredDeleteRequestSpy()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<DeleteRequest, DeleteResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        var id = Guid.NewGuid();
        service.Add(new Account(id) { Name = "ToDelete" });

        // Act
        service.Delete(Account.EntityLogicalName, id);

        // Assert
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(spy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
        await Assert.That(spy.ReceivedRequests[0].Target.Id).IsEqualTo(id);
    }

    #endregion

    #region Retrieve routes through Execute

    [Test]
    public async Task Retrieve_TriggersRegisteredRetrieveRequestSpy()
    {
        // Arrange
        var id = Guid.NewGuid();
        var service = new FakeOrganizationService();
        service.Add(new Account(id) { Name = "Exists" });

        // NOTE: We cannot use this spy approach directly because it would recurse.
        // Instead, the test verifies the spy is triggered; the handler can use internal helpers.
        // For now, we use a simple recording spy without custom handler.
        var recordingSpy = new SpyOrganizationRequestFake<RetrieveRequest, RetrieveResponse>();
        service.AddRequest(recordingSpy);

        // Act — this will fail if Retrieve doesn't route through Execute
        // (today it bypasses Execute entirely; spy handler returns empty response)
        try
        {
            service.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        }
        catch
        {
            // May throw because spy returns empty RetrieveResponse — that's ok for this test
        }

        // Assert — the spy must have been triggered
        await Assert.That(recordingSpy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(recordingSpy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
        await Assert.That(recordingSpy.ReceivedRequests[0].Target.Id).IsEqualTo(id);
    }

    #endregion

    #region Associate routes through Execute

    [Test]
    public async Task Associate_TriggersRegisteredAssociateRequestSpy()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<AssociateRequest, AssociateResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        service.Add(new Account(accountId));
        service.Add(new Contact(contactId));

        service.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "test_account_contact",
            Entity1LogicalName = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "test_account_contact_intersect"
        });
        service.AddMetadata(new EntityMetadata { LogicalName = "test_account_contact_intersect" });

        // Act
        service.Associate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Assert
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(spy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
        await Assert.That(spy.ReceivedRequests[0].Target.Id).IsEqualTo(accountId);
    }

    #endregion

    #region Disassociate routes through Execute

    [Test]
    public async Task Disassociate_TriggersRegisteredDisassociateRequestSpy()
    {
        // Arrange
        var spy = new SpyOrganizationRequestFake<DisassociateRequest, DisassociateResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        service.Add(new Account(accountId));
        service.Add(new Contact(contactId));

        service.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "test_account_contact",
            Entity1LogicalName = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "test_account_contact_intersect"
        });
        service.AddMetadata(new EntityMetadata { LogicalName = "test_account_contact_intersect" });

        // First associate so there's something to disassociate
        service.Associate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Act
        service.Disassociate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Assert
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(spy.ReceivedRequests[0].Target.LogicalName).IsEqualTo(Account.EntityLogicalName);
        await Assert.That(spy.ReceivedRequests[0].Target.Id).IsEqualTo(accountId);
    }

    #endregion

    #region Nested pipeline calls (Associate triggers Create internally)

    [Test]
    public async Task Associate_InternallyTriggersCreateRequestSpy_ForIntersectEntity()
    {
        // Arrange — spy on CreateRequest to see the intersect record being created
        var createSpy = new SpyOrganizationRequestFake<CreateRequest, CreateResponse>(
            (_, _) => new CreateResponse
            {
                ResponseName = "Create",
                Results = new ParameterCollection { { "id", Guid.NewGuid() } }
            });
        var service = new FakeOrganizationService();
        service.AddRequest(createSpy);

        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        service.Add(new Account(accountId));
        service.Add(new Contact(contactId));

        service.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "test_account_contact",
            Entity1LogicalName = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "test_account_contact_intersect"
        });
        service.AddMetadata(new EntityMetadata { LogicalName = "test_account_contact_intersect" });

        // Act
        service.Associate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Assert — CreateRequest spy should have seen the intersect entity creation
        await Assert.That(createSpy.ReceivedRequests).Count().IsGreaterThanOrEqualTo(1);
        await Assert.That(createSpy.ReceivedRequests.Any(r =>
            r.Target.LogicalName == "test_account_contact_intersect")).IsTrue();
    }

    [Test]
    public async Task Disassociate_InternallyTriggersDeleteRequestSpy_ForIntersectEntity()
    {
        // Arrange
        var service = new FakeOrganizationService();

        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        service.Add(new Account(accountId));
        service.Add(new Contact(contactId));

        service.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "test_account_contact",
            Entity1LogicalName = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "test_account_contact_intersect"
        });
        service.AddMetadata(new EntityMetadata { LogicalName = "test_account_contact_intersect" });

        // Associate first
        service.Associate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Now register the spy AFTER associate, so we only capture the disassociate's delete
        var deleteSpy = new SpyOrganizationRequestFake<DeleteRequest, DeleteResponse>();
        service.AddRequest(deleteSpy);

        // Act
        service.Disassociate(
            Account.EntityLogicalName,
            accountId,
            new Relationship("test_account_contact"),
            [new(Contact.EntityLogicalName, contactId)]);

        // Assert — DeleteRequest spy should have seen the intersect entity deletion
        await Assert.That(deleteSpy.ReceivedRequests).Count().IsGreaterThanOrEqualTo(1);
        await Assert.That(deleteSpy.ReceivedRequests.Any(r =>
            r.Target.LogicalName == "test_account_contact_intersect")).IsTrue();
    }

    #endregion

    #region Seed API (Add) does NOT trigger pipeline

    [Test]
    public async Task Add_DoesNotTriggerCreateRequestSpy()
    {
        // Arrange — Add is a test-seeding API; it should NOT route through Execute
        var spy = new SpyOrganizationRequestFake<CreateRequest, CreateResponse>(
            (_, _) => new CreateResponse
            {
                ResponseName = "Create",
                Results = new ParameterCollection { { "id", Guid.NewGuid() } }
            });
        var service = new FakeOrganizationService();
        service.AddRequest(spy);

        // Act
        service.Add(new Account(Guid.NewGuid()) { Name = "Seeded" });

        // Assert — spy must NOT have been triggered
        await Assert.That(spy.ReceivedRequests).Count().IsEqualTo(0);
    }

    #endregion
}
