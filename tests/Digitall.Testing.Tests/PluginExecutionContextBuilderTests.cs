using System;
using AwesomeAssertions;
using Digitall.Testing.Extensions;
using Digitall.Testing.Tests.Fixtures;
using Digitall.Testing.Tests.Fixtures.SamplePlugin;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Digitall.Testing.Tests;

[TestClass]
public class PluginExecutionContextBuilderTests
{
    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void AddedData_Should_BeRetrieved()
    {
        var entity = new Entity("unittest", Guid.NewGuid());

        var serviceProvider = new FakePluginContextBuilder()
            .AddData(entity)
            .BuildServiceProvider();

        var organizationServiceFactory = serviceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().NotBeNull();

        var retrievedEntity = organizationService.Retrieve(entity.LogicalName, entity.Id, new ColumnSet(true));
        retrievedEntity.Should().NotBeNull();
        retrievedEntity.Should().BeEquivalentTo(entity);
    }

    [TestMethod]
    public void SettingEntityTarget_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new Entity("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithTarget(target)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();

        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().ContainKey("Target");
        pluginContext.InputParameters["Target"].Should().Be(target);

        pluginContext.PrimaryEntityId.Should().Be(target.Id);
        pluginContext.PrimaryEntityName.Should().Be(target.LogicalName);
    }

    [TestMethod]
    public void SettingTargetReference_Should_SetInputParameter_And_PluginPrimaryEntity()
    {
        var target = new EntityReference("unittest", Guid.NewGuid());
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithTarget(target)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();

        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().ContainKey("Target");
        pluginContext.InputParameters["Target"].Should().Be(target);

        pluginContext.PrimaryEntityId.Should().Be(target.Id);
        pluginContext.PrimaryEntityName.Should().Be(target.LogicalName);
    }

    [TestMethod]
    [DataRow(SdkMessageNames.Create)]
    [DataRow(SdkMessageNames.Update)]
    [DataRow(SdkMessageNames.Associate)]
    [DataRow("custom")]
    public void SettingRequestType_Should_SetPluginMessageName(string messageName)
    {
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithMessageName(messageName)
            .BuildServiceProvider();

        var pluginContext = serviceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();
        pluginContext.MessageName.Should().Be(messageName);
    }

    [TestMethod]
    public void SettingTracingService_Should_OverwriteDefault()
    {
        var tracingService = Substitute.For<ITracingService>();
        var serviceProvider = new PluginExecutionContextBuilder(tracingService)
            .BuildServiceProvider();

        var tracingServiceFromContext = serviceProvider.GetService(typeof(ITracingService));
        tracingServiceFromContext.Should().NotBeNull();
        tracingServiceFromContext.Should().Be(tracingService);
    }

    [TestMethod]
    [DataRow(typeof(IPluginExecutionContext))]
    [DataRow(typeof(IPluginExecutionContext2))]
    [DataRow(typeof(IPluginExecutionContext3))]
    [DataRow(typeof(IPluginExecutionContext4))]
    [DataRow(typeof(IPluginExecutionContext5))]
    [DataRow(typeof(IPluginExecutionContext6))]
    [DataRow(typeof(IPluginExecutionContext7))]
    public void PluginTestContext_FromMinimalBuilder_Should_HaveAllCurrentIPluginExecutionContextFlavors(Type iPluginExecutionContextType)
    {
        var serviceProvider = new PluginExecutionContextBuilder().BuildServiceProvider();

        serviceProvider.Should().NotBeNull();

        var pluginContextPlain = serviceProvider.GetService(iPluginExecutionContextType) as IPluginExecutionContext;
        pluginContextPlain.Should().NotBeNull();

        var pluginContext = serviceProvider.GetService(iPluginExecutionContextType);
        pluginContext.Should().NotBeNull().And.BeAssignableTo(iPluginExecutionContextType);
    }

    [TestMethod]
    public void TestPlugin_Durchstich()
    {
        var tracingService = Substitute.For<ITracingService>();
        var serviceProvider = new PluginExecutionContextBuilder(tracingService)
            .BuildServiceProvider();

        var plugin = new TestPlugin();
        plugin.Execute(serviceProvider);

        tracingService.Received().Trace("TestPlugin: Execute");
    }

    [TestMethod]
    public void GetFakedDataverse_Should_Return_FakedDataverse()
    {
        var service = new FakePluginContextBuilder().GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
    }

    [TestMethod]
    public void FakedDataverseBuilder_With_Custom_TimeProvider()
    {
        var service = new FakePluginContextBuilder(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
        service.TimeProvider.GetUtcNow().Year.Should().Be(2000);
    }

    [TestMethod]
    public void FakedDataverseBuilder_With_Custom_FakeDataverse()
    {
        var fakeDataverse = new FakeOrganizationService(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        var service = new FakePluginContextBuilder(fakeDataverse).GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
        service.TimeProvider.GetUtcNow().Year.Should().Be(2000);
    }
}
