// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class SpyOrganizationRequestFakeTests
{
    [Test]
    public async Task ReceivedRequests_IsEmpty_OnConstruction()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();

        await Assert.That(sut.ReceivedRequests).IsEmpty();
    }

    [Test]
    public async Task Execute_WithNoHandler_ReturnsDefaultResponse()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();
        var service = new FakeOrganizationService();

        var response = sut.Execute(new WhoAmIRequest(), service);

        await Assert.That(response).IsNotNull();
        await Assert.That(response).IsTypeOf<WhoAmIResponse>();
    }

    [Test]
    public async Task Execute_RecordsRequest_InReceivedRequests()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();
        var request = new WhoAmIRequest();
        var service = new FakeOrganizationService();

        sut.Execute(request, service);

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(sut.ReceivedRequests[0]).IsEqualTo(request);
    }

    [Test]
    public async Task Execute_MultipleTimes_AccumulatesAllRequests()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();
        var service = new FakeOrganizationService();
        var request1 = new WhoAmIRequest();
        var request2 = new WhoAmIRequest();
        var request3 = new WhoAmIRequest();

        sut.Execute(request1, service);
        sut.Execute(request2, service);
        sut.Execute(request3, service);

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(3);
        await Assert.That(sut.ReceivedRequests[0]).IsEqualTo(request1);
        await Assert.That(sut.ReceivedRequests[1]).IsEqualTo(request2);
        await Assert.That(sut.ReceivedRequests[2]).IsEqualTo(request3);
    }

    [Test]
    public async Task Execute_WithHandler_InvokesHandlerAndReturnsItsResult()
    {
        var expectedResponse = new WhoAmIResponse();
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>(
            (_, _) => expectedResponse);

        var response = sut.Execute(new WhoAmIRequest(), new FakeOrganizationService());

        await Assert.That(response).IsEqualTo(expectedResponse);
    }

    [Test]
    public async Task Execute_WithHandler_PassesRequestAndServiceToHandler()
    {
        WhoAmIRequest? capturedRequest = null;
        FakeOrganizationService? capturedService = null;

        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>(
            (req, svc) =>
            {
                capturedRequest = req;
                capturedService = svc;
                return new WhoAmIResponse();
            });

        var request = new WhoAmIRequest();
        var service = new FakeOrganizationService();

        sut.Execute(request, service);

        await Assert.That(capturedRequest).IsNotNull();
        await Assert.That(capturedRequest).IsEqualTo(request);
        await Assert.That(capturedService).IsNotNull();
        await Assert.That(capturedService).IsEqualTo(service);
    }

    [Test]
    public async Task Execute_WithHandler_StillRecordsRequest()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>(
            (_, _) => new WhoAmIResponse());

        var request = new WhoAmIRequest();
        sut.Execute(request, new FakeOrganizationService());

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(sut.ReceivedRequests[0]).IsEqualTo(request);
    }

    [Test]
    public async Task AddRequest_DispatchesViaFakeOrganizationService_AndRecordsRequest()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(sut);

        service.Execute(new WhoAmIRequest());

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(1);
    }

    [Test]
    public async Task AddRequest_DispatchedMultipleTimes_AccumulatesAllRequestsViaService()
    {
        var sut = new SpyOrganizationRequestFake<WhoAmIRequest, WhoAmIResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(sut);

        service.Execute(new WhoAmIRequest());
        service.Execute(new WhoAmIRequest());

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Execute_WithCreateRequest_TracksTypedRequest()
    {
        var sut = new SpyOrganizationRequestFake<CreateRequest, CreateResponse>();
        var service = new FakeOrganizationService();
        service.AddRequest(sut);

        var account = new Account { Name = "Test" };
        service.Execute(new CreateRequest { Target = account });

        await Assert.That(sut.ReceivedRequests).Count().IsEqualTo(1);
        await Assert.That(sut.ReceivedRequests[0].Target).IsEqualTo(account);
    }
}
