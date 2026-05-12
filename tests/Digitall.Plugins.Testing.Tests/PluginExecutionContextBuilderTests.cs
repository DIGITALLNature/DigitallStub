using System;
using AwesomeAssertions;
using Digitall.Dataverse.Testing.Extensions;
using Digitall.Plugins.Testing.Extensions;
using Digitall.Plugins.Testing.Tests.Fixtures;
using Digitall.Plugins.Testing.Tests.Fixtures.SamplePlugin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Digitall.Plugins.Testing.Tests;

public class PluginExecutionContextBuilderTests
{
    [Test]
    public void PluginTestContext_FromMinimalBuilder_Should_HaveEssentials()
    {
        var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();

        serviceProvider.Should().NotBeNull();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();
        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().BeEmpty();

        var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
        tracingService.Should().NotBeNull();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().BeNull();
    }

    [Test]
    public void PluginTestContext_FromDefaultBuilder_Should_HaveCommonServices()
    {
        var serviceProvider = new FakePluginContextBuilder().BuildServiceProvider();

        serviceProvider.Should().NotBeNull();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

        pluginContext.Should().NotBeNull();
        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().BeEmpty();

        var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
        tracingService.Should().NotBeNull();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().NotBeNull();
    }

    [Test]
    public void AddedData_Should_BeRetrieved()
    {
        var entity = new Entity("unittest", Guid.NewGuid());

        var serviceProvider = new FakePluginContextBuilder().AddData(entity).BuildServiceProvider();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().NotBeNull();

        var retrievedEntity = organizationService.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));
        retrievedEntity.Should().NotBeNull();
        retrievedEntity.Should().BeEquivalentTo(entity);
    }

    [Test]
    public void SettingEntityTarget_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new Entity("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder().WithTarget(target).BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();

        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().ContainKey("Target");
        pluginContext.InputParameters["Target"].Should().Be(target);

        pluginContext.PrimaryEntityId.Should().Be(target.Id);
        pluginContext.PrimaryEntityName.Should().Be(target.LogicalName);
    }

    [Test]
    public void SettingTargetReference_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new EntityReference("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder().WithTarget(target).BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();

        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().ContainKey("Target");
        pluginContext.InputParameters["Target"].Should().Be(target);

        pluginContext.PrimaryEntityId.Should().Be(target.Id);
        pluginContext.PrimaryEntityName.Should().Be(target.LogicalName);
    }

    [Test]
    [Arguments(SdkMessageNames.Create)]
    [Arguments(SdkMessageNames.Update)]
    [Arguments(SdkMessageNames.Associate)]
    [Arguments("custom")]
    public void SettingRequestType_Should_SetPluginMessageName(string messageName)
    {
        var serviceProvider = new PluginExecutionContextBuilder().WithMessageName(messageName).BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();
        pluginContext.MessageName.Should().Be(messageName);
    }

    [Test]
    public void SettingTracingService_Should_OverwriteDefault()
    {
        var tracingService = Substitute.For<ITracingService>();
        var serviceProvider = new PluginExecutionContextBuilder(tracingService).BuildServiceProvider();

        var tracingServiceFromContext = serviceProvider.GetService(typeof(ITracingService));
        tracingServiceFromContext.Should().NotBeNull();
        tracingServiceFromContext.Should().Be(tracingService);
    }

    [Test]
    [Arguments(typeof(IPluginExecutionContext))]
    [Arguments(typeof(IPluginExecutionContext2))]
    [Arguments(typeof(IPluginExecutionContext3))]
    [Arguments(typeof(IPluginExecutionContext4))]
    [Arguments(typeof(IPluginExecutionContext5))]
    [Arguments(typeof(IPluginExecutionContext6))]
    [Arguments(typeof(IPluginExecutionContext7))]
    public void PluginTestContext_FromMinimalBuilder_Should_HaveAllCurrentIPluginExecutionContextFlavors(Type iPluginExecutionContextType)
    {
        var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();

        serviceProvider.Should().NotBeNull();

        var pluginContextPlain = serviceProvider.GetService(iPluginExecutionContextType) as IPluginExecutionContext;
        pluginContextPlain.Should().NotBeNull();

        var pluginContext = serviceProvider.GetService(iPluginExecutionContextType);
        pluginContext.Should().NotBeNull().And.BeAssignableTo(iPluginExecutionContextType);
    }

    [Test]
    public void TestPlugin_Durchstich()
    {
        var tracingService = Substitute.For<ITracingService>();
        var serviceProvider = new PluginExecutionContextBuilder(tracingService).BuildServiceProvider();

        var plugin = new TestPlugin();
        plugin.Execute(serviceProvider);

        tracingService.Received().Trace("TestPlugin: Execute");
    }

    [Test]
    public void SettingModeStageAndIds_Should_SetPluginExecutionContextFields()
    {
        var initiatingUserId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceProvider = new PluginExecutionContextBuilder { Mode = 1, Stage = 40 }.WithInitiatingUserId(initiatingUserId).WithCorrelationId(correlationId).WithMessageName(SdkMessageNames.Update)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

        pluginContext.Should().NotBeNull();
        pluginContext.Mode.Should().Be(1);
        pluginContext.Stage.Should().Be(40);
        pluginContext.InitiatingUserId.Should().Be(initiatingUserId);
        pluginContext.CorrelationId.Should().Be(correlationId);
        pluginContext.MessageName.Should().Be(SdkMessageNames.Update);
    }

    [Test]
    public void WithInputOutputSharedAndImages_Should_ExposeConfiguredCollections()
    {
        var input = new ParameterCollection { ["in"] = 1 };
        var output = new ParameterCollection { ["out"] = 2 };
        var shared = new ParameterCollection { ["shared"] = 3 };
        var preImages = new EntityImageCollection { ["Pre"] = new Entity("account", Guid.NewGuid()) };
        var postImages = new EntityImageCollection { ["Post"] = new Entity("account", Guid.NewGuid()) };

        var serviceProvider = new PluginExecutionContextBuilder().WithInputParameters(input).WithOutputParameters(output).WithSharedVariables(shared).WithPreEntityImages(preImages)
            .WithPostEntityImages(postImages).BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        var pluginContext7 = serviceProvider.GetService(typeof(IPluginExecutionContext7)) as IPluginExecutionContext7;

        pluginContext.Should().NotBeNull();
        pluginContext7.Should().NotBeNull();
        pluginContext.InputParameters.Should().BeSameAs(input);
        pluginContext.OutputParameters.Should().BeSameAs(output);
        pluginContext.SharedVariables.Should().BeSameAs(shared);
        pluginContext.PreEntityImages.Should().BeSameAs(preImages);
        pluginContext.PostEntityImages.Should().BeSameAs(postImages);
        pluginContext7.PreEntityImagesCollection.Should().ContainSingle();
        pluginContext7.PostEntityImagesCollection.Should().ContainSingle();
    }

    [Test]
    public void OrganizationServiceFactory_Should_ReturnConfiguredService_ForAnyUser()
    {
        var organizationService = Substitute.For<IOrganizationService>();
        var serviceProvider = new PluginExecutionContextBuilder(organizationService).BuildServiceProvider();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;

        organizationServiceFactory.Should().NotBeNull();
        organizationServiceFactory.CreateOrganizationService(Guid.NewGuid()).Should().Be(organizationService);
        organizationServiceFactory.CreateOrganizationService(null).Should().Be(organizationService);
    }

    [Test]
    public void LoggerConstructor_Should_RegisterILogger()
    {
        var logger = Substitute.For<ILogger>();
        var serviceProvider = new PluginExecutionContextBuilder(logger).BuildServiceProvider();

        var loggerFromContext = serviceProvider.GetService(typeof(ILogger));

        loggerFromContext.Should().NotBeNull();
        loggerFromContext.Should().Be(logger);
    }

    [Test]
    public void ExistingTargetParameter_Should_ThrowOnBuild_WhenTargetIsAlsoConfigured()
    {
        var action = () => new PluginExecutionContextBuilder().WithInputParameter("Target", new Entity("contact", Guid.NewGuid())).WithTarget(new Entity("account", Guid.NewGuid()))
            .BuildServiceProvider();

        action.Should().Throw<ArgumentException>();
    }

    [Test]
    public void InvalidUserIdEnvVar_Should_DefaultToEmptyGuid()
    {
        var originalUserId = Environment.GetEnvironmentVariable("UserId");

        try
        {
            Environment.SetEnvironmentVariable("UserId", "invalid-guid");

            var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();
            var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

            pluginContext.Should().NotBeNull();
            pluginContext.UserId.Should().Be(Guid.Empty);
        }
        finally
        {
            Environment.SetEnvironmentVariable("UserId", originalUserId);
        }
    }
}
