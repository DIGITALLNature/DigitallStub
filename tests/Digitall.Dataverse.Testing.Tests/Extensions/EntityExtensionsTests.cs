// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class EntityExtensionsTests
{
    private readonly FakeOrganizationService _state = new();

    [Test]
    public async Task KeySelector_Should_ReturnId_When_AttributeIsPrimaryKey()
    {
        var id = Guid.NewGuid();
        var entity = new Entity("account", id);

        var result = entity.KeySelector("accountid");

        await Assert.That(result).IsEqualTo(id);
    }

    [Test]
    public async Task KeySelector_Should_ReturnEmptyGuid_When_AttributeDoesNotExist()
    {
        var entity = new Entity("account", Guid.NewGuid());

        var result = entity.KeySelector("nonexistent");

        await Assert.That(result).IsEqualTo(Guid.Empty);
    }

    [Test]
    public async Task KeySelector_Should_HandleAliasedValue()
    {
        var entity = new Entity("account") { ["alias.name"] = new AliasedValue("account", "name", "John Doe") };

        var result = entity.KeySelector("alias.name");

        await Assert.That(result).IsEqualTo("John Doe");
    }

    [Test]
    public async Task KeySelector_Should_HandleEntityReference()
    {
        var refId = Guid.NewGuid();
        var entity = new Entity("account") { ["parentaccountid"] = new EntityReference("account", refId) };

        var result = entity.KeySelector("parentaccountid");

        await Assert.That(result).IsEqualTo(refId);
    }

    [Test]
    public async Task KeySelector_Should_HandleOptionSetValue()
    {
        var entity = new Entity("account") { ["statuscode"] = new OptionSetValue(1) };

        var result = entity.KeySelector("statuscode");

        await Assert.That(result).IsEqualTo(1);
    }

    [Test]
    public async Task KeySelector_Should_HandleMoney()
    {
        var entity = new Entity("account") { ["creditlimit"] = new Money(1000m) };

        var result = entity.KeySelector("creditlimit");

        await Assert.That(result).IsEqualTo(1000m);
    }

    [Test]
    public async Task ProjectAttributes_WithColumnSet_Should_ProjectRequestedAttributes()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["telephone1"] = "123456", ["websiteurl"] = "http://example.com" };

        var columnSet = new ColumnSet("name", "telephone1");

        var projected = entity.ProjectAttributes(columnSet, _state);

        await Assert.That(projected.Attributes.Count).IsEqualTo(2);
        await Assert.That(projected.Attributes.ContainsKey("name")).IsTrue();
        await Assert.That(projected.Attributes.ContainsKey("telephone1")).IsTrue();
        await Assert.That(projected.Attributes.ContainsKey("websiteurl")).IsFalse();
    }

    [Test]
    public async Task ProjectAttributes_WithAllColumns_Should_ReturnAllNonNullAttributes()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["telephone1"] = null };

        var columnSet = new ColumnSet(true);

        var projected = entity.ProjectAttributes(columnSet, _state);

        await Assert.That(projected.Attributes.Count).IsEqualTo(1);
        await Assert.That(projected.Attributes.ContainsKey("name")).IsTrue();
        await Assert.That(projected.Attributes.ContainsKey("telephone1")).IsFalse();
    }

    [Test]
    public async Task ProjectAttributes_WithQueryExpression_Should_HandleLinkEntities()
    {
        var account = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["contact.fullname"] = new AliasedValue("contact", "fullname", "John Smith") };

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet("name") };
        var le = qe.AddLink("contact", "accountid", "parentcustomerid");
        le.EntityAlias = "contact";
        le.Columns = new ColumnSet("fullname");

        var projected = account.ProjectAttributes(qe, _state);

        await Assert.That(projected.Attributes.ContainsKey("name")).IsTrue();
        await Assert.That(projected.Attributes.ContainsKey("contact.fullname")).IsTrue();
    }

    [Test]
    public async Task CloneEntity_Should_CreateDeepCopy()
    {
        var entity = new Entity("account", Guid.NewGuid()) { ["name"] = "Account 1", ["ref"] = new EntityReference("contact", Guid.NewGuid()) };

        var cloned = entity.CloneEntity();

        await Assert.That(cloned).IsNotSameReferenceAs(entity);
        await Assert.That(cloned.Id).IsEqualTo(entity.Id);
        await Assert.That(cloned["name"]).IsEqualTo(entity["name"]);
        await Assert.That(cloned["ref"]).IsNotSameReferenceAs(entity["ref"]);
        await Assert.That(((EntityReference)cloned["ref"]).Id).IsEqualTo(((EntityReference)entity["ref"]).Id);
    }

    [Test]
    public async Task CloneEntity_Should_ReturnEntityRuntimeType_WhenSourceIsDerived()
    {
        var derived = new DerivedEntity { Id = Guid.NewGuid(), ["name"] = "x" };

        var cloned = derived.CloneEntity();

        await Assert.That(cloned.GetType()).IsEqualTo(typeof(Entity));
        await Assert.That(cloned.LogicalName).IsEqualTo("account");
        await Assert.That(cloned.Id).IsEqualTo(derived.Id);
        await Assert.That(cloned["name"]).IsEqualTo("x");
    }

    private sealed class DerivedEntity : Entity
    {
        public DerivedEntity() : base("account")
        {
        }
    }

    [Test]
    public async Task JoinAttributes_Should_AddAliasedValues()
    {
        var mainEntity = new Entity("account", Guid.NewGuid());
        var otherEntity = new Entity("contact", Guid.NewGuid()) { ["firstname"] = "John" };

        mainEntity.JoinAttributes(otherEntity, new ColumnSet("firstname"), "c");

        await Assert.That(mainEntity.Attributes.ContainsKey("c.firstname")).IsTrue();
        var aliased = (AliasedValue)mainEntity["c.firstname"];
        await Assert.That(aliased.EntityLogicalName).IsEqualTo("contact");
        await Assert.That(aliased.AttributeLogicalName).IsEqualTo("firstname");
        await Assert.That(aliased.Value).IsEqualTo("John");
    }
}
