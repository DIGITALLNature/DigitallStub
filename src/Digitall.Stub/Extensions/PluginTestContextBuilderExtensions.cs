// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using Digitall.Stub.OrganizationRequests;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub;

public static class PluginTestContextBuilderExtensions
{
    public static PluginTestContextBuilder<TPlugin> WithTarget<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, Entity entity) where TPlugin : IPlugin
    {
        return builder with { Target = entity };
    }

    public static PluginTestContextBuilder<TPlugin> WithTargetReference<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, EntityReference entityReference) where TPlugin : IPlugin
    {
        return builder with { Target = entityReference };
    }

    public static PluginTestContextBuilder<TPlugin> WithPreImage<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, Entity entity) where TPlugin : IPlugin
    {
        return builder.WithPreImages(new EntityImageCollection{{"PreImage", entity}});
    }

    public static PluginTestContextBuilder<TPlugin> WithPreImages<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, EntityImageCollection entities) where TPlugin : IPlugin
    {
        return builder with { PreEntityImages = entities };
    }

    public static PluginTestContextBuilder<TPlugin> WithPostImage<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, Entity entity) where TPlugin : IPlugin
    {
        return builder.WithPostImages(new EntityImageCollection{{"PostImage", entity}});
    }

    public static PluginTestContextBuilder<TPlugin> WithPostImages<TPlugin>(this PluginTestContextBuilder<TPlugin> builder,EntityImageCollection entities) where TPlugin : IPlugin
    {
        return builder with { PostEntityImages = entities };
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

    public static PluginTestContextBuilder<TPlugin> AddData<TPlugin>(this PluginTestContextBuilder<TPlugin> builder,params Entity[] entity) where TPlugin : IPlugin
    {
        builder.OrganizationService?.AddRange(entity);
        return builder;
    }

    public static PluginTestContextBuilder<TPlugin> AddOrganizationRequest<TPlugin>(this PluginTestContextBuilder<TPlugin> builder, IOrganizationRequestStub organizationRequestStub) where TPlugin : IPlugin
    {
        builder.OrganizationService?.AddStub(organizationRequestStub);
        return builder;
    }
}
