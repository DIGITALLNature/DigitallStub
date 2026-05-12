// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Digitall.Plugins.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Plugins.Testing.Tests.Extensions;

public class PluginExecutionContextBuilderTests
{
    [Test]
    public void PluginExecutionContextBuilderExtensions_WithTarget_ShouldSetTarget()
    {
        var target = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithTarget(target);

        builder.Target.Should().NotBeNull();
        builder.Target.Value.Should().Be(target);
    }

    [Test]
    public void PluginExecutionContextBuilderExtensions_WithPreEntityImage_ShouldAddImage()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithPreEntityImage(image, "MyImage");

        builder.PreEntityImages.Should().ContainKey("MyImage");
        builder.PreEntityImages["MyImage"].Should().Be(image);
    }

    [Test]
    public void PluginExecutionContextBuilderExtensions_WithInputParameter_ShouldAddParameter()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithInputParameter("MyParam", "MyValue");

        builder.InputParameters.Should().ContainKey("MyParam");
        builder.InputParameters["MyParam"].Should().Be("MyValue");
    }

    [Test]
    public void PluginExecutionContextBuilderExtensions_WithMessageName_ShouldSetMessageName()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithMessageName("Update");

        builder.MessageName.Should().Be("Update");
    }
}
