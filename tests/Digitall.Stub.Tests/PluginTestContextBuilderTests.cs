using System;
using System.Collections.Generic;
using Digitall.APower;
using Digitall.Stub.Tests.Fixtures;
using Digitall.Stub.Tests.Fixtures.SamplePlugin;
using FluentAssertions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NSubstitute;

namespace Digitall.Stub.Tests;

[TestClass]
public class PluginTestContextBuilderTests
{
    [TestMethod]
    public void PluginTestContext_FromMinimalBuilder_Should_HaveEssentials()
    {
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal.Build();

        pluginTestContext.ServiceProvider.Should().NotBeNull();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();
        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().BeEmpty();

        var tracingService = pluginTestContext.ServiceProvider.GetService(typeof(ITracingService)) as ITracingService;
        tracingService.Should().NotBeNull();

        var organizationServiceFactory = pluginTestContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().BeNull();
    }

    [TestMethod]
    public void PluginTestContext_FromDefaultBuilder_Should_HaveCommonServices()
    {
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Default.Build();

        pluginTestContext.ServiceProvider.Should().NotBeNull();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;

        pluginContext.Should().NotBeNull();
        pluginContext.InputParameters.Should().NotBeNull();
        pluginContext.InputParameters.Should().BeEmpty();

        var tracingService = pluginTestContext.ServiceProvider.GetService(typeof(ITracingService)) as ITracingService;
        tracingService.Should().NotBeNull();

        var organizationServiceFactory = pluginTestContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
        organizationServiceFactory.Should().NotBeNull();

        var organizationService = organizationServiceFactory.CreateOrganizationService(null);
        organizationService.Should().NotBeNull();
    }

    [TestMethod]
    public void AddedData_Should_BeRetrieved()
    {
        var entity = new Entity("unittest", Guid.NewGuid());

        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Default
            .AddData(entity)
            .Build();

        var organizationServiceFactory = pluginTestContext.ServiceProvider.GetService(typeof(IOrganizationServiceFactory)) as IOrganizationServiceFactory;
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
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal
            .WithTarget(target)
            .Build();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
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
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal
            .WithTargetReference(target)
            .Build();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
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
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal
            .WithMessageName(messageName)
            .Build();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(typeof(IPluginExecutionContext)) as IPluginExecutionContext;
        pluginContext.Should().NotBeNull();
        pluginContext.MessageName.Should().Be(messageName);
    }

    [TestMethod]
    public void SettingTracingService_Should_OverwriteDefault()
    {
        var tracingService = Substitute.For<ITracingService>();
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal
            .WithTracingService(tracingService)
            .Build();

        var tracingServiceFromContext = pluginTestContext.ServiceProvider.GetService(typeof(ITracingService));
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
        var pluginTestContext = PluginTestContextBuilder<IPlugin>.Minimal.Build();

        pluginTestContext.ServiceProvider.Should().NotBeNull();

        var pluginContextPlain = pluginTestContext.ServiceProvider.GetService(iPluginExecutionContextType) as IPluginExecutionContext;
        pluginContextPlain.Should().NotBeNull();

        var pluginContext = pluginTestContext.ServiceProvider.GetService(iPluginExecutionContextType);
        pluginContext.Should().NotBeNull().And.BeAssignableTo(iPluginExecutionContextType);
    }

    [TestMethod]
    public void TestPlugin_Durchstich()
    {
        var tracingService = Substitute.For<ITracingService>();
        var pluginTestContext = PluginTestContextBuilder<TestPlugin>.Minimal
            .WithTracingService(tracingService)
            .Build();

        pluginTestContext.Should().NotBeNull();
        pluginTestContext.ExecutePlugin();

        tracingService.Received().Trace("TestPlugin: Execute");
    }

    [TestMethod]
    public void TestExecutorPlugin_Durchstich()
    {
        var tracingService = Substitute.For<ITracingService>();
        var pluginTestContext = PluginTestContextBuilder<TestExecutorPlugin>.Minimal
            .WithTracingService(tracingService)
            .Build();

        pluginTestContext.Should().NotBeNull();
        pluginTestContext.ExecuteExecutorPlugin().Should().Be(ExecutionResult.Ok);

        tracingService.Received().Trace("TestPlugin: Execute");
    }

    [TestMethod]
    public void TestExecutorPlugin_WithOutExecutor_ThrowsInvalidCastException()
    {
        var pluginTestContext = PluginTestContextBuilder<TestPlugin>.Minimal
            .Build();

        pluginTestContext.Should().NotBeNull();

        var action = () => pluginTestContext.ExecuteExecutorPlugin();
        action.Should().Throw<InvalidCastException>();
    }
}
