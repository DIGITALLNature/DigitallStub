// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using TUnit.Core;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class EntityExtensionsTests
{
    private readonly FakeOrganizationService _state = new();

    [Test]
    public void KeySelector_Should_ReturnId_When_AttributeIsPrimaryKey()
    {
        var id = Guid.NewGuid();
        var entity = new Entity("account", id);

        var result = entity.KeySelector("accountid");

        result.Should().Be(id);
    }

    [Test]
    public void KeySelector_Should_ReturnEmptyGuid_When_AttributeDoesNotExist()
    {
        var entity = new Entity("account", Guid.NewGuid());

        var result = entity.KeySelector("nonexistent");

        result.Should().Be(Guid.Empty);
    }

    [Test]
    public void KeySelector_Should_HandleAliasedValue()
    {
        var entity = new Entity("account") { ["alias.name"] = new AliasedValue("account", "name", "John Doe") };

        var result = entity.KeySelector("alias.name");

        result.Should().Be("John Doe");
    }

    [Test]
    public void KeySelector_Should_HandleEntityReference()
    {
        var refId = Guid.NewGuid();
        var entity = new Entity("account") { ["parentaccountid"] = new EntityReference("account", refId) };

        var result = entity.KeySelector("parentaccountid");

        result.Should().Be(refId);
    }

    [Test]
    public void KeySelector_Should_HandleOptionSetValue()
    {
        var entity = new Entity("account") { ["statuscode"] = new OptionSetValue(1) };

        var result = entity.KeySelector("statuscode");

        result.Should().Be(1);
    }

    [Test]
    public void KeySelector_Should_HandleMoney()
    {
        var entity = new Entity("account") { ["creditlimit"] = new Money(1000m) };

        var result = entity.KeySelector("creditlimit");

        result.Should().Be(1000m);
    }

    [Test]
    public void ProjectAttributes_WithColumnSet_Should_ProjectRequestedAttributes()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["telephone1"] = "123456", ["websiteurl"] = "http://example.com" };

        var columnSet = new ColumnSet("name", "telephone1");

        var projected = entity.ProjectAttributes(columnSet, _state);

        projected.Attributes.Count.Should().Be(2);
        projected.Attributes.Should().ContainKey("name");
        projected.Attributes.Should().ContainKey("telephone1");
        projected.Attributes.Should().NotContainKey("websiteurl");
    }

    [Test]
    public void ProjectAttributes_WithAllColumns_Should_ReturnAllNonNullAttributes()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["telephone1"] = null };

        var columnSet = new ColumnSet(true);

        var projected = entity.ProjectAttributes(columnSet, _state);

        projected.Attributes.Count.Should().Be(1);
        projected.Attributes.Should().ContainKey("name");
        projected.Attributes.Should().NotContainKey("telephone1");
    }

    [Test]
    public void ProjectAttributes_WithQueryExpression_Should_HandleLinkEntities()
    {
        var account = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["contact.fullname"] = new AliasedValue("contact", "fullname", "John Smith") };

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet("name") };
        var le = qe.AddLink("contact", "accountid", "parentcustomerid");
        le.EntityAlias = "contact";
        le.Columns = new ColumnSet("fullname");

        var projected = account.ProjectAttributes(qe, _state);

        projected.Attributes.Should().ContainKey("name");
        projected.Attributes.Should().ContainKey("contact.fullname");
    }

    [Test]
    public void CloneEntity_Should_CreateDeepCopy()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["ref"] = new EntityReference("contact", Guid.NewGuid()) };

        var cloned = entity.CloneEntity();

        cloned.Should().NotBeSameAs(entity);
        cloned.Id.Should().Be(entity.Id);
        cloned["name"].Should().Be(entity["name"]);
        cloned["ref"].Should().NotBeSameAs(entity["ref"]);
        ((EntityReference)cloned["ref"]).Id.Should().Be(((EntityReference)entity["ref"]).Id);
    }

    [Test]
    public void JoinAttributes_Should_AddAliasedValues()
    {
        var mainEntity = new Entity("account", Guid.NewGuid());
        var otherEntity = new Entity("contact", Guid.NewGuid()) { ["firstname"] = "John" };

        mainEntity.JoinAttributes(otherEntity, new ColumnSet("firstname"), "c");

        mainEntity.Attributes.Should().ContainKey("c.firstname");
        var aliased = (AliasedValue)mainEntity["c.firstname"];
        aliased.EntityLogicalName.Should().Be("contact");
        aliased.AttributeLogicalName.Should().Be("firstname");
        aliased.Value.Should().Be("John");
    }
}
