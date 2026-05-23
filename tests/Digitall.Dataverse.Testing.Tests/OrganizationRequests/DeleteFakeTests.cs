// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class DeleteFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new DeleteFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_ExistingEntity_RemovesFromState()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        _sut.Execute(new DeleteRequest { Target = new EntityReference("account", id) });

        var remaining = _sut.CreateQuery("account").ToList();
        await Assert.That(remaining).IsEmpty();
    }

    [Test]
    public async Task Execute_NonExistingEntity_ThrowsFault()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        var nonExistingId = Guid.NewGuid();
        void Action() => _sut.Execute(new DeleteRequest { Target = new EntityReference("account", nonExistingId) });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Execute_UnknownEntityType_ThrowsFault()
    {
        void Action() => _sut.Execute(new DeleteRequest { Target = new EntityReference("unknown_entity", Guid.NewGuid()) });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public async Task Execute_MultipleEntities_OnlyDeletesTarget()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id1 });
        _sut.Add(new Entity("account") { Id = id2 });

        _sut.Execute(new DeleteRequest { Target = new EntityReference("account", id1) });

        var remaining = _sut.CreateQuery("account").ToList();
        await Assert.That(remaining).HasCount().EqualTo(1);
        await Assert.That(remaining[0].Id).IsEqualTo(id2);
    }

    [Test]
    public async Task Execute_ReturnsDeleteResponse()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        var response = _sut.Execute(new DeleteRequest { Target = new EntityReference("account", id) });

        await Assert.That(response).IsTypeOf<DeleteResponse>();
    }
}
