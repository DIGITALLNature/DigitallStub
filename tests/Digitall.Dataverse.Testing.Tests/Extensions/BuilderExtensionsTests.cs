// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using TUnit.Core;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class BuilderExtensionsTests
{
    [Test]
    public void FakeDataverseBuilderExtensions_AddData_ShouldAddRecords()
    {
        var entity = new Entity("account", Guid.NewGuid());
        var builder = new FakeDataverseBuilder();

        builder.AddData(entity);

        var service = builder.GetOrganizationService();
        service.State.Entities["account"].Should().ContainKey(entity.Id);
    }

    [Test]
    public void FakeDataverseBuilderExtensions_AddConfig_ShouldAddEnvironmentVariables()
    {
        var builder = new FakeDataverseBuilder();

        builder.AddConfig("my_key", "default_val", "override_val");

        var service = builder.GetOrganizationService();
        service.State.Entities.Should().ContainKey("environmentvariabledefinition");
        service.State.Entities.Should().ContainKey("environmentvariablevalue");
    }
}
