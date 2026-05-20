// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Linq;
using Digitall.Testing.Tests.Fixtures;

namespace Digitall.Testing.Tests.OrganizationRequests;

public class DataContextTests
{
    [Test]
    public async Task EmptyAccountSet_Should_Return_EmptyList()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        using var dataContext = new DataContext(dataverse);
        await Assert.That(dataContext.AccountSet).IsEmpty();
    }

    [Test]
    public async Task FilledAccountSet_Should_NotBeEmpty()
    {
        var dataverse = new FakeOrganizationService();
        dataverse.AddDefaultRequests();

        dataverse.Add(new Account(Guid.NewGuid()));

        using var dataContext = new DataContext(dataverse);
        await Assert.That(dataContext.AccountSet).IsNotEmpty();
    }

    [Test]
    public async Task ProjectionOfEarlyBound_Should_MaintainType()
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
            await Assert.That(dataContext.AccountSet.Select(a => a.Id).Single()).IsEqualTo(accountId);
        }

        using (var dataContext = new DataContext(dataverse))
        {
            await Assert.That(dataContext.AccountSet.Select(a => a.Name).Single()).IsEqualTo(accountName);
        }

        await Assert.That(account.Id).IsEqualTo(accountId);
        await Assert.That(account.Name).IsEqualTo(accountName);

        using (var dataContext = new DataContext(dataverse))
        {
            await Assert.That(dataContext.AccountSet.Select(a => a.Name).Single()).IsEqualTo(accountName);
        }
    }
}
