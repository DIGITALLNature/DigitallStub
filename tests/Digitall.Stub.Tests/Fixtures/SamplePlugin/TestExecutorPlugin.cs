// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.APower;

namespace Digitall.Stub.Tests.Fixtures.SamplePlugin;

public class TestExecutorPlugin : Executor
{
    protected override ExecutionResult Execute()     {
        Trace("TestPlugin: Execute");
        return ExecutionResult.Ok;
    }
}
