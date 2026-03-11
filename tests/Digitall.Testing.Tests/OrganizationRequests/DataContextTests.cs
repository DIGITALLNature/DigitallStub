// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using AwesomeAssertions;
using Digitall.Testing.Tests.Fixtures;

namespace Digitall.Testing.Tests.OrganizationRequests;

[TestClass]
public class DataContextTests
{
    [TestMethod]
    public void EmptyAccountSet_Should_Return_EmptyList()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Should().BeEmpty();
        }
    }

    [TestMethod]
    public void FilledAccountSet_Should_NotBeEmpty()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        dataverse.Add(new Account(Guid.NewGuid()));

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Should().NotBeEmpty();
        }
    }

    [TestMethod]
    public void ProjectionOfEarlyBound_Should_MaintainType()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        var accountId = Guid.NewGuid();
        var accountName = "Test Account";
        var account = new Account(accountId)
        {
            Name = accountName,
        };
        dataverse.Add(account);

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select(a => a.Id).Single().Should().Be(accountId);
        }

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select(a => a.Name).Single().Should().Be(accountName);
        }

        account.Id.Should().Be(accountId);
        account.Name.Should().Be(accountName);

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select(a => a.Name).Single().Should().Be(accountName);
        }
    }
}
