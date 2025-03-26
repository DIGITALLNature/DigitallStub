// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;
using NSubstitute;

namespace Digitall.Stub;

public class PluginExecutionContextBuilder
{
    public string MessageName { get; set; }
    public int Mode { get; set; }
    public int Stage { get; set; }
    public Target? Target { get; set; }
    public ParameterCollection InputParameters { get; set; }
    public ParameterCollection OutputParameters { get; set; }
    public EntityImageCollection PreEntityImages { get; set; } = [];
    public EntityImageCollection PostEntityImages { get; set; } = [];
    public ParameterCollection SharedVariables { get; set; }
    public Guid InitiatingUserId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; set; } = Guid.NewGuid();
    public IOrganizationService OrganizationService { get; set; }
    public ITracingService TracingService { get; set; } = Substitute.For<ITracingService>();
    public ILogger Logger { get; set; } = Substitute.For<ILogger>();

    public IServiceProvider BuildServiceProvider()
    {
        var pluginExecutionContext = Substitute.For<IPluginExecutionContext7>();

        pluginExecutionContext.MessageName.Returns(MessageName);
        pluginExecutionContext.Mode.Returns(Mode);
        pluginExecutionContext.Stage.Returns(Stage);
        pluginExecutionContext.InitiatingUserId.Returns(InitiatingUserId);
        pluginExecutionContext.CorrelationId.Returns(CorrelationId);

        // parameters
        pluginExecutionContext.InputParameters.Returns(InputParameters);
        pluginExecutionContext.OutputParameters.Returns(OutputParameters);

        // pre / post images
        pluginExecutionContext.PreEntityImages.Returns(PreEntityImages);
        pluginExecutionContext.PreEntityImagesCollection.Returns([PreEntityImages]);
        pluginExecutionContext.PostEntityImages.Returns(PreEntityImages);
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
        organizationServiceFactory.CreateOrganizationService(Arg.Any<Guid?>()).Returns(OrganizationService); // TODO how to add data?

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
}

public static class PluginExecutionContextBuilderExtensions
{
    public static TPluginExecutionContextBuilder WithTarget<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Entity target)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.Target = new Target(target);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithTarget<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, EntityReference target)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.Target = new Target(target);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithPreEntityImage<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Entity preImage, string name = "PreImage")
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.PreEntityImages.Add(name, preImage);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithPreEntityImages<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, EntityImageCollection preImages)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.PreEntityImages = preImages;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithPostEntityImage<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Entity postImage, string name = "PostImage")
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.PostEntityImages.Add(name, postImage);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithPostEntityImages<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, EntityImageCollection postImages)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.PostEntityImages = postImages;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithInputParameter<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, string key, object value)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.InputParameters.Add(key, value);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithInputParameters<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, ParameterCollection parameters)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.InputParameters = parameters;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithOutputParameter<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, string key, object value)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.OutputParameters.Add(key, value);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithOutputParameters<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, ParameterCollection parameters)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.OutputParameters = parameters;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithSharedVariable<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, string key, object value)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.SharedVariables.Add(key, value);
        return builder;
    }

    public static TPluginExecutionContextBuilder WithSharedVariables<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, ParameterCollection parameters)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.SharedVariables = parameters;
        return builder;
    }

    public static PluginExecutionContextBuilder WithInitiatingUserId<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Guid userId)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.InitiatingUserId = userId;
        return builder;
    }

    public static PluginExecutionContextBuilder WithCorrelationId<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Guid correlationId)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.CorrelationId = correlationId;
        return builder;
    }
}

public record Target
{
    public Target(Entity target)
    {
        Id = target.Id;
        LogicalName = target.LogicalName;
        Value = target;
    }

    public Target(EntityReference target)
    {
        Id = target.Id;
        LogicalName = target.LogicalName;
        Value = target;
    }

    public Guid Id { get; }
    public string LogicalName { get; }
    public object Value { get; }
}

public class DataverseStubBuilder : PluginExecutionContextBuilder
{
    public new DataverseStub OrganizationService { get; set; }
}

public static class DataverseStubBuilderExtensions
{
    public static DataverseStubBuilder WithData(this DataverseStubBuilder builder, params Entity[] records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    // TODO add more extension methods e.g. to add stubs
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

        return new RegistrationInfo { MessageName = "TODO", Mode = PluginExecutionMode.Async, Stage = PluginExecutionStage.Post };
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

public class TestClass
{
    public void TestMethod()
    {
        // Arrange
        var serviceProvider = new PluginExecutionContextBuilder()
            .WithTarget(new Entity("account"))
            .BuildServiceProvider();

        var serviceProviderWithStub = new DataverseStubBuilder()
            .WithTarget(new Entity("account"))
            .WithData(new Entity("account"), new Entity("contact"))
            .BuildServiceProvider();

        IPlugin plugin = null;

        // Act
        plugin.Execute(serviceProviderWithStub);

        // Assert
        // use our extension methods to get target etc.
    }
}
