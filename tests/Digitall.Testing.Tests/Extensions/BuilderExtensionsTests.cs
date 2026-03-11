// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Tests.Extensions;

[TestClass]
public class BuilderExtensionsTests
{
    [TestMethod]
    public void PluginExecutionContextBuilderExtensions_WithTarget_ShouldSetTarget()
    {
        var target = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithTarget(target);

        builder.Target.Should().NotBeNull();
        builder.Target.Value.Should().Be(target);
    }

    [TestMethod]
    public void PluginExecutionContextBuilderExtensions_WithPreEntityImage_ShouldAddImage()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithPreEntityImage(image, "MyImage");

        builder.PreEntityImages.Should().ContainKey("MyImage");
        builder.PreEntityImages["MyImage"].Should().Be(image);
    }

    [TestMethod]
    public void PluginExecutionContextBuilderExtensions_WithInputParameter_ShouldAddParameter()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithInputParameter("MyParam", "MyValue");

        builder.InputParameters.Should().ContainKey("MyParam");
        builder.InputParameters["MyParam"].Should().Be("MyValue");
    }

    [TestMethod]
    public void PluginExecutionContextBuilderExtensions_WithMessageName_ShouldSetMessageName()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithMessageName("Update");

        builder.MessageName.Should().Be("Update");
    }

    [TestMethod]
    public void FakeDataverseBuilderExtensions_AddData_ShouldAddRecords()
    {
        var entity = new Entity("account", Guid.NewGuid());
        var builder = new FakeDataverseBuilder();

        builder.AddData(entity);

        var service = builder.GetOrganizationService();
        service.InternalState["account"].Should().ContainKey(entity.Id);
    }

    [TestMethod]
    public void FakeDataverseBuilderExtensions_AddConfig_ShouldAddEnvironmentVariables()
    {
        var builder = new FakeDataverseBuilder();

        builder.AddConfig("my_key", "default_val", "override_val");

        var service = builder.GetOrganizationService();
        service.InternalState.Should().ContainKey("environmentvariabledefinition");
        service.InternalState.Should().ContainKey("environmentvariablevalue");
    }
}
