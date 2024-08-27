// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.APower;

namespace Digitall.Stub;

public static class PluginTestContextExtensions
{
    public static ExecutionResult Execute<TPlugin>(this PluginTestContextBuilder<TPlugin>.PluginTestContext context) where TPlugin : Executor
    {
        var plugin = System.Activator.CreateInstance<TPlugin>();
        plugin.Execute(context.ServiceProvider);
        return plugin.Result;
    }
}
