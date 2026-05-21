// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.


namespace Digitall.Testing.Logic.Queries.FetchAggregation;

internal class CountAggregate : FetchAggregate
{
    protected override object AggregateValues(IEnumerable<object?> values)
    {
        return values.Count();
    }
}
