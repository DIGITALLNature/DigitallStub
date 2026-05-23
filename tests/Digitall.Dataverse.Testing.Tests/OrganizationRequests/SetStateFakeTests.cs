// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class SetStateFakeTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new SetStateFake());
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_SetsStateCodeAndStatusCode()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        _sut.Execute(new SetStateRequest
        {
            EntityMoniker = new EntityReference("account", id),
            State = new OptionSetValue(1),
            Status = new OptionSetValue(2)
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        await Assert.That(result.GetAttributeValue<OptionSetValue>("statecode")!.Value).IsEqualTo(1);
        await Assert.That(result.GetAttributeValue<OptionSetValue>("statuscode")!.Value).IsEqualTo(2);
    }

    [Test]
    public async Task Execute_StatusMinusOne_UsesDefaultStatusCode()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        _sut.Execute(new SetStateRequest
        {
            EntityMoniker = new EntityReference("account", id),
            State = new OptionSetValue(0),
            Status = new OptionSetValue(-1)
        });

        var result = _sut.Retrieve("account", id, new ColumnSet(true));
        await Assert.That(result.GetAttributeValue<OptionSetValue>("statecode")!.Value).IsEqualTo(0);
        // Default status code is 1 when no metadata is registered
        await Assert.That(result.GetAttributeValue<OptionSetValue>("statuscode")!.Value).IsEqualTo(1);
    }

    [Test]
    public async Task Execute_NonExistingEntity_ThrowsFault()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        void Action() => _sut.Execute(new SetStateRequest
        {
            EntityMoniker = new EntityReference("account", Guid.NewGuid()),
            State = new OptionSetValue(1),
            Status = new OptionSetValue(2)
        });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Execute_ReturnsSetStateResponse()
    {
        var id = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = id });

        var response = _sut.Execute(new SetStateRequest
        {
            EntityMoniker = new EntityReference("account", id),
            State = new OptionSetValue(0),
            Status = new OptionSetValue(1)
        });

        await Assert.That(response).IsTypeOf<SetStateResponse>();
    }
}
