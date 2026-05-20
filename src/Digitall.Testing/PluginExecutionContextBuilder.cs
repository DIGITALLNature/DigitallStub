// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Digitall.Testing.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace Digitall.Testing;

public class PluginExecutionContextBuilder
{
    public PluginExecutionContextBuilder()
    {
    }

    public PluginExecutionContextBuilder(IOrganizationService organizationService)
    {
        OrganizationService = organizationService;
    }

    public PluginExecutionContextBuilder(ITracingService tracingService)
    {
        TracingService = tracingService;
    }

    public PluginExecutionContextBuilder(ILogger logger)
    {
        Logger = logger;
    }

    public PluginExecutionContextBuilder(ITracingService tracingService, ILogger logger)
    {
        TracingService = tracingService;
        Logger = logger;
    }

    public PluginExecutionContextBuilder(IOrganizationService organizationService, ITracingService tracingService, ILogger logger)
    {
        OrganizationService = organizationService;
        TracingService = tracingService;
        Logger = logger;
    }

    public string? MessageName { get; set; }
    public int Mode { get; set; }
    public int Stage { get; set; }
    public Target? Target { get; set; }
    public ParameterCollection InputParameters { get; set; } = [];
    public ParameterCollection OutputParameters { get; set; } = [];
    public EntityImageCollection PreEntityImages { get; set; } = [];
    public EntityImageCollection PostEntityImages { get; set; } = [];
    public ParameterCollection SharedVariables { get; set; } = [];
    public Guid InitiatingUserId { get; set; }

    public Guid UserId
    {
        get
        {
            Guid.TryParse(Environment.GetEnvironmentVariable("UserId"), out var userId);
            return userId;
        }
        set { Environment.SetEnvironmentVariable("UserId", value.ToString()); }
    }

    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; } = Guid.NewGuid();
    public int Depth { get; set; } = 1;

    public IOrganizationService? OrganizationService { get; set; }
    public ITracingService TracingService { get; set; } = Mock.Of<ITracingService>().Object;
    public ILogger Logger { get; set; } = Mock.Of<ILogger>().Object;

    public IServiceProvider BuildServiceProvider()
    {
        var pluginExecutionContext = Mock.Of<IPluginExecutionContext7>();

        pluginExecutionContext.MessageName.Returns(MessageName!);
        pluginExecutionContext.Mode.Returns(Mode);
        pluginExecutionContext.Stage.Returns(Stage);
        pluginExecutionContext.InitiatingUserId.Returns(InitiatingUserId);
        pluginExecutionContext.UserId.Returns(UserId);
        pluginExecutionContext.CorrelationId.Returns(CorrelationId);
        pluginExecutionContext.Depth.Returns(Depth);
        pluginExecutionContext.TenantId.Returns(TenantId);

        // parameters
        pluginExecutionContext.InputParameters.Returns(InputParameters);
        pluginExecutionContext.OutputParameters.Returns(OutputParameters);

        // pre / post images
        pluginExecutionContext.PreEntityImages.Returns(PreEntityImages);
        pluginExecutionContext.PreEntityImagesCollection.Returns([PreEntityImages]);
        pluginExecutionContext.PostEntityImages.Returns(PostEntityImages);
        pluginExecutionContext.PostEntityImagesCollection.Returns([PostEntityImages]);

        // shared variables
        pluginExecutionContext.SharedVariables.Returns(SharedVariables);

        // target
        if (Target != null)
        {
            pluginExecutionContext.PrimaryEntityId.Returns(Target.Id);
            pluginExecutionContext.PrimaryEntityName.Returns(Target.LogicalName);
            InputParameters.Add("Target", Target.Value);
        }

        // organization service
        var organizationServiceFactory = Mock.Of<IOrganizationServiceFactory>();
        organizationServiceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(OrganizationService!);

        // service provider
        var serviceProvider = Mock.Of<IServiceProvider>();

        serviceProvider.GetService(typeof(IPluginExecutionContext)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext2)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext3)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext4)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext5)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext6)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IPluginExecutionContext7)).Returns(pluginExecutionContext.Object);
        serviceProvider.GetService(typeof(IOrganizationServiceFactory)).Returns(organizationServiceFactory.Object);
        serviceProvider.GetService(typeof(ITracingService)).Returns(TracingService);
        serviceProvider.GetService(typeof(ILogger)).Returns(Logger);

        return serviceProvider.Object;
    }
}
