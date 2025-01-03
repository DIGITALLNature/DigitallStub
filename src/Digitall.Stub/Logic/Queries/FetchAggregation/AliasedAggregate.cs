// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

abstract class AliasedAggregate : FetchAggregate
{
    protected override object AggregateValues(IEnumerable<object> values)
    {
        var lst = values.Where(x => x != null);
        bool alisedValue = lst.FirstOrDefault() is AliasedValue;
        if (alisedValue)
        {
            lst = lst.Select(x => (x as AliasedValue)?.Value);
        }

        return AggregateAliasedValues(lst);
    }

    protected abstract object AggregateAliasedValues(IEnumerable<object> values);
}
