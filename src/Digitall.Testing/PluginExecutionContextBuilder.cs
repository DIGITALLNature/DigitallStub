// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using Digitall.Testing.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using NSubstitute;

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

    public IOrganizationService? OrganizationService { get; set; }
    public ITracingService TracingService { get; set; } = Substitute.For<ITracingService>();
    public ILogger Logger { get; set; } = Substitute.For<ILogger>();

    public IServiceProvider BuildServiceProvider()
    {
        var pluginExecutionContext = Substitute.For<IPluginExecutionContext7>();

        pluginExecutionContext.MessageName.Returns(MessageName);
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
            pluginExecutionContext.InputParameters.Add("Target", Target.Value);
        }

        // organization service
        var organizationServiceFactory = Substitute.For<IOrganizationServiceFactory>();
        organizationServiceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(OrganizationService);

        // service provider
        var serviceProvider = Substitute.For<IServiceProvider>();

        serviceProvider.GetService(typeof(IPluginExecutionContext)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext2)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext3)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext4)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext5)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext6)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext7)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IOrganizationServiceFactory)).Returns(organizationServiceFactory);
        serviceProvider.GetService(typeof(ITracingService)).Returns(TracingService);
        serviceProvider.GetService(typeof(ILogger)).Returns(Logger);

        return serviceProvider;
    }

    public Guid TenantId { get; set; } = Guid.NewGuid();

    public int Depth { get; set; } = 1;
}

// TODO move this class to separate package (dependency to AssemblyPower)
public class PluginExecutionContextBuilder<TPlugin, TRequest> : PluginExecutionContextBuilder where TPlugin : IPlugin where TRequest : OrganizationRequest, new()
{
    public PluginExecutionContextBuilder()
    {
        var registration = GetRegistrationInfo();

        MessageName = registration.MessageName;
        Mode = (int)registration.Mode;
        Stage = (int)registration.Stage;
    }

    private static RegistrationInfo GetRegistrationInfo()
    {
        var request = new TRequest();
        var messageName = request.RequestName;

        var pluginRegistration = Attribute.GetCustomAttributes(typeof(TPlugin), typeof(PluginRegistrationAttribute)).Cast<PluginRegistrationAttribute>();

        // TODO filter

        return new RegistrationInfo { MessageName = messageName, Mode = PluginExecutionMode.Async, Stage = PluginExecutionStage.Post };
    }

    private record RegistrationInfo
    {
        public string MessageName { get; set; }
        public PluginExecutionMode Mode { get; set; }
        public PluginExecutionStage Stage { get; set; }
    }

    private enum PluginExecutionMode
    {
        // TODO use enum from AssemblyPower
        Sync,
        Async
    }

    private enum PluginExecutionStage
    {
        // TODO use enum from AssemblyPower
        Pre,
        Post
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    private class PluginRegistrationAttribute : Attribute
    {
        // TODO use class from AssemblyPower
    }
}
