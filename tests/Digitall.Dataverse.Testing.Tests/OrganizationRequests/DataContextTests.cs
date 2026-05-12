// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using AwesomeAssertions;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using TUnit.Core;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class DataContextTests
{
    [Test]
    public void EmptyAccountSet_Should_Return_EmptyList()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        using var dataContext = new DataContext(dataverse);
        dataContext.AccountSet.AsEnumerable().Should().BeEmpty();
    }

    [Test]
    public void FilledAccountSet_Should_NotBeEmpty()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        dataverse.Add(new Account(Guid.NewGuid()));

        using var dataContext = new DataContext(dataverse);
        dataContext.AccountSet.AsEnumerable().Should().NotBeEmpty();
    }

    [Test]
    public void ProjectionOfEarlyBound_Should_MaintainType()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        var accountId = Guid.NewGuid();
        const string accountName = "Test Account";
        var account = new Account(accountId) { Name = accountName, };
        dataverse.Add(account);

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select(a => a.Id).Single().Should().Be(accountId);
        }

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select<Account, string>(a => a.Name).Single().Should().Be(accountName);
        }

        account.Id.Should().Be(accountId);
        account.Name.Should().Be(accountName);

        using (var dataContext = new DataContext(dataverse))
        {
            dataContext.AccountSet.Select<Account, string>(a => a.Name).Single().Should().Be(accountName);
        }
    }
}
