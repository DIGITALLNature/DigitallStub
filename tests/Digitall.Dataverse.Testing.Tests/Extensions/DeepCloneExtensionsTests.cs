// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class DeepCloneExtensionsTests
{
    [Test]
    public async Task DeepClone_Entity_PreservesAllAttributes()
    {
        var entity = new Entity("account", Guid.NewGuid());
        entity["name"] = "Test Account";
        entity["revenue"] = new Money(1000m);

        var clone = entity.DeepClone();

        await Assert.That(clone!.Attributes).HasCount().EqualTo(2);
        await Assert.That(clone["name"]).IsEqualTo("Test Account");
        await Assert.That(((Money)clone["revenue"]).Value).IsEqualTo(1000m);
    }

    [Test]
    public async Task DeepClone_Entity_CreatesIndependentCopy()
    {
        var entity = new Entity("account", Guid.NewGuid());
        entity["name"] = "Original";

        var clone = entity.DeepClone();
        clone!["name"] = "Modified";

        await Assert.That(entity["name"]).IsEqualTo("Original");
        await Assert.That(clone["name"]).IsEqualTo("Modified");
    }

    [Test]
    public async Task DeepClone_Null_ReturnsNull()
    {
        Entity? entity = null;

        var clone = entity.DeepClone();

        await Assert.That(clone).IsNull();
    }

    [Test]
    public async Task DeepClone_Entity_WithEntityReference_PreservesReferences()
    {
        var refId = Guid.NewGuid();
        var entity = new Entity("contact", Guid.NewGuid());
        entity["parentcustomerid"] = new EntityReference("account", refId);

        var clone = entity.DeepClone();

        var clonedRef = (EntityReference)clone!["parentcustomerid"];
        await Assert.That(clonedRef.LogicalName).IsEqualTo("account");
        await Assert.That(clonedRef.Id).IsEqualTo(refId);
    }

    [Test]
    public async Task DeepClone_Entity_WithOptionSetValue_PreservesValue()
    {
        var entity = new Entity("account", Guid.NewGuid());
        entity["statecode"] = new OptionSetValue(1);

        var clone = entity.DeepClone();

        var clonedOption = (OptionSetValue)clone!["statecode"];
        await Assert.That(clonedOption.Value).IsEqualTo(1);
    }

    [Test]
    public async Task DeepClone_Entity_PreservesIdAndLogicalName()
    {
        var id = Guid.NewGuid();
        var entity = new Entity("opportunity", id);

        var clone = entity.DeepClone();

        await Assert.That(clone!.Id).IsEqualTo(id);
        await Assert.That(clone.LogicalName).IsEqualTo("opportunity");
    }
}
