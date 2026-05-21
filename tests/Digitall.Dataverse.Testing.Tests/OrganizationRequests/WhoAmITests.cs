// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using DotNetEnv;
using Microsoft.Crm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class WhoAmITests
{
    [Before(Class)]
    public static async Task MyClassInitialize()
    {
        Env.Load();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Stubs_Dispatch_Working()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new WhoAmIFake());

        var result = sut.Execute(new WhoAmIRequest());

        await Assert.That(result).IsTypeOf<WhoAmIResponse>();
    }
}
