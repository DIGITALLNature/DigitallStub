// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using System.Linq;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class CountAggregate : FetchAggregate
{
    protected override object AggregateValues(IEnumerable<object> values)
    {
        return values.Count();
    }
}
