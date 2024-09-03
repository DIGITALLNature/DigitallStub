// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class AvgAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object> values)
    {
        var lst = values.Where(x => x != null);
        if (!lst.Any()) return null;
         
        var firstValue = lst.First();
        var valType = firstValue.GetType();
         
        if (valType == typeof(decimal) || valType == typeof(decimal?))
        {
            return lst.Average(x => (decimal)x);
        }
         
        if (valType == typeof(Money))
        {
            return new Money(lst.Average(x => (x as Money).Value));
        }
         
        if (valType == typeof(int) || valType == typeof(int?))
        {
            return lst.Average(x => (int)x);
        }
         
        if (valType == typeof(float) || valType == typeof(float?))
        {
            return lst.Average(x => (float)x);
        }
         
        if (valType == typeof(double) || valType == typeof(double?))
        {
            return lst.Average(x => (double)x);
        }
         
        throw new Exception("Unhndled property type '" + valType.FullName + "' in 'avg' aggregate");
    }
}
