// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic.Queries.FetchAggregation;

internal class MaxAggregate : AliasedAggregate
{
    protected override object? AggregateAliasedValues(IEnumerable<object?> values)
    {
        var lst = values.Where(x => x != null).ToList();
        if (lst.Count == 0) return null;

        var valType = lst[0]!.GetType();

        if (valType == typeof(decimal) || valType == typeof(decimal?))
        {
            return lst.Max(x => (decimal)x!);
        }

        if (valType == typeof(Money))
        {
            return new Money(lst.Max(x => ((Money)x!).Value));
        }

        if (valType == typeof(int) || valType == typeof(int?))
        {
            return lst.Max(x => (int)x!);
        }

        if (valType == typeof(float) || valType == typeof(float?))
        {
            return lst.Max(x => (float)x!);
        }

        if (valType == typeof(double) || valType == typeof(double?))
        {
            return lst.Max(x => (double)x!);
        }

        if (valType == typeof(DateTime) || valType == typeof(DateTime?))
        {
            return lst.Max(x => (DateTime)x!);
        }

        throw new Exception("Unhndled property type '" + valType.FullName + "' in 'max' aggregate");
    }
}
