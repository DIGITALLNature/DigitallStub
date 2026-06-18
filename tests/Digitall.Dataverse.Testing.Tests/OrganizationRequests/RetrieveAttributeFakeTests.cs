// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class RetrieveAttributeFakeTests
{
    private FakeOrganizationService _sut = new();

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveAttributeFake());
        await Task.CompletedTask;
    }

    private static EntityMetadata CreateMetadataWithAttribute(string entityLogicalName, AttributeMetadata attribute)
    {
        var metadata = new EntityMetadata { LogicalName = entityLogicalName };

        // Use reflection to set Attributes since it has no public setter
        var attributesProperty = typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.Attributes))
                                 ?? throw new InvalidOperationException($"Property {nameof(EntityMetadata.Attributes)} was not found.");
        attributesProperty.SetValue(metadata, new[] { attribute });

        return metadata;
    }

    [Test]
    public async Task Execute_RegisteredAttribute_ReturnsAttributeMetadata()
    {
        var nameAttribute = new StringAttributeMetadata("name") { LogicalName = "name" };
        _sut.AddMetadata(CreateMetadataWithAttribute("account", nameAttribute));

        var response = (RetrieveAttributeResponse)_sut.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = "account",
            LogicalName = "name"
        });

        await Assert.That(response.AttributeMetadata).IsNotNull();
        await Assert.That(response.AttributeMetadata.LogicalName).IsEqualTo("name");
    }

    [Test]
    public async Task Execute_NonRegisteredEntity_ThrowsException()
    {
        var nameAttribute = new StringAttributeMetadata("name") { LogicalName = "name" };
        _sut.AddMetadata(CreateMetadataWithAttribute("account", nameAttribute));

        Assert.Throws<KeyNotFoundException>(() => _sut.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = "contact",
            LogicalName = "name"
        }));
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_NonRegisteredAttribute_ThrowsException()
    {
        var nameAttribute = new StringAttributeMetadata("name") { LogicalName = "name" };
        _sut.AddMetadata(CreateMetadataWithAttribute("account", nameAttribute));

        Assert.Throws<InvalidOperationException>(() => _sut.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = "account",
            LogicalName = "missing"
        }));
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_NullRequest_ThrowsArgumentNull()
    {
        OrganizationRequest? request = null;

        Assert.Throws<ArgumentNullException>(() => _sut.Execute(request!));
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_WrongRequestType_ThrowsInvalidCast()
    {
        Assert.Throws<InvalidCastException>(() => new RetrieveAttributeFake().Execute(new RetrieveEntityRequest(), _sut));
        await Task.CompletedTask;
    }

    [Test]
    public async Task Execute_ReturnsCorrectResponseType()
    {
        var nameAttribute = new StringAttributeMetadata("name") { LogicalName = "name" };
        _sut.AddMetadata(CreateMetadataWithAttribute("account", nameAttribute));

        var response = _sut.Execute(new RetrieveAttributeRequest
        {
            EntityLogicalName = "account",
            LogicalName = "name"
        });

        await Assert.That(response).IsTypeOf<RetrieveAttributeResponse>();
    }
}
