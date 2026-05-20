using System;
using System.Threading.Tasks;
using Digitall.Testing.Extensions;
using Digitall.Testing.Tests.Fixtures;
using Digitall.Testing.Tests.Fixtures.SamplePlugin;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests;

public class PluginExecutionContextBuilderTests
{
    [Test]
    public async Task PluginTestContext_FromMinimalBuilder_Should_HaveEssentials()
    {
        var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();

        await Assert.That(serviceProvider).IsNotNull();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(pluginContext!.InputParameters).IsNotNull();
        await Assert.That(pluginContext.InputParameters).IsEmpty();

        var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
        await Assert.That(tracingService).IsNotNull();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        await Assert.That(organizationServiceFactory).IsNotNull();

        var organizationService = organizationServiceFactory!.CreateOrganizationService(null);
        await Assert.That(organizationService).IsNull();
    }

    [Test]
    public async Task PluginTestContext_FromDefaultBuilder_Should_HaveCommonServices()
    {
        var serviceProvider = new FakePluginContextBuilder().BuildServiceProvider();

        await Assert.That(serviceProvider).IsNotNull();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(pluginContext!.InputParameters).IsNotNull();
        await Assert.That(pluginContext.InputParameters).IsEmpty();

        var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
        await Assert.That(tracingService).IsNotNull();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        await Assert.That(organizationServiceFactory).IsNotNull();

        var organizationService = organizationServiceFactory!.CreateOrganizationService(null);
        await Assert.That(organizationService).IsNotNull();
    }

    [Test]
    public async Task AddedData_Should_BeRetrieved()
    {
        var entity = new Entity("unittest", Guid.NewGuid());

        var serviceProvider = new FakePluginContextBuilder()
            .AddData(entity)
            .BuildServiceProvider();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        await Assert.That(organizationServiceFactory).IsNotNull();

        var organizationService = organizationServiceFactory!.CreateOrganizationService(null);
        await Assert.That(organizationService).IsNotNull();

        var retrievedEntity = organizationService!.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));
        await Assert.That(retrievedEntity).IsNotNull();
        await Assert.That(retrievedEntity).IsEquivalentTo(entity);
    }

    [Test]
    public async Task SettingEntityTarget_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new Entity("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithTarget(target)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        await Assert.That(pluginContext).IsNotNull();

        await Assert.That(pluginContext!.InputParameters).IsNotNull();
        await Assert.That(pluginContext.InputParameters.ContainsKey("Target")).IsTrue();
        await Assert.That(pluginContext.InputParameters["Target"]).IsEqualTo(target);

        await Assert.That(pluginContext.PrimaryEntityId).IsEqualTo(target.Id);
        await Assert.That(pluginContext.PrimaryEntityName).IsEqualTo(target.LogicalName);
    }

    [Test]
    public async Task SettingTargetReference_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new EntityReference("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithTarget(target)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        await Assert.That(pluginContext).IsNotNull();

        await Assert.That(pluginContext!.InputParameters).IsNotNull();
        await Assert.That(pluginContext.InputParameters.ContainsKey("Target")).IsTrue();
        await Assert.That(pluginContext.InputParameters["Target"]).IsEqualTo(target);

        await Assert.That(pluginContext.PrimaryEntityId).IsEqualTo(target.Id);
        await Assert.That(pluginContext.PrimaryEntityName).IsEqualTo(target.LogicalName);
    }

    [Test]
    [Arguments(SdkMessageNames.Create)]
    [Arguments(SdkMessageNames.Update)]
    [Arguments(SdkMessageNames.Associate)]
    [Arguments("custom")]
    public async Task SettingRequestType_Should_SetPluginMessageName(string messageName)
    {
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithMessageName(messageName)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(pluginContext!.MessageName).IsEqualTo(messageName);
    }

    [Test]
    public async Task SettingTracingService_Should_OverwriteDefault()
    {
        var tracingService = Mock.Of<ITracingService>().Object;
        var serviceProvider = new PluginExecutionContextBuilder(tracingService)
            .BuildServiceProvider();

        var tracingServiceFromContext = serviceProvider.GetService(typeof(ITracingService));
        await Assert.That(tracingServiceFromContext).IsNotNull();
        await Assert.That(tracingServiceFromContext).IsEqualTo(tracingService);
    }

    [Test]
    [Arguments(typeof(IPluginExecutionContext))]
    [Arguments(typeof(IPluginExecutionContext2))]
    [Arguments(typeof(IPluginExecutionContext3))]
    [Arguments(typeof(IPluginExecutionContext4))]
    [Arguments(typeof(IPluginExecutionContext5))]
    [Arguments(typeof(IPluginExecutionContext6))]
    [Arguments(typeof(IPluginExecutionContext7))]
    public async Task PluginTestContext_FromMinimalBuilder_Should_HaveAllCurrentIPluginExecutionContextFlavors(Type iPluginExecutionContextType)
    {
        var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();

        await Assert.That(serviceProvider).IsNotNull();

        var pluginContextPlain = serviceProvider.GetService(iPluginExecutionContextType) as IPluginExecutionContext;
        await Assert.That(pluginContextPlain).IsNotNull();

        var pluginContext = serviceProvider.GetService(iPluginExecutionContextType);
        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(iPluginExecutionContextType.IsAssignableFrom(pluginContext!.GetType())).IsTrue();
    }

    [Test]
    public async Task TestPlugin_Durchstich()
    {
        var tracingServiceMock = Mock.Of<ITracingService>();
        var serviceProvider = new PluginExecutionContextBuilder(tracingServiceMock.Object)
            .BuildServiceProvider();

        var plugin = new TestPlugin();
        plugin.Execute(serviceProvider);

        tracingServiceMock.Trace("TestPlugin: Execute", Arg.Any<object[]>()).WasCalled();
    }

    [Test]
    public async Task SettingModeStageAndIds_Should_SetPluginExecutionContextFields()
    {
        var initiatingUserId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var serviceProvider = new PluginExecutionContextBuilder
        {
            Mode = 1,
            Stage = 40
        }
        .WithInitiatingUserId(initiatingUserId)
        .WithCorrelationId(correlationId)
        .WithMessageName(SdkMessageNames.Update)
        .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(pluginContext!.Mode).IsEqualTo(1);
        await Assert.That(pluginContext.Stage).IsEqualTo(40);
        await Assert.That(pluginContext.InitiatingUserId).IsEqualTo(initiatingUserId);
        await Assert.That(pluginContext.CorrelationId).IsEqualTo(correlationId);
        await Assert.That(pluginContext.MessageName).IsEqualTo(SdkMessageNames.Update);
    }

    [Test]
    public async Task WithInputOutputSharedAndImages_Should_ExposeConfiguredCollections()
    {
        var input = new ParameterCollection { ["in"] = 1 };
        var output = new ParameterCollection { ["out"] = 2 };
        var shared = new ParameterCollection { ["shared"] = 3 };
        var preImages = new EntityImageCollection { ["Pre"] = new Entity("account", Guid.NewGuid()) };
        var postImages = new EntityImageCollection { ["Post"] = new Entity("account", Guid.NewGuid()) };

        var serviceProvider = new PluginExecutionContextBuilder()
            .WithInputParameters(input)
            .WithOutputParameters(output)
            .WithSharedVariables(shared)
            .WithPreEntityImages(preImages)
            .WithPostEntityImages(postImages)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        var pluginContext7 = serviceProvider.GetService(typeof(IPluginExecutionContext7)) as IPluginExecutionContext7;

        await Assert.That(pluginContext).IsNotNull();
        await Assert.That(pluginContext7).IsNotNull();
        await Assert.That(pluginContext!.InputParameters).IsSameReferenceAs(input);
        await Assert.That(pluginContext.OutputParameters).IsSameReferenceAs(output);
        await Assert.That(pluginContext.SharedVariables).IsSameReferenceAs(shared);
        await Assert.That(pluginContext.PreEntityImages).IsSameReferenceAs(preImages);
        await Assert.That(pluginContext.PostEntityImages).IsSameReferenceAs(postImages);
        await Assert.That(pluginContext7!.PreEntityImagesCollection).HasCount().EqualTo(1);
        await Assert.That(pluginContext7.PostEntityImagesCollection).HasCount().EqualTo(1);
    }

    [Test]
    public async Task OrganizationServiceFactory_Should_ReturnConfiguredService_ForAnyUser()
    {
        var organizationService = Mock.Of<IOrganizationService>().Object;
        var serviceProvider = new PluginExecutionContextBuilder(organizationService).BuildServiceProvider();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;

        await Assert.That(organizationServiceFactory).IsNotNull();
        await Assert.That(organizationServiceFactory!.CreateOrganizationService(Guid.NewGuid())).IsEqualTo(organizationService);
        await Assert.That(organizationServiceFactory.CreateOrganizationService(null)).IsEqualTo(organizationService);
    }

    [Test]
    public async Task LoggerConstructor_Should_RegisterILogger()
    {
        var logger = Mock.Of<ILogger>().Object;
        var serviceProvider = new PluginExecutionContextBuilder(logger).BuildServiceProvider();

        var loggerFromContext = serviceProvider.GetService(typeof(ILogger));

        await Assert.That(loggerFromContext).IsNotNull();
        await Assert.That(loggerFromContext).IsEqualTo(logger);
    }

    [Test]
    public async Task ExistingTargetParameter_Should_ThrowOnBuild_WhenTargetIsAlsoConfigured()
    {
        Action action = () => new PluginExecutionContextBuilder()
            .WithInputParameter("Target", new Entity("contact", Guid.NewGuid()))
            .WithTarget(new Entity("account", Guid.NewGuid()))
            .BuildServiceProvider();

        Assert.Throws<ArgumentException>(action);
    }

    [Test]
    public async Task InvalidUserIdEnvVar_Should_DefaultToEmptyGuid()
    {
        var originalUserId = Environment.GetEnvironmentVariable("UserId");

        try
        {
            Environment.SetEnvironmentVariable("UserId", "invalid-guid");

            var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();
            var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

            await Assert.That(pluginContext).IsNotNull();
            await Assert.That(pluginContext!.UserId).IsEqualTo(Guid.Empty);
        }
        finally
        {
            Environment.SetEnvironmentVariable("UserId", originalUserId);
        }
    }
}
