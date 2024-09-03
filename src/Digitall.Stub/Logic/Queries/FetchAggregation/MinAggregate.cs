// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class MinAggregate : AliasedAggregate
{
    protected override object AggregateAliasedValues(IEnumerable<object> values)
    {
        var lst = values.Where(x => x != null);
        if (!lst.Any()) return null;
         
        var firstValue = lst.Where(x => x != null).First();
        var valType = firstValue.GetType();
         
        if (valType == typeof(decimal) || valType == typeof(decimal?))
        {
            return lst.Min(x => (decimal)x);
        }
         
        if (valType == typeof(Money))
        {
            return new Money(lst.Min(x => (x as Money).Value));
        }
         
        if (valType == typeof(int) || valType == typeof(int?))
        {
            return lst.Min(x => (int)x);
        }
         
        if (valType == typeof(float) || valType == typeof(float?))
        {
            return lst.Min(x => (float)x);
        }
         
        if (valType == typeof(double) || valType == typeof(double?))
        {
            return lst.Min(x => (double)x);
        }
                         
        if (valType == typeof(DateTime) || valType == typeof(DateTime?))
        {
            return lst.Min(x => (DateTime)x);
        }
         
        throw new Exception("Unhndled property type '" + valType.FullName + "' in 'min' aggregate");
    }
}
