// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class BulkDeleteFakeTests
{
    private FakeOrganizationService _sut = null!;
    private Guid _userId;

    [Before(Test)]
    public async Task Setup()
    {
        _userId = Guid.NewGuid();
        _sut = new FakeOrganizationService
        {
            Options =
            {
                UserId = _userId
            }
        };
        _sut.AddRequest(new BulkDeleteFake());
        _sut.AddRequest(new RetrieveMultipleFake());
        await Task.CompletedTask;
    }

    private static BulkDeleteRequest BuildRequest(string jobName = "Test Job") =>
        new()
        {
            JobName = jobName,
            QuerySet = [],
            CCRecipients = [],
            ToRecipients = []
        };

    [Test]
    public async Task Execute_ValidRequest_ReturnsJobId()
    {
        var response = (BulkDeleteResponse)_sut.Execute(BuildRequest());

        await Assert.That(response).IsNotNull();
        await Assert.That(response["JobId"]).IsNotNull();
        await Assert.That((Guid)response["JobId"]).IsNotEqualTo(Guid.Empty);
    }

    [Test]
    public async Task Execute_ValidRequest_AsyncOperationHasName()
    {
        var response = (BulkDeleteResponse)_sut.Execute(BuildRequest("My Bulk Delete"));
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));

        await Assert.That(asyncOp.GetAttributeValue<string>("name")).IsEqualTo("My Bulk Delete");
    }

    [Test]
    public async Task Execute_ValidRequest_AsyncOperationHasOwner()
    {
        var response = (BulkDeleteResponse)_sut.Execute(BuildRequest());
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));
        var ownerId = asyncOp.GetAttributeValue<EntityReference>("ownerid");

        await Assert.That(ownerId).IsNotNull();
        await Assert.That(ownerId!.LogicalName).IsEqualTo("systemuser");
        await Assert.That(ownerId.Id).IsEqualTo(_userId);
    }

    [Test]
    public async Task Execute_ValidRequest_AsyncOperationHasBulkDeleteOperationType()
    {
        var response = (BulkDeleteResponse)_sut.Execute(BuildRequest());
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));
        var operationType = asyncOp.GetAttributeValue<OptionSetValue>("operationtype");

        await Assert.That(operationType).IsNotNull();
        await Assert.That(operationType!.Value).IsEqualTo(13);
    }

    [Test]
    public async Task Execute_ValidRequest_AsyncOperationHasRecurrencePattern()
    {
        var request = new BulkDeleteRequest
        {
            JobName = "Recurring Delete",
            QuerySet = [],
            CCRecipients = [],
            ToRecipients = [],
            RecurrencePattern = "FREQ=DAILY;INTERVAL=1"
        };

        var response = (BulkDeleteResponse)_sut.Execute(request);
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));

        await Assert.That(asyncOp.GetAttributeValue<string>("recurrencepattern")).IsEqualTo("FREQ=DAILY;INTERVAL=1");
    }

    [Test]
    public async Task Execute_ValidRequest_AsyncOperationHasRecurrenceStartTime()
    {
        var startTime = new DateTime(2025, 6, 1, 8, 0, 0, DateTimeKind.Utc);
        var request = new BulkDeleteRequest
        {
            JobName = "Scheduled Delete",
            QuerySet = [],
            CCRecipients = [],
            ToRecipients = [],
            StartDateTime = startTime
        };

        var response = (BulkDeleteResponse)_sut.Execute(request);
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));

        await Assert.That(asyncOp.GetAttributeValue<DateTime>("recurrencestarttime")).IsEqualTo(startTime);
    }

    [Test]
    public async Task Execute_WithQuerySet_DeletesMatchingRecords()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        var request = new BulkDeleteRequest
        {
            JobName = "Delete Accounts",
            QuerySet = [new QueryExpression("account")],
            CCRecipients = [],
            ToRecipients = []
        };

        _sut.Execute(request);

        var remaining = _sut.CreateQuery("account").Where(e => e.Id == accountId).ToList();
        await Assert.That(remaining).IsEmpty();
    }

    [Test]
    public async Task Execute_WithDefaultStartDateTime_DoesNotSetRecurrenceStartTime()
    {
        var request = BuildRequest();

        var response = (BulkDeleteResponse)_sut.Execute(request);
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));

        await Assert.That(asyncOp.Attributes.ContainsKey("recurrencestarttime")).IsFalse();
    }

    [Test]
    public async Task Execute_WithNullRecurrencePattern_DoesNotSetRecurrencePattern()
    {
        var request = BuildRequest();

        var response = (BulkDeleteResponse)_sut.Execute(request);
        var jobId = (Guid)response["JobId"];

        var asyncOp = _sut.Retrieve("asyncoperation", jobId, new ColumnSet(true));

        await Assert.That(asyncOp.Attributes.ContainsKey("recurrencepattern")).IsFalse();
    }
}
