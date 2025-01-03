// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Stub.OrganizationRequests;
using DotNetEnv;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;

namespace Digitall.Stub.Tests.OrganizationRequests;

[TestClass]
public class WhoAmITests
{
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) => Env.Load();


    [TestMethod]
    public void Stubs_Dispatch_Working()
    {
        var sut = new DataverseStub();
        sut.AddStub(new WhoAmIStub());

        var result = sut.Execute(new WhoAmIRequest());

        result.Should().BeAssignableTo<WhoAmIResponse>();
    }
}
