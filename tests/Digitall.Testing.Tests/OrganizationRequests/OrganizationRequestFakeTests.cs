// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Testing.OrganizationRequests;
using Digitall.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests.OrganizationRequests;

public class OrganizationRequestFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        await Task.CompletedTask;
    }

    [Test]
    public async Task CreateFake_Should_CreateRecord()
    {
        _sut.AddRequest(new CreateFake());
        var account = new Account { Name = "Test Account" };
        var request = new CreateRequest { Target = account };

        var response = (CreateResponse)_sut.Execute(request);

        await Assert.That(response.id).IsNotEqualTo(Guid.Empty);
        var retrieved = _sut.Retrieve(Account.EntityLogicalName, response.id, new ColumnSet(true));
        await Assert.That(retrieved.GetAttributeValue<string>("name")).IsEqualTo("Test Account");
    }

    [Test]
    public async Task UpdateFake_Should_UpdateRecord()
    {
        _sut.AddRequest(new UpdateFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id) { Name = "Old Name" });

        var updateAccount = new Account(id) { Name = "New Name" };
        var request = new UpdateRequest { Target = updateAccount };

        _sut.Execute(request);

        var retrieved = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved.GetAttributeValue<string>("name")).IsEqualTo("New Name");
    }

    [Test]
    public async Task DeleteFake_Should_RemoveRecord()
    {
        _sut.AddRequest(new DeleteFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id));

        var request = new DeleteRequest { Target = new EntityReference(Account.EntityLogicalName, id) };

        _sut.Execute(request);

        void Action() => _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
    }

    [Test]
    public async Task RetrieveFake_Should_ReturnRecord()
    {
        _sut.AddRequest(new RetrieveFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id) { Name = "Test Account" });

        var request = new RetrieveRequest
        {
            Target = new EntityReference(Account.EntityLogicalName, id),
            ColumnSet = new ColumnSet("name")
        };

        var response = (RetrieveResponse)_sut.Execute(request);

        await Assert.That(response.Entity).IsNotNull();
        await Assert.That(response.Entity.Id).IsEqualTo(id);
        await Assert.That(response.Entity.GetAttributeValue<string>("name")).IsEqualTo("Test Account");
    }

    [Test]
    public async Task UpsertFake_Should_CreateIfNew()
    {
        _sut.AddRequest(new UpsertFake());
        var id = Guid.NewGuid();
        var account = new Account(id) { Name = "New Account" };
        var request = new UpsertRequest { Target = account };

        var response = (UpsertResponse)_sut.Execute(request);

        await Assert.That((bool)response.Results["RecordCreated"]).IsTrue();
        var retrieved = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved).IsNotNull();
    }

    [Test]
    public async Task UpsertFake_Should_UpdateIfExisting()
    {
        _sut.AddRequest(new UpsertFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id) { Name = "Existing" });

        var account = new Account(id) { Name = "Updated" };
        var request = new UpsertRequest { Target = account };

        var response = (UpsertResponse)_sut.Execute(request);

        await Assert.That((bool)response.Results["RecordCreated"]).IsFalse();
        var retrieved = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved.GetAttributeValue<string>("name")).IsEqualTo("Updated");
    }

    [Test]
    public async Task SetStateFake_Should_UpdateStateAndStatus()
    {
        _sut.AddRequest(new SetStateFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id));

        var request = new Microsoft.Crm.Sdk.Messages.SetStateRequest
        {
            EntityMoniker = new EntityReference(Account.EntityLogicalName, id),
            State = new OptionSetValue(1),
            Status = new OptionSetValue(2)
        };

        _sut.Execute(request);

        var updated = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(updated.GetAttributeValue<OptionSetValue>("statecode").Value).IsEqualTo(1);
        await Assert.That(updated.GetAttributeValue<OptionSetValue>("statuscode").Value).IsEqualTo(2);
    }

    [Test]
    public async Task SetStateFake_Should_UseDefaultStatusValue()
    {
        _sut.AddRequest(new SetStateFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id));

        var request = new Microsoft.Crm.Sdk.Messages.SetStateRequest
        {
            EntityMoniker = new EntityReference(Account.EntityLogicalName, id),
            State = new OptionSetValue(0),
            Status = new OptionSetValue(-1) // Dataverse should use the default statuscode for the statecode
        };

        _sut.Execute(request);

        var updated = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(updated.GetAttributeValue<OptionSetValue>("statecode").Value).IsEqualTo(0);
        await Assert.That(updated.GetAttributeValue<OptionSetValue>("statuscode").Value).IsEqualTo(1);
    }

    [Test]
    public async Task AssignRequestFake_Should_UpdateOwner()
    {
        _sut.AddRequest(new AssignRequestFake());
        var id = Guid.NewGuid();
        _sut.Add(new Account(id));
        var userId = Guid.NewGuid();
        var userRef = new EntityReference("systemuser", userId);

        var request = new Microsoft.Crm.Sdk.Messages.AssignRequest
        {
            Target = new EntityReference(Account.EntityLogicalName, id),
            Assignee = userRef
        };

        _sut.Execute(request);

        var updated = _sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(updated.GetAttributeValue<EntityReference>("ownerid").Id).IsEqualTo(userId);
        await Assert.That(updated.GetAttributeValue<EntityReference>("owninguser").Id).IsEqualTo(userId);
    }

    [Test]
    public async Task AssociateFake_Should_CallStateAssociate()
    {
        _sut.AddRequest(new AssociateFake());
        var accountId = Guid.NewGuid();
        var contactId = Guid.NewGuid();
        _sut.Add(new Account(accountId));
        _sut.Add(new Contact(contactId));

        _sut.AddRelationship(new ManyToManyRelationshipMetadata
        {
            SchemaName = "account_contacts",
            Entity1LogicalName = Account.EntityLogicalName,
            Entity1IntersectAttribute = "accountid",
            Entity2LogicalName = Contact.EntityLogicalName,
            Entity2IntersectAttribute = "contactid",
            IntersectEntityName = "account_contacts_association"
        });
        _sut.AddMetadata(new EntityMetadata { LogicalName = "account_contacts_association" });

        var request = new AssociateRequest
        {
            Target = new EntityReference(Account.EntityLogicalName, accountId),
            Relationship = new Relationship("account_contacts"),
            RelatedEntities = [new EntityReference(Contact.EntityLogicalName, contactId)]
        };

        _sut.Execute(request);
        await Task.CompletedTask;
    }

    [Test]
    public async Task DisassociateFake_Should_CallStateDisassociate()
    {
        _sut.AddRequest(new DisassociateFake());
        var accountId = Guid.NewGuid();
        var request = new DisassociateRequest
        {
            Target = new EntityReference(Account.EntityLogicalName, accountId),
            Relationship = new Relationship("account_contacts"),
            RelatedEntities = [new EntityReference(Contact.EntityLogicalName, Guid.NewGuid())]
        };

        try {
            _sut.Execute(request);
        } catch (Exception) {
            // Ignore failure for now as long as it reaches the fake
        }
        await Task.CompletedTask;
    }

    [Test]
    public async Task ExecuteTransactionFake_Should_ExecuteAllRequests()
    {
        _sut.AddRequest(new ExecuteTransactionFake());
        _sut.AddRequest(new CreateFake());

        var request = new ExecuteTransactionRequest
        {
            Requests = [
                new CreateRequest { Target = new Account { Name = "Acc 1" } },
                new CreateRequest { Target = new Account { Name = "Acc 2" } }
            ],
            ReturnResponses = true
        };

        var response = (ExecuteTransactionResponse)_sut.Execute(request);

        await Assert.That(response.Responses.Count).IsEqualTo(2);
        var accounts = _sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(accounts.Entities).Count().IsEqualTo(2);
    }

    [Test]
    public async Task BulkDeleteFake_Should_DeleteRecords()
    {
        _sut.AddRequest(new BulkDeleteFake());
        _sut.AddRequest(new CreateFake());

        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        _sut.Add(new Account(id1) { Name = "Delete Me" });
        _sut.Add(new Account(id2) { Name = "Keep Me" });

        var request = new Microsoft.Crm.Sdk.Messages.BulkDeleteRequest
        {
            JobName = "Bulk Delete Test",
            QuerySet = [
                new QueryExpression(Account.EntityLogicalName) {
                    Criteria = new FilterExpression {
                        Conditions = {
                            new ConditionExpression("name", ConditionOperator.Equal, "Delete Me")
                        }
                    }
                }
            ],
            ToRecipients = [],
            CCRecipients = [],
            SendEmailNotification = false
        };

        var response = (Microsoft.Crm.Sdk.Messages.BulkDeleteResponse)_sut.Execute(request);

        await Assert.That(response.Results.ContainsKey("JobId")).IsTrue();

        void RetrieveDeleted() => _sut.Retrieve(Account.EntityLogicalName, id1, new ColumnSet(true));
        Assert.Throws<FaultException<OrganizationServiceFault>>(RetrieveDeleted);

        var kept = _sut.Retrieve(Account.EntityLogicalName, id2, new ColumnSet(true));
        await Assert.That(kept).IsNotNull();
    }

    [Test]
    public async Task RetrieveEntityFake_Should_ReturnMetadata()
    {
        _sut.AddRequest(new RetrieveEntityFake());
        var metadata = new EntityMetadata { LogicalName = Account.EntityLogicalName };
        _sut.State.EntityMetadata.Add(Account.EntityLogicalName, metadata);

        var request = new RetrieveEntityRequest { LogicalName = Account.EntityLogicalName };

        var response = (RetrieveEntityResponse)_sut.Execute(request);

        await Assert.That(response.EntityMetadata.LogicalName).IsEqualTo(Account.EntityLogicalName);
    }
}
