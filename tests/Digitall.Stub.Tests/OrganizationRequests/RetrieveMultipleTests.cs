// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Digitall.Stub.Tests.Fixtures;
using DotNetEnv;
using FluentAssertions;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Tests.OrganizationRequests;

[TestClass]
public class RetrieveMultipleTests
{
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext)
    {
        Environment.SetEnvironmentVariable("MaxRetrieveCount", "10");
        Env.Load();
    }

    [TestMethod]
    public void Stubs_Dispatch_Working()
    {
        var sut = new DataverseStub();

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName));

        result.Should().NotBeNull();
    }

    #region QueryExpression

    [TestMethod]
    public void QueryExpression_Top()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }

    [TestMethod]
    public void QueryExpression_Paging()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 19).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(10);
        result.PagingCookie.Should().NotBeNull();
        result.MoreRecords.Should().BeTrue();

        result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
        {
            PagingCookie = result.PagingCookie,
            PageNumber = 2
        }

        });

        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(9);
        result.PagingCookie.Should().BeNull();
        result.MoreRecords.Should().BeFalse();
    }

    [TestMethod]
    public void QueryExpression_EmptyPageOnPaging()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(10);
        result.PagingCookie.Should().NotBeNull();
        result.MoreRecords.Should().BeTrue();

        result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
            {
                PagingCookie = result.PagingCookie,
                PageNumber = 21 // out of range
            }

        });

        result.Should().NotBeNull();
        result.Entities.Should().BeEmpty();
        result.PagingCookie.Should().BeNull();
        result.MoreRecords.Should().BeFalse();

    }

    [TestMethod]
    public void QueryExpression_Destinct()
    {
        var sut = new DataverseStub();
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

        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(2);

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

        resultDestinct.Should().NotBeNull();
        resultDestinct.Entities.Should().HaveCount(1);
    }

    [TestMethod]
    public void QueryExpression_Empty()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }


    [TestMethod]
    public void QueryExpression_TotalRecords()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }

    [TestMethod]
    public void QueryExpression_Order()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 50).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}", ExchangeRate = x}));

        sut.AddRange(manyRecords);

        var resultDescending = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Descending) } });
        resultDescending.Should().NotBeNull();
        resultDescending.Entities.Select(a => a.ToEntity<Account>()).Should().BeInDescendingOrder(x => x.ExchangeRate);

        var resultAscending = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Ascending) } });
        resultAscending.Should().NotBeNull();
        resultAscending.Entities.Select(a => a.ToEntity<Account>()).Should().BeInAscendingOrder(x => x.ExchangeRate);
    }

    #endregion

    #region QueryByAttribute
   [TestMethod]
    public void QueryByAttribute_Top()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }

    [TestMethod]
    public void QueryByAttribute_Paging()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 19).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(10);
        result.PagingCookie.Should().NotBeNull();
        result.MoreRecords.Should().BeTrue();

        result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
        {
            PagingCookie = result.PagingCookie,
            PageNumber = 2
        }

        });

        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(9);
        result.PagingCookie.Should().BeNull();
        result.MoreRecords.Should().BeFalse();
    }

    [TestMethod]
    public void QueryByAttribute_EmptyPageOnPaging()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(10);
        result.PagingCookie.Should().NotBeNull();
        result.MoreRecords.Should().BeTrue();

        result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),PageInfo = new PagingInfo
            {
                PagingCookie = result.PagingCookie,
                PageNumber = 21 // out of range
            }

        });

        result.Should().NotBeNull();
        result.Entities.Should().BeEmpty();
        result.PagingCookie.Should().BeNull();
        result.MoreRecords.Should().BeFalse();

    }

    [TestMethod]
    public void QueryByAttribute_Empty()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }


    [TestMethod]
    public void QueryByAttribute_TotalRecords()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }

    [TestMethod]
    public void QueryByAttribute_Order()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 50).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}", ExchangeRate = x}));

        sut.AddRange(manyRecords);

        var resultDescending = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Descending) } });
        resultDescending.Should().NotBeNull();
        resultDescending.Entities.Select(a => a.ToEntity<Account>()).Should().BeInDescendingOrder(x => x.ExchangeRate);

        var resultAscending = sut.RetrieveMultiple(new QueryByAttribute(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true),
            Orders = { new OrderExpression(Account.LogicalNames.ExchangeRate, OrderType.Ascending) } });
        resultAscending.Should().NotBeNull();
        resultAscending.Entities.Select(a => a.ToEntity<Account>()).Should().BeInAscendingOrder(x => x.ExchangeRate);
    }
    #endregion
}
