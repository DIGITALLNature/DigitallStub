// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic.Queries.FetchAggregation;

internal abstract class AliasedAggregate : FetchAggregate
{
    protected override object? AggregateValues(IEnumerable<object?> values)
    {
        var lst = values.Where(x => x != null).ToArray();
        var aliasedValue = lst.FirstOrDefault() is AliasedValue;
        if (aliasedValue)
        {
            lst = lst.Select(x => (x as AliasedValue)?.Value).ToArray();
        }

        return AggregateAliasedValues(lst);
    }

    protected abstract object? AggregateAliasedValues(IEnumerable<object?> values);
}
