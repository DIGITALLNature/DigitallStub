// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class RetrieveAllEntitiesTests
{
    [Test]
    public async Task Execute_Should_Return_Empty_When_No_Metadata_Registered()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new RetrieveAllEntitiesFake());

        var response = (RetrieveAllEntitiesResponse)sut.Execute(new RetrieveAllEntitiesRequest());

        await Assert.That(response).IsNotNull();
        await Assert.That(response.EntityMetadata).IsNotNull();
        await Assert.That(response.EntityMetadata).IsEmpty();
    }

    [Test]
    public async Task Execute_Should_Return_All_Registered_EntityMetadata()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new RetrieveAllEntitiesFake());

        var accountMetadata = new EntityMetadata { LogicalName = "account" };
        var contactMetadata = new EntityMetadata { LogicalName = "contact" };
        sut.State.EntityMetadata["account"] = accountMetadata;
        sut.State.EntityMetadata["contact"] = contactMetadata;

        var response = (RetrieveAllEntitiesResponse)sut.Execute(new RetrieveAllEntitiesRequest());

        await Assert.That(response.EntityMetadata).HasCount(2);
        await Assert.That(response.EntityMetadata.Select(m => m.LogicalName)).Contains("account");
        await Assert.That(response.EntityMetadata.Select(m => m.LogicalName)).Contains("contact");
    }

    [Test]
    public async Task Execute_Should_Return_Registered_Instance_Reference()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new RetrieveAllEntitiesFake());

        var metadata = new EntityMetadata { LogicalName = "account" };
        sut.State.EntityMetadata["account"] = metadata;

        var response = (RetrieveAllEntitiesResponse)sut.Execute(new RetrieveAllEntitiesRequest());

        await Assert.That(response.EntityMetadata).HasCount(1);
        await Assert.That(response.EntityMetadata[0]).IsSameReferenceAs(metadata);
    }

    [Test]
    public async Task Execute_Should_Ignore_Filters_And_Return_All_Metadata()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new RetrieveAllEntitiesFake());

        sut.State.EntityMetadata["account"] = new EntityMetadata { LogicalName = "account" };
        sut.State.EntityMetadata["contact"] = new EntityMetadata { LogicalName = "contact" };

        var request = new RetrieveAllEntitiesRequest
        {
            EntityFilters = EntityFilters.Attributes,
            RetrieveAsIfPublished = true
        };

        var response = (RetrieveAllEntitiesResponse)sut.Execute(request);

        await Assert.That(response.EntityMetadata).HasCount(2);
    }
}
