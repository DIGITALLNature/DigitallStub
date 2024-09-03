// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using System.Linq;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class CountDistinctAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object> values)
    {
        return values.Where(x => x != null).Distinct().Count();
    }
}
