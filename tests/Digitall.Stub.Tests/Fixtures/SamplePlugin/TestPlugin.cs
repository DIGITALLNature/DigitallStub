// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Tests.Fixtures.SamplePlugin;

public class TestPlugin : IPlugin
{
    public void Execute(IServiceProvider serviceProvider)
    {
        var tracingService = serviceProvider.GetService(typeof(ITracingService)) as ITracingService;
        tracingService.Trace("TestPlugin: Execute");
    }
}
