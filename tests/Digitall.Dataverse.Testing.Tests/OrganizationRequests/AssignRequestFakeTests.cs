// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class AssignRequestFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new AssignRequestFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_AssignToSystemUser_SetsOwnerIdAndOwningUser()
    {
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        _sut.Execute(new AssignRequest
        {
            Target = new EntityReference("account", accountId),
            Assignee = new EntityReference("systemuser", userId)
        });

        var result = _sut.Retrieve("account", accountId, new ColumnSet(true));
        var ownerId = result.GetAttributeValue<EntityReference>("ownerid");
        var owningUser = result.GetAttributeValue<EntityReference>("owninguser");

        await Assert.That(ownerId).IsNotNull();
        await Assert.That(ownerId!.Id).IsEqualTo(userId);
        await Assert.That(ownerId.LogicalName).IsEqualTo("systemuser");
        await Assert.That(owningUser).IsNotNull();
        await Assert.That(owningUser!.Id).IsEqualTo(userId);
    }

    [Test]
    public async Task Execute_AssignToTeam_SetsOwnerIdAndOwningTeam()
    {
        var accountId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        _sut.Execute(new AssignRequest
        {
            Target = new EntityReference("account", accountId),
            Assignee = new EntityReference("team", teamId)
        });

        var result = _sut.Retrieve("account", accountId, new ColumnSet(true));
        var ownerId = result.GetAttributeValue<EntityReference>("ownerid");
        var owningTeam = result.GetAttributeValue<EntityReference>("owningteam");

        await Assert.That(ownerId).IsNotNull();
        await Assert.That(ownerId!.Id).IsEqualTo(teamId);
        await Assert.That(ownerId.LogicalName).IsEqualTo("team");
        await Assert.That(owningTeam).IsNotNull();
        await Assert.That(owningTeam!.Id).IsEqualTo(teamId);
    }

    [Test]
    public async Task Execute_NonExistingEntity_ThrowsFault()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        void Action() => _sut.Execute(new AssignRequest
        {
            Target = new EntityReference("account", Guid.NewGuid()),
            Assignee = new EntityReference("systemuser", Guid.NewGuid())
        });

        Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_NullTarget_ThrowsFault()
    {
        void Action() => _sut.Execute(new AssignRequest
        {
            Target = null,
            Assignee = new EntityReference("systemuser", Guid.NewGuid())
        });

        Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_NullAssignee_ThrowsFault()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        void Action() => _sut.Execute(new AssignRequest
        {
            Target = new EntityReference("account", accountId),
            Assignee = null
        });

        Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_ReturnsAssignResponse()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId });

        var response = _sut.Execute(new AssignRequest
        {
            Target = new EntityReference("account", accountId),
            Assignee = new EntityReference("systemuser", Guid.NewGuid())
        });

        await Assert.That(response).IsTypeOf<AssignResponse>();
    }
}
