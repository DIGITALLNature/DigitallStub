// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.


namespace Digitall.Testing.Logic.Queries.FetchAggregation;

internal class CountColumnAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object?> values)
    {
        return values.Count(x => x != null);
    }
}
