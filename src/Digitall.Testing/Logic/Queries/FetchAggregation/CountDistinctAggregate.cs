// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.


namespace Digitall.Testing.Logic.Queries.FetchAggregation;

class CountDistinctAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object?> values)
    {
        return values.Where(x => x != null).Distinct().Count();
    }
}
