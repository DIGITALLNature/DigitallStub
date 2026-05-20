// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Threading.Tasks;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Tests.Extensions;

public class BuilderExtensionsTests
{
    [Test]
    public async Task PluginExecutionContextBuilderExtensions_WithTarget_ShouldSetTarget()
    {
        var target = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithTarget(target);

        await Assert.That(builder.Target).IsNotNull();
        await Assert.That(builder.Target.Value).IsEqualTo(target);
    }

    [Test]
    public async Task PluginExecutionContextBuilderExtensions_WithPreEntityImage_ShouldAddImage()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        builder.WithPreEntityImage(image, "MyImage");

        await Assert.That(builder.PreEntityImages.ContainsKey("MyImage")).IsTrue();
        await Assert.That(builder.PreEntityImages["MyImage"]).IsEqualTo(image);
    }

    [Test]
    public async Task PluginExecutionContextBuilderExtensions_WithInputParameter_ShouldAddParameter()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithInputParameter("MyParam", "MyValue");

        await Assert.That(builder.InputParameters.ContainsKey("MyParam")).IsTrue();
        await Assert.That(builder.InputParameters["MyParam"]).IsEqualTo("MyValue");
    }

    [Test]
    public async Task PluginExecutionContextBuilderExtensions_WithMessageName_ShouldSetMessageName()
    {
        var builder = new PluginExecutionContextBuilder();

        builder.WithMessageName("Update");

        await Assert.That(builder.MessageName).IsEqualTo("Update");
    }

    [Test]
    public async Task FakeDataverseBuilderExtensions_AddData_ShouldAddRecords()
    {
        var entity = new Entity("account", Guid.NewGuid());
        var builder = new FakeDataverseBuilder();

        builder.AddData(entity);

        var service = builder.GetOrganizationService();
        await Assert.That(service.ServiceState["account"].ContainsKey(entity.Id)).IsTrue();
    }

    [Test]
    public async Task FakeDataverseBuilderExtensions_AddConfig_ShouldAddEnvironmentVariables()
    {
        var builder = new FakeDataverseBuilder();

        builder.AddConfig("my_key", "default_val", "override_val");

        var service = builder.GetOrganizationService();
        await Assert.That(service.ServiceState.ContainsKey("environmentvariabledefinition")).IsTrue();
        await Assert.That(service.ServiceState.ContainsKey("environmentvariablevalue")).IsTrue();
    }
}
