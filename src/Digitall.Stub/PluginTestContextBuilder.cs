// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Diagnostics;
using Digitall.APower;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Digitall.Stub;

public record PluginTestContextBuilder<TPlugin> where TPlugin : IPlugin
{
    public string MessageName;
    public object Target;
    public ITracingService TracingService;
    public DataverseStub OrganizationService;

    internal PluginTestContextBuilder() { }

    public PluginTestContext Build()
    {
        ParameterCollection inputParameters = [];

        var pluginExecutionContext = Substitute.For<IPluginExecutionContext7>();
        pluginExecutionContext.MessageName.Returns(MessageName);

        if (Target != null)
        {
            inputParameters.Add("Target", Target);

            var targetEntity = Target as Entity;
            var targetReference = Target as EntityReference;

            pluginExecutionContext.PrimaryEntityName.Returns(targetEntity?.LogicalName ?? targetReference?.LogicalName ?? throw new NotSupportedException("target entity name is missing or type is not supported"));
            pluginExecutionContext.PrimaryEntityId.Returns(targetEntity?.Id ?? targetReference?.Id ?? throw new NotSupportedException("target entity id is missing or type is not supported"));
        }

        pluginExecutionContext.InputParameters.Returns(inputParameters);
        pluginExecutionContext.OutputParameters.Returns([]);

        var organizationServiceFactory = Substitute.For<IOrganizationServiceFactory>();
        organizationServiceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(OrganizationService);

        var serviceProvider = Substitute.For<IServiceProvider>();
        serviceProvider.GetService(typeof(IPluginExecutionContext)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext2)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext3)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext4)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext5)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext6)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IPluginExecutionContext7)).Returns(pluginExecutionContext);
        serviceProvider.GetService(typeof(IOrganizationServiceFactory)).Returns(organizationServiceFactory);
        serviceProvider.GetService(typeof(ITracingService)).Returns(TracingService ?? Substitute.For<ITracingService>());

        return new PluginTestContext(serviceProvider);
    }

    public static PluginTestContextBuilder<TPlugin> Minimal => new();

    public static PluginTestContextBuilder<TPlugin> Default
    {
        get
        {
            var tracingService = Substitute.For<ITracingService>();
            tracingService.When(x => x.Trace(Arg.Any<string>())).Do(x => Debug.WriteLine(x.Arg<string>()));

            var organizationService = new DataverseStub();
            organizationService.AddDefaultStubs();

            return Minimal with
            {
                TracingService = tracingService,
                OrganizationService = organizationService,
            };
        }
    }

    public class PluginTestContext(IServiceProvider serviceProvider)
    {
        public readonly IServiceProvider ServiceProvider = serviceProvider;

        public void ExecutePlugin()
        {
            var plugin = Activator.CreateInstance<TPlugin>();
            plugin.Execute(ServiceProvider);
        }

        public ExecutionResult ExecuteExecutorPlugin()
        {
            if (!typeof(Executor).IsAssignableFrom(typeof(TPlugin)))
            {
                throw new InvalidCastException("The plugin must inherit from the Executor base class.");
            }

            var plugin = Activator.CreateInstance<TPlugin>() as Executor;
            plugin!.Execute(ServiceProvider);
            return plugin.Result;
        }
    }
}
