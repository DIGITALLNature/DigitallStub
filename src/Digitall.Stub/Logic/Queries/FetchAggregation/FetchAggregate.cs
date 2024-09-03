// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

abstract class FetchAggregate
{
    public string Attribute { get; set; }
    public string OutputAlias { get; set; }

    public object Process(IEnumerable<Entity> entities)
    {
        return AggregateValues(entities.Select(e =>
            e.Contains(Attribute) ? e[Attribute] : null
        ));
    }

    protected abstract object AggregateValues(IEnumerable<object> values);
}
