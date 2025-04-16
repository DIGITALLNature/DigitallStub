// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Digitall.Stub.Model;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Extensions;

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

    public static TPluginExecutionContextBuilder WithInitiatingUserId<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Guid userId)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.InitiatingUserId = userId;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithCorrelationId<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, Guid correlationId)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.CorrelationId = correlationId;
        return builder;
    }

    public static TPluginExecutionContextBuilder WithMessageName<TPluginExecutionContextBuilder>(this TPluginExecutionContextBuilder builder, string messageName)
        where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        builder.MessageName = messageName;
        return builder;
    }
}
