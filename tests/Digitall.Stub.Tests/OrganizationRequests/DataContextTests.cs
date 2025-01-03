// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Digitall.Stub.Tests.Fixtures;
using FluentAssertions;

namespace Digitall.Stub.Tests.OrganizationRequests;

[TestClass]
public class DataContextTests
{
    [TestMethod]
    public void EmptyAccountSet_Should_Return_EmptyList()
    {
        var stub = new DataverseStub();
        stub.AddDefaultStubs();

        using (var dataContext = new DataContext(stub))
        {
            dataContext.AccountSet.Should().BeEmpty();
        }
    }

    [TestMethod]
    public void FilledAccountSet_Should_NotBeEmpty()
    {
        var stub = new DataverseStub();
        stub.AddDefaultStubs();

        stub.Add(new Account(Guid.NewGuid()));

        using (var dataContext = new DataContext(stub))
        {
            dataContext.AccountSet.Should().NotBeEmpty();
        }
    }
}