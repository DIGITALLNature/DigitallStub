// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class UpdateFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new UpdateFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_ExistingEntity_MergesAttributes()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id, ["name"] = "Old Name", ["revenue"] = 100m });

        _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = id, ["name"] = "New Name" }
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        await Assert.That(result.GetAttributeValue<string>("name")).IsEqualTo("New Name");
        await Assert.That(result.GetAttributeValue<decimal>("revenue")).IsEqualTo(100m);
    }

    [Test]
    public async Task Execute_ExistingEntity_AddsNewAttributes()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id, ["name"] = "Test" });

        _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = id, ["city"] = "Berlin" }
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        await Assert.That(result.GetAttributeValue<string>("name")).IsEqualTo("Test");
        await Assert.That(result.GetAttributeValue<string>("city")).IsEqualTo("Berlin");
    }

    [Test]
    public async Task Execute_NonExistingEntity_ThrowsFault()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        void Action() => _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "X" }
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Execute_NullTarget_ThrowsFault()
    {
        void Action() => _sut.Execute(new UpdateRequest { Target = null });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Execute_SetsModifiedOn()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id, ["name"] = "Test" });

        _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = id, ["name"] = "Updated" }
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        await Assert.That(result.GetAttributeValue<DateTime>("modifiedon")).IsNotEqualTo(default(DateTime));
    }

    [Test]
    public async Task Execute_SetsModifiedBy_WhenUserIdConfigured()
    {
        var userId = Guid.NewGuid();
        _sut.Options.UserId = userId;

        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id, ["name"] = "Test" });

        _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = id, ["name"] = "Updated" }
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        var modifiedBy = result.GetAttributeValue<EntityReference>("modifiedby");
        await Assert.That(modifiedBy).IsNotNull();
        await Assert.That(modifiedBy!.Id).IsEqualTo(userId);
    }

    [Test]
    public async Task Execute_ReturnsUpdateResponse()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        var response = _sut.Execute(new UpdateRequest
        {
            Target = new Entity("account") { Id = id, ["name"] = "X" }
        });

        await Assert.That(response).IsTypeOf<UpdateResponse>();
    }
}
