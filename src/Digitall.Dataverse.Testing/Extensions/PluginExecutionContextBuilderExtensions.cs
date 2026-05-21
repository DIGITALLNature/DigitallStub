// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Model;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.PluginTelemetry;

namespace Digitall.Dataverse.Testing.Extensions;

public static class PluginExecutionContextBuilderExtensions
{
    extension<TPluginExecutionContextBuilder>(TPluginExecutionContextBuilder builder) where TPluginExecutionContextBuilder : PluginExecutionContextBuilder
    {
        public TPluginExecutionContextBuilder WithTarget(Entity target)
        {
            builder.Target = new Target(target);
            return builder;
        }

        public TPluginExecutionContextBuilder WithTarget(EntityReference target)
        {
            builder.Target = new Target(target);
            return builder;
        }

        public TPluginExecutionContextBuilder WithPreEntityImage(Entity preImage, string name = "PreImage")
        {
            builder.PreEntityImages.Add(name, preImage);
            return builder;
        }

        public TPluginExecutionContextBuilder WithPreEntityImages(EntityImageCollection preImages)
        {
            builder.PreEntityImages = preImages;
            return builder;
        }

        public TPluginExecutionContextBuilder WithPostEntityImage(Entity postImage, string name = "PostImage")
        {
            builder.PostEntityImages.Add(name, postImage);
            return builder;
        }

        public TPluginExecutionContextBuilder WithPostEntityImages(EntityImageCollection postImages)
        {
            builder.PostEntityImages = postImages;
            return builder;
        }

        public TPluginExecutionContextBuilder WithInputParameter(string key, object value)
        {
            builder.InputParameters.Add(key, value);
            return builder;
        }

        public TPluginExecutionContextBuilder WithInputParameters(ParameterCollection parameters)
        {
            builder.InputParameters = parameters;
            return builder;
        }

        public TPluginExecutionContextBuilder WithOutputParameter(string key, object value)
        {
            builder.OutputParameters.Add(key, value);
            return builder;
        }

        public TPluginExecutionContextBuilder WithOutputParameters(ParameterCollection parameters)
        {
            builder.OutputParameters = parameters;
            return builder;
        }

        public TPluginExecutionContextBuilder WithSharedVariable(string key, object value)
        {
            builder.SharedVariables.Add(key, value);
            return builder;
        }

        public TPluginExecutionContextBuilder WithSharedVariables(ParameterCollection parameters)
        {
            builder.SharedVariables = parameters;
            return builder;
        }

        public TPluginExecutionContextBuilder WithInitiatingUserId(Guid userId)
        {
            builder.InitiatingUserId = userId;
            return builder;
        }

        public TPluginExecutionContextBuilder WithUserId(Guid userId)
        {
            builder.UserId = userId;
            return builder;
        }

        public TPluginExecutionContextBuilder WithCorrelationId(Guid correlationId)
        {
            builder.CorrelationId = correlationId;
            return builder;
        }

        public TPluginExecutionContextBuilder WithMessageName(string messageName)
        {
            builder.MessageName = messageName;
            return builder;
        }

        public TPluginExecutionContextBuilder WithTenantId(Guid tenantId)
        {
            builder.TenantId = tenantId;
            return builder;
        }

        public TPluginExecutionContextBuilder WithDepth(int depth)
        {
            builder.Depth = depth;
            return builder;
        }

        public TPluginExecutionContextBuilder WithTracingService(ITracingService tracingService)
        {
            builder.TracingService = tracingService;
            return builder;
        }

        public TPluginExecutionContextBuilder WithOrganizationService(IOrganizationService organizationService)
        {
            builder.OrganizationService = organizationService;
            return builder;
        }

        public TPluginExecutionContextBuilder WithLogger(ILogger logger)
        {
            builder.Logger = logger;
            return builder;
        }
    }
}
