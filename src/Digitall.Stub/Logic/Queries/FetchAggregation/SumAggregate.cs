// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class SumAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object> values)
    {
        var lst = values.ToList().Where(x => x != null);
        // TODO: Check these cases in CRM proper
        if (!lst.Any()) return null;
         
        var valType = lst.First().GetType();
         
        if (valType == typeof(decimal) || valType == typeof(decimal?))
        {
            return lst.Sum(x => x as decimal? ?? 0m);
        }
        if (valType == typeof(Money))
        {
            return new Money(lst.Sum(x => (x as Money)?.Value ?? 0m));
        }
         
        if (valType == typeof(int) || valType == typeof(int?))
        {
            return lst.Sum(x => x as int? ?? 0);
        }
         
        if (valType == typeof(float) || valType == typeof(float?))
        {
            return lst.Sum(x => x as float? ?? 0f);
        }
         
        if (valType == typeof(double) || valType == typeof(double?))
        {
            return lst.Sum(x => x as double? ?? 0d);
        }
                       
        throw new Exception("Unhndled property type '" + valType.FullName + "' in 'sum' aggregate");
    }
}
