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

    [TestMethod]
    public void Top()
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
    public void Paging()
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
    public void EmptyPageOnPaging()
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
    public void Destinct()
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
    public void Empty()
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
    public void TotalRecords()
    {
        var sut = new DataverseStub();

        var manyRecords = new List<Account>();
        Enumerable.Range(0, 200).ToList().ForEach(x => manyRecords.Add(new Account(Guid.NewGuid()){Name = $"Account {x}"}));

        sut.AddRange(manyRecords);

        var result = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { TopCount = 5, ColumnSet = new ColumnSet(true) });
        result.Should().NotBeNull();
        result.Entities.Should().HaveCount(5);
    }
}
