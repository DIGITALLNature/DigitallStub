// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using System.Threading.Tasks;
using Digitall.Testing.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests;

public class FakeDataverseBuilderTests
{
    [Test]
    public async Task GetOrganizationService_Should_Return_FakeOrganizationServiceAsync()
    {
        var service = new FakeDataverseBuilder().GetOrganizationService();

        await Assert.That(service).IsNotNull();
        await Assert.That(service).IsTypeOf<FakeOrganizationServiceAsync>();
    }

    [Test]
    public async Task FakeDataverseBuilder_With_Custom_TimeProvider()
    {
        var service = new FakeDataverseBuilder(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetOrganizationService();

        await Assert.That(service).IsNotNull();
        await Assert.That(service).IsTypeOf<FakeOrganizationServiceAsync>();
        await Assert.That(service.TimeProvider.GetUtcNow().Year).IsEqualTo(2000);
    }

    [Test]
    public async Task FakeDataverseBuilder_Should_Register_DefaultRequests()
    {
        var service = new FakeDataverseBuilder().GetOrganizationService();

        var result = service.Execute(new WhoAmIRequest());

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsTypeOf<WhoAmIResponse>();
    }

    [Test]
    public async Task AddData_Should_BeRetrieved_From_OrganizationService()
    {
        var entity = new Entity("unittest", Guid.NewGuid()) { ["name"] = "builder-record" };

        var service = new FakeDataverseBuilder()
            .AddData(entity)
            .GetOrganizationService();

        var retrieved = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));

        await Assert.That(retrieved).IsNotNull();
        await Assert.That(retrieved).IsEquivalentTo(entity);
    }

    [Test]
    public async Task AddConfig_Should_Add_Definition_And_Value()
    {
        var service = new FakeDataverseBuilder()
            .AddConfig("dg_TestFlag", "default", "override")
            .GetOrganizationService();

        var definition = service.CreateQuery("environmentvariabledefinition").Single();
        var value = service.CreateQuery("environmentvariablevalue").Single();

        await Assert.That(definition.GetAttributeValue<string>("schemaname")).IsEqualTo("dg_TestFlag");
        await Assert.That(definition.GetAttributeValue<string>("defaultvalue")).IsEqualTo("default");
        await Assert.That(value.GetAttributeValue<string>("value")).IsEqualTo("override");
        await Assert.That(value.GetAttributeValue<EntityReference>("environmentvariabledefinitionid").Id).IsEqualTo(definition.Id);
    }

    [Test]
    public async Task AddEntityMetadata_And_Relationships_Should_BeStored_In_Service()
    {
        var accountMetadata = new EntityMetadata
        {
            LogicalName = "account"
        };

        typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.ManyToManyRelationships))!
            .SetValue(accountMetadata, Array.Empty<ManyToManyRelationshipMetadata>());
        typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.OneToManyRelationships))!
            .SetValue(accountMetadata, Array.Empty<OneToManyRelationshipMetadata>());
        typeof(EntityMetadata).GetProperty(nameof(EntityMetadata.ManyToOneRelationships))!
            .SetValue(accountMetadata, Array.Empty<OneToManyRelationshipMetadata>());

        var relationship = new OneToManyRelationshipMetadata { SchemaName = "dg_account_contact" };

        var service = new FakeDataverseBuilder()
            .AddEntityMetadata(accountMetadata)
            .AddRelationships(relationship)
            .GetOrganizationService();

        await Assert.That(service.EntityMetadata.ContainsKey("account")).IsTrue();
        await Assert.That(service.Relationships.ContainsKey("dg_account_contact")).IsTrue();
    }
}
