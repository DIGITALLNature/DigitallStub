// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace Digitall.Dataverse.Testing;

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

    // ReSharper disable once UnusedMember.Global : Public API
    public PluginExecutionContextBuilder(ITracingService tracingService, ILogger logger)
    {
        TracingService = tracingService;
        Logger = logger;
    }

    // ReSharper disable once UnusedMember.Global : Public API
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

    private Guid? _userId;

    public Guid UserId
    {
        get
        {
            if (_userId.HasValue)
                return _userId.Value;

            if (OrganizationService is FakeOrganizationService fakeService)
                return fakeService.Options.UserId;

            return Guid.Empty;
        }
        // ReSharper disable once UnusedMember.Global : Public API
        set => _userId = value;
    }

    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; } = Guid.NewGuid();
    public int Depth { get; set; } = 1;

    public IOrganizationService? OrganizationService { get; set; }
    public ITracingService TracingService { get; set; } = ITracingService.Mock();
    public ILogger Logger { get; set; } = ILogger.Mock();

    /// <summary>
    /// Override this method to add additional service mocks to the <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    protected virtual void ConfigureServices(IServiceProviderMock serviceProvider)
    {
    }

    public IServiceProvider BuildServiceProvider()
    {
        var pluginExecutionContext = IPluginExecutionContext7.Mock();

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
        var organizationServiceFactory = IOrganizationServiceFactory.Mock();
        organizationServiceFactory.CreateOrganizationService(Any<Guid?>()).Returns(OrganizationService!);

        // service provider
        var serviceProvider = IServiceProvider.Mock();

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

        ConfigureServices(serviceProvider);

        return serviceProvider;
    }
}
