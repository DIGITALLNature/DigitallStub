// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using AwesomeAssertions;
using Digitall.Testing.OrganizationRequests;
using DotNetEnv;
using Microsoft.Crm.Sdk.Messages;

namespace Digitall.Testing.Tests.OrganizationRequests;

[TestClass]
public class WhoAmITests
{
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext) => Env.Load();


    [TestMethod]
    public void Stubs_Dispatch_Working()
    {
        var sut = new FakedDataverse();
        sut.AddRequest(new WhoAmIFake());

        var result = sut.Execute(new WhoAmIRequest());

        result.Should().BeAssignableTo<WhoAmIResponse>();
    }
}
