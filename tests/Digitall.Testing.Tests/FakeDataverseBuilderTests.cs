// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using AwesomeAssertions;
using Digitall.Testing.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests;

[TestClass]
public class FakeDataverseBuilderTests
{
    [TestMethod]
    public void GetOrganizationService_Should_Return_FakeOrganizationServiceAsync()
    {
        var service = new FakeDataverseBuilder().GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationServiceAsync>();
    }

    [TestMethod]
    public void FakeDataverseBuilder_With_Custom_TimeProvider()
    {
        var service = new FakeDataverseBuilder(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationServiceAsync>();
        service.TimeProvider.GetUtcNow().Year.Should().Be(2000);
    }

    [TestMethod]
    public void FakeDataverseBuilder_Should_Register_DefaultRequests()
    {
        var service = new FakeDataverseBuilder().GetOrganizationService();

        var result = service.Execute(new WhoAmIRequest());

        result.Should().NotBeNull().And.BeOfType<WhoAmIResponse>();
    }

    [TestMethod]
    public void AddData_Should_BeRetrieved_From_OrganizationService()
    {
        var entity = new Entity("unittest", Guid.NewGuid()) { ["name"] = "builder-record" };

        var service = new FakeDataverseBuilder()
            .AddData(entity)
            .GetOrganizationService();

        var retrieved = service.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));

        retrieved.Should().NotBeNull();
        retrieved.Should().BeEquivalentTo(entity);
    }

    [TestMethod]
    public void AddConfig_Should_Add_Definition_And_Value()
    {
        var service = new FakeDataverseBuilder()
            .AddConfig("dg_TestFlag", "default", "override")
            .GetOrganizationService();

        var definition = service.CreateQuery("environmentvariabledefinition").Single();
        var value = service.CreateQuery("environmentvariablevalue").Single();

        definition.GetAttributeValue<string>("schemaname").Should().Be("dg_TestFlag");
        definition.GetAttributeValue<string>("defaultvalue").Should().Be("default");
        value.GetAttributeValue<string>("value").Should().Be("override");
        value.GetAttributeValue<EntityReference>("environmentvariabledefinitionid").Id.Should().Be(definition.Id);
    }

    [TestMethod]
    public void AddEntityMetadata_And_Relationships_Should_BeStored_In_Service()
    {
        var accountMetadata = new EntityMetadata
        {
            LogicalName = "account"
        };

        // SDK relationship collections are not publicly settable, so initialize them via reflection.
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

        service.EntityMetadata.Should().ContainKey("account");
        service.Relationships.Should().ContainKey("dg_account_contact");
    }
}
