// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using DotNetEnv;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class RetrieveMultipleTests
{
    [Before(Class)]
    public static async Task MyClassInitialize()
    {
        Environment.SetEnvironmentVariable("MaxRetrieveCount", "10");
        Env.Load();
        await Task.CompletedTask;
    }

    [Test]
    public async Task Stubs_Dispatch_Working()
    {
        var sut = new FakeOrganizationService();

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName));

        await Assert.That(result).IsNotNull();
    }

    #region QueryExpression

    [Test]
    public async Task QueryExpression_Top()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }

    [Test]
    public async Task QueryExpression_Paging()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 19).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(10);
        await Assert.That(result.PagingCookie).IsNotNull();
        await Assert.That(result.MoreRecords).IsTrue();

        result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
        {
            PagingCookie = result.PagingCookie,
            PageNumber = 2
        }

        });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(9);
        await Assert.That(result.PagingCookie).IsNull();
        await Assert.That(result.MoreRecords).IsFalse();
    }

    [Test]
    public async Task QueryExpression_EmptyPageOnPaging()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(10);
        await Assert.That(result.PagingCookie).IsNotNull();
        await Assert.That(result.MoreRecords).IsTrue();

        result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
            {
                PagingCookie = result.PagingCookie,
                PageNumber = 21 // out of range
            }

        });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).IsEmpty();
        await Assert.That(result.PagingCookie).IsNull();
        await Assert.That(result.MoreRecords).IsFalse();

    }

    [Test]
    public async Task QueryExpression_Destinct()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, "B Corp")
                }
            },
            LinkEntities = { new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName, Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner) }}
        );

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(2);

        var resultDestinct = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            Criteria = new FilterExpression
            {
                Conditions =
                {
                    new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, "B Corp")
                }
            },
            LinkEntities = { new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName, Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner) },
            Distinct = true
            }
        );

        await Assert.That(resultDestinct).IsNotNull();
        await Assert.That(resultDestinct.Entities).Count().IsEqualTo(1);
    }

    [Test]
    public async Task QueryExpression_Empty()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }


    [Test]
    public async Task QueryExpression_TotalRecords()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }

    [Test]
    public async Task QueryExpression_Order()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 50).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}", ExchangeRate = x}));

        sut.AddRange(manyRecords);

        var resultDescending = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Descending) } });
        await Assert.That(resultDescending).IsNotNull();
        var descItems = resultDescending.Entities.Select(a => a.ToEntity<Account>()).ToList();
        await Assert.That(descItems.SequenceEqual(descItems.OrderByDescending(x => x.ExchangeRate))).IsTrue();

        var resultAscending = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Ascending) } });
        await Assert.That(resultAscending).IsNotNull();
        var ascItems = resultAscending.Entities.Select(a => a.ToEntity<Account>()).ToList();
        await Assert.That(ascItems.SequenceEqual(ascItems.OrderBy(x => x.ExchangeRate))).IsTrue();
    }

    #endregion

    #region QueryByAttribute
   [Test]
    public async Task QueryByAttribute_Top()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }

    [Test]
    public async Task QueryByAttribute_Paging()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 19).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(10);
        await Assert.That(result.PagingCookie).IsNotNull();
        await Assert.That(result.MoreRecords).IsTrue();

        result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
        {
            PagingCookie = result.PagingCookie,
            PageNumber = 2
        }

        });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(9);
        await Assert.That(result.PagingCookie).IsNull();
        await Assert.That(result.MoreRecords).IsFalse();
    }

    [Test]
    public async Task QueryByAttribute_EmptyPageOnPaging()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(10);
        await Assert.That(result.PagingCookie).IsNotNull();
        await Assert.That(result.MoreRecords).IsTrue();

        result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
            {
                PagingCookie = result.PagingCookie,
                PageNumber = 21 // out of range
            }

        });

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).IsEmpty();
        await Assert.That(result.PagingCookie).IsNull();
        await Assert.That(result.MoreRecords).IsFalse();

    }

    [Test]
    public async Task QueryByAttribute_Empty()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }


    [Test]
    public async Task QueryByAttribute_TotalRecords()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Entities).Count().IsEqualTo(5);
    }

    [Test]
    public async Task QueryByAttribute_Order()
    {
        var sut = new FakeOrganizationService();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 50).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}", ExchangeRate = x}));

        sut.AddRange(manyRecords);

        var resultDescending = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Descending) } });
        await Assert.That(resultDescending).IsNotNull();
        var descItems = resultDescending.Entities.Select(a => a.ToEntity<Account>()).ToList();
        await Assert.That(descItems.SequenceEqual(descItems.OrderByDescending(x => x.ExchangeRate))).IsTrue();

        var resultAscending = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Ascending) } });
        await Assert.That(resultAscending).IsNotNull();
        var ascItems = resultAscending.Entities.Select(a => a.ToEntity<Account>()).ToList();
        await Assert.That(ascItems.SequenceEqual(ascItems.OrderBy(x => x.ExchangeRate))).IsTrue();
    }
    #endregion
}
