// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Threading.Tasks;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests.Extensions;

public class QueryExpressionExtensionsTests
{
    [Test]
    public async Task CloneQuery_Should_CreateDeepCopy()
    {
        var qe = new QueryExpression("account");
        qe.ColumnSet = new ColumnSet("name");
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "Test");

        var cloned = qe.CloneQuery();

        await Assert.That(cloned).IsNotSameReferenceAs(qe);
        await Assert.That(cloned.EntityName).IsEqualTo(qe.EntityName);
        await Assert.That(cloned.ColumnSet.Columns.Contains(qe.ColumnSet.Columns[0])).IsTrue();
        await Assert.That(cloned.Criteria.Conditions.Count).IsEqualTo(qe.Criteria.Conditions.Count);
        await Assert.That(cloned.Criteria.Conditions[0].AttributeName).IsEqualTo(qe.Criteria.Conditions[0].AttributeName);
    }
}
