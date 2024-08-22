using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Digitall.Stub.Plugin
{
    public class PluginTestContext<TPlugin>(IServiceProvider serviceProvider)
        where TPlugin : IPlugin
    {
        public IServiceProvider ServiceProvider = serviceProvider;

        public void ExecutePlugin()
        {
            var plugin = Activator.CreateInstance<TPlugin>();
            plugin.Execute(ServiceProvider);
        }
    }

    public record PluginTestContextBuilder<TPlugin>
        where TPlugin : IPlugin
    {
        public string MessageName;
        public object Target;
        public ITracingService TracingService;
        public DataverseStub OrganizationService;

        internal PluginTestContextBuilder() { }

        public PluginTestContext<TPlugin> Build()
        {
            ParameterCollection inputParameters = [];

            var pluginExecutionContext = Substitute.For<IPluginExecutionContext>();
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

            var organizationServiceFactory = Substitute.For<IOrganizationServiceFactory>();
            organizationServiceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(OrganizationService);

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IPluginExecutionContext)).Returns(pluginExecutionContext);
            serviceProvider.GetService(typeof(IOrganizationServiceFactory)).Returns(organizationServiceFactory);
            serviceProvider.GetService(typeof(ITracingService)).Returns(TracingService ?? Substitute.For<ITracingService>());

            return new PluginTestContext<TPlugin>(serviceProvider);
        }

        public static PluginTestContextBuilder<TPlugin> Minimal => new();

        public static PluginTestContextBuilder<TPlugin> Default
        {
            get
            {
                var tracingService = Substitute.For<ITracingService>();
                tracingService.When(x => x.Trace(Arg.Any<string>())).Do(x => Debug.WriteLine(x.Arg<string>()));

                var organizationService = new DataverseStub();

                return Minimal with
                {
                    TracingService = tracingService,
                    OrganizationService = organizationService,
                };
            }
        }
    }

    public static class PluginTestContextExtensions
    {
        public static PluginTestContextBuilder<TPlugin> WithTarget<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, Entity entity) where TPlugin : IPlugin
        {
            return builder with { Target = entity };
        }

        public static PluginTestContextBuilder<TPlugin> WithTargetReference<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, EntityReference entityReference) where TPlugin : IPlugin
        {
            return builder with { Target = entityReference };
        }

        public static PluginTestContextBuilder<TPlugin> WithTracingService<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, ITracingService tracingService) where TPlugin : IPlugin
        {
            return builder with { TracingService = tracingService };
        }

        public static PluginTestContextBuilder<TPlugin> WithMessageName<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, string messageName) where TPlugin : IPlugin
        {
            return builder with { MessageName = messageName };
        }

        public static PluginTestContextBuilder<TPlugin> AddData<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, Entity entity) where TPlugin : IPlugin
        {
            builder.OrganizationService?.Add(entity);
            return builder;
        }

        public static PluginTestContextBuilder<TPlugin> AddData<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, IEnumerable<Entity> entities) where TPlugin : IPlugin
        {
            builder.OrganizationService?.AddRange(entities);
            return builder;
        }
    }
}