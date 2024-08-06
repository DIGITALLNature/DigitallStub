// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using DotNetEnv;
using Microsoft.Xrm.Sdk;
using NSubstitute;

namespace Digitall.Stub;

/// <summary>
    /// Represents the context for a plugin execution test.
    /// </summary>
    public sealed class PluginExecutionTestContext<TEntity> where TEntity: Entity, new()
    {
        private readonly IPluginExecutionContext5 _plugincontext;

        /// <summary>
        /// Gets or sets the target entity.
        /// </summary>
        public TEntity Target { get; } = new TEntity();

        /// <summary>
        /// Gets or sets the target entity reference.
        /// </summary>
        public EntityReference TargetReference { get; } = new EntityReference("unittest", Guid.NewGuid());

        /// <summary>
        /// Gets the input parameters collection.
        /// </summary>
        public ParameterCollection InputParameters { get; } = new ParameterCollection();

        /// <summary>
        /// Gets or sets the pre-image entity.
        /// </summary>
        public TEntity PreImage { get; } = new TEntity();

        /// <summary>
        /// Gets or sets the post-image entity.
        /// </summary>
        public TEntity Postimage { get; } = new TEntity();


        /// <summary>
        /// Initializes a new instance of the <see cref="PluginExecutionTestContext"/> class.
        /// </summary>
        /// <param name="targetType">The target type (default is Entity).</param>
        public PluginExecutionTestContext(TargetType targetType = TargetType.Entity)
        {
            Env.Load();

            _plugincontext = Substitute.For<IPluginExecutionContext5>();

            switch (targetType)
            {
                case TargetType.Entity:
                    InputParameters.Add("Target", Target);
                    break;
                case TargetType.Reference:
                    InputParameters.Add("Target", TargetReference);
                    break;
            }

            _plugincontext.InputParameters.Returns(InputParameters);
            _plugincontext.PreEntityImages.Returns(new EntityImageCollection { { "PreImage", PreImage } });
            _plugincontext.PostEntityImages.Returns(new EntityImageCollection { { "PostImage", Postimage } });
            _plugincontext.InitiatingUserId.Returns(Guid.Parse(DotNetEnv.Env.GetString("CallerId", Guid.Empty.ToString())));
            _plugincontext.CorrelationId.Returns(Guid.NewGuid());

        }

        // Current last version
        public IPluginExecutionContext5 Context => _plugincontext;

        public IPluginExecutionContext5 Context5 => _plugincontext;
        public IPluginExecutionContext4 Context4 => _plugincontext;
        public IPluginExecutionContext3 Context3 => _plugincontext;
        public IPluginExecutionContext2 Context2 => _plugincontext;
        public IPluginExecutionContext Context1 => _plugincontext;
    }
