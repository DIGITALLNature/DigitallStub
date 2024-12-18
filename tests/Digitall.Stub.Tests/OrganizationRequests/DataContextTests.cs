// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
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

    [TestMethod]
    public void ProjectionOfEarlyBound_Should_MaintainType()
    {
        var stub = new DataverseStub();
        stub.AddDefaultStubs();

        var accountId = Guid.NewGuid();
        var accountName = "Test Account";
        var account = new Account(accountId)
        {
            Name = accountName,
        };
        stub.Add(account);

        using (var dataContext = new DataContext(stub))
        {
            dataContext.AccountSet.Select(a => a.Id).Single().Should().Be(accountId);
        }

        using (var dataContext = new DataContext(stub))
        {
            dataContext.AccountSet.Select(a => a.Name).Single().Should().Be(accountName);
        }

        account.Id.Should().Be(accountId);
        account.Name.Should().Be(accountName);

        using (var dataContext = new DataContext(stub))
        {
            dataContext.AccountSet.Select(a => a.Id).Single().Should().Be(accountId);
            dataContext.AccountSet.Select(a => a.Name).Single().Should().Be(accountName);
        }
    }
}
