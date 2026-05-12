// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using AwesomeAssertions;
using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk.Query;
using TUnit.Core;

namespace Digitall.Dataverse.Testing.Tests.Extensions;

public class QueryExpressionExtensionsTests
{
    [Test]
    public void CloneQuery_Should_CreateDeepCopy()
    {
        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet("name") };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "Test");

        var cloned = qe.CloneQuery();

        cloned.Should().NotBeSameAs(qe);
        cloned.EntityName.Should().Be(qe.EntityName);
        cloned.ColumnSet.Columns.Should().Contain(qe.ColumnSet.Columns);
        cloned.Criteria.Conditions.Should().HaveCount(qe.Criteria.Conditions.Count);
        cloned.Criteria.Conditions[0].AttributeName.Should().Be(qe.Criteria.Conditions[0].AttributeName);
    }
}
