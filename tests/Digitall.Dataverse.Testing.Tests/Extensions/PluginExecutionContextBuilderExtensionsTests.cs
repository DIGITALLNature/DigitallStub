// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class PluginExecutionContextBuilderExtensionsTests
{
    [Test]
    public async Task WithTarget_Entity_ShouldSetTargetValueAndReturnSameBuilder()
    {
        var target = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithTarget(target);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.Target).IsNotNull();
        await Assert.That(builder.Target!.Value).IsEqualTo(target);
    }

    [Test]
    public async Task WithTarget_Entity_ShouldSetIdAndLogicalName()
    {
        var target = new Entity("contact", Guid.NewGuid());

        var builder = new PluginExecutionContextBuilder().WithTarget(target);

        await Assert.That(builder.Target!.Id).IsEqualTo(target.Id);
        await Assert.That(builder.Target.LogicalName).IsEqualTo(target.LogicalName);
    }

    [Test]
    public async Task WithTarget_EntityReference_ShouldSetTargetValueAndReturnSameBuilder()
    {
        var target = new EntityReference("contact", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithTarget(target);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.Target).IsNotNull();
        await Assert.That(builder.Target!.Value).IsEqualTo(target);
    }

    [Test]
    public async Task WithTarget_EntityReference_ShouldSetIdAndLogicalName()
    {
        var target = new EntityReference("lead", Guid.NewGuid());

        var builder = new PluginExecutionContextBuilder().WithTarget(target);

        await Assert.That(builder.Target!.Id).IsEqualTo(target.Id);
        await Assert.That(builder.Target.LogicalName).IsEqualTo(target.LogicalName);
    }

    [Test]
    public async Task WithPreEntityImage_ShouldAddImageWithDefaultName_PreImage()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPreEntityImage(image);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PreEntityImages.ContainsKey("PreImage")).IsTrue();
        await Assert.That(builder.PreEntityImages["PreImage"]).IsEqualTo(image);
    }

    [Test]
    public async Task WithPreEntityImage_ShouldAddImageWithCustomName()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPreEntityImage(image, "CustomPre");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PreEntityImages.ContainsKey("CustomPre")).IsTrue();
        await Assert.That(builder.PreEntityImages["CustomPre"]).IsEqualTo(image);
    }

    [Test]
    public async Task WithPreEntityImages_ShouldReplaceCollectionAndReturnSameBuilder()
    {
        var images = new EntityImageCollection { ["Pre1"] = new Entity("account", Guid.NewGuid()) };
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPreEntityImages(images);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PreEntityImages).IsSameReferenceAs(images);
    }

    [Test]
    public async Task WithPostEntityImage_ShouldAddImageWithDefaultName_PostImage()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPostEntityImage(image);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PostEntityImages.ContainsKey("PostImage")).IsTrue();
        await Assert.That(builder.PostEntityImages["PostImage"]).IsEqualTo(image);
    }

    [Test]
    public async Task WithPostEntityImage_ShouldAddImageWithCustomName()
    {
        var image = new Entity("account", Guid.NewGuid());
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPostEntityImage(image, "CustomPost");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PostEntityImages.ContainsKey("CustomPost")).IsTrue();
        await Assert.That(builder.PostEntityImages["CustomPost"]).IsEqualTo(image);
    }

    [Test]
    public async Task WithPostEntityImages_ShouldReplaceCollectionAndReturnSameBuilder()
    {
        var images = new EntityImageCollection { ["Post1"] = new Entity("account", Guid.NewGuid()) };
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithPostEntityImages(images);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.PostEntityImages).IsSameReferenceAs(images);
    }

    [Test]
    public async Task WithInputParameter_ShouldAddParameterAndReturnSameBuilder()
    {
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithInputParameter("myKey", "myValue");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.InputParameters.ContainsKey("myKey")).IsTrue();
        await Assert.That(builder.InputParameters["myKey"]).IsEqualTo("myValue");
    }

    [Test]
    public async Task WithInputParameters_ShouldReplaceCollectionAndReturnSameBuilder()
    {
        var parameters = new ParameterCollection { ["param1"] = 42 };
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithInputParameters(parameters);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.InputParameters).IsSameReferenceAs(parameters);
    }

    [Test]
    public async Task WithOutputParameter_ShouldAddParameterAndReturnSameBuilder()
    {
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithOutputParameter("outKey", "outValue");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.OutputParameters.ContainsKey("outKey")).IsTrue();
        await Assert.That(builder.OutputParameters["outKey"]).IsEqualTo("outValue");
    }

    [Test]
    public async Task WithOutputParameters_ShouldReplaceCollectionAndReturnSameBuilder()
    {
        var parameters = new ParameterCollection { ["out1"] = "result" };
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithOutputParameters(parameters);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.OutputParameters).IsSameReferenceAs(parameters);
    }

    [Test]
    public async Task WithSharedVariable_ShouldAddVariableAndReturnSameBuilder()
    {
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithSharedVariable("sharedKey", "sharedValue");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.SharedVariables.ContainsKey("sharedKey")).IsTrue();
        await Assert.That(builder.SharedVariables["sharedKey"]).IsEqualTo("sharedValue");
    }

    [Test]
    public async Task WithSharedVariables_ShouldReplaceCollectionAndReturnSameBuilder()
    {
        var parameters = new ParameterCollection { ["shared1"] = true };
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithSharedVariables(parameters);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.SharedVariables).IsSameReferenceAs(parameters);
    }

    [Test]
    public async Task WithInitiatingUserId_ShouldSetUserIdAndReturnSameBuilder()
    {
        var userId = Guid.NewGuid();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithInitiatingUserId(userId);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.InitiatingUserId).IsEqualTo(userId);
    }

    [Test]
    public async Task WithUserId_ShouldSetUserIdAndReturnSameBuilder()
    {
        var userId = Guid.NewGuid();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithUserId(userId);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.UserId).IsEqualTo(userId);
    }

    [Test]
    public async Task WithCorrelationId_ShouldSetCorrelationIdAndReturnSameBuilder()
    {
        var correlationId = Guid.NewGuid();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithCorrelationId(correlationId);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.CorrelationId).IsEqualTo(correlationId);
    }

    [Test]
    public async Task WithMessageName_ShouldSetMessageNameAndReturnSameBuilder()
    {
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithMessageName("Create");

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.MessageName).IsEqualTo("Create");
    }

    [Test]
    public async Task WithTenantId_ShouldSetTenantIdAndReturnSameBuilder()
    {
        var tenantId = Guid.NewGuid();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithTenantId(tenantId);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.TenantId).IsEqualTo(tenantId);
    }

    [Test]
    public async Task WithDepth_ShouldSetDepthAndReturnSameBuilder()
    {
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithDepth(3);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.Depth).IsEqualTo(3);
    }

    [Test]
    public async Task WithTracingService_ShouldSetTracingServiceAndReturnSameBuilder()
    {
        var tracingService = Mock.Of<ITracingService>();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithTracingService((ITracingService)tracingService);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.TracingService).IsEqualTo((ITracingService)tracingService);
    }

    [Test]
    public async Task WithOrganizationService_ShouldSetOrganizationServiceAndReturnSameBuilder()
    {
        var orgService = Mock.Of<IOrganizationService>();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithOrganizationService((IOrganizationService)orgService);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.OrganizationService).IsEqualTo((IOrganizationService)orgService);
    }

    [Test]
    public async Task WithLogger_ShouldSetLoggerAndReturnSameBuilder()
    {
        var logger = Mock.Of<ILogger>();
        var builder = new PluginExecutionContextBuilder();

        var result = builder.WithLogger((ILogger)logger);

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.Logger).IsEqualTo((ILogger)logger);
    }

    [Test]
    public async Task Extensions_ShouldPreserveDerivedType_WhenUsedOnFakePluginContextBuilder()
    {
        var target = new Entity("account", Guid.NewGuid());
        var fakeBuilder = new FakePluginContextBuilder();

        var result = fakeBuilder.WithTarget(target);

        await Assert.That(result).IsTypeOf<FakePluginContextBuilder>();
        await Assert.That(result).IsSameReferenceAs(fakeBuilder);
    }

    [Test]
    public async Task FluentChaining_ShouldApplyAllSettings()
    {
        var entityTarget = new Entity("account", Guid.NewGuid());
        var preImage = new Entity("account", Guid.NewGuid());
        var postImage = new Entity("account", Guid.NewGuid());
        var userId = Guid.NewGuid();
        var initiatingUserId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var tracingService = Mock.Of<ITracingService>();
        var orgService = Mock.Of<IOrganizationService>();
        var logger = Mock.Of<ILogger>();

        var builder = new PluginExecutionContextBuilder()
            .WithTarget(entityTarget)
            .WithPreEntityImage(preImage)
            .WithPostEntityImage(postImage)
            .WithInputParameter("in", 1)
            .WithOutputParameter("out", 2)
            .WithSharedVariable("shared", 3)
            .WithUserId(userId)
            .WithInitiatingUserId(initiatingUserId)
            .WithCorrelationId(correlationId)
            .WithMessageName("Create")
            .WithTenantId(tenantId)
            .WithDepth(2)
            .WithTracingService((ITracingService)tracingService)
            .WithOrganizationService((IOrganizationService)orgService)
            .WithLogger((ILogger)logger);

        await Assert.That(builder.Target).IsNotNull();
        await Assert.That(builder.PreEntityImages.ContainsKey("PreImage")).IsTrue();
        await Assert.That(builder.PostEntityImages.ContainsKey("PostImage")).IsTrue();
        await Assert.That(builder.InputParameters.ContainsKey("in")).IsTrue();
        await Assert.That(builder.OutputParameters.ContainsKey("out")).IsTrue();
        await Assert.That(builder.SharedVariables.ContainsKey("shared")).IsTrue();
        await Assert.That(builder.UserId).IsEqualTo(userId);
        await Assert.That(builder.InitiatingUserId).IsEqualTo(initiatingUserId);
        await Assert.That(builder.CorrelationId).IsEqualTo(correlationId);
        await Assert.That(builder.MessageName).IsEqualTo("Create");
        await Assert.That(builder.TenantId).IsEqualTo(tenantId);
        await Assert.That(builder.Depth).IsEqualTo(2);
        await Assert.That(builder.TracingService).IsEqualTo((ITracingService)tracingService);
        await Assert.That(builder.OrganizationService).IsEqualTo((IOrganizationService)orgService);
        await Assert.That(builder.Logger).IsEqualTo((ILogger)logger);
    }
}
