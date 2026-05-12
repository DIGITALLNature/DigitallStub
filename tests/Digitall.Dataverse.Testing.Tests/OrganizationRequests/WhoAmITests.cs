// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using AwesomeAssertions;
using Digitall.Dataverse.Testing.OrganizationRequests;
using DotNetEnv;
using Microsoft.Crm.Sdk.Messages;
using TUnit.Core;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class WhoAmITests
{
    [Before(HookType.Class)]
    public static void MyClassInitialize() => Env.Load();


    [Test]
    public void Stubs_Dispatch_Working()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new WhoAmIFake());

        var result = sut.Execute(new WhoAmIRequest());

        result.Should().BeAssignableTo<WhoAmIResponse>();
    }
}
