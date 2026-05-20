// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Digitall.Testing.Logic.Queries.FetchAggregation;

class ArrayComparer : IEqualityComparer<IComparable?[]>
{
    public bool Equals(IComparable?[]? x, IComparable?[]? y)
    {
        if (x is null || y is null) return ReferenceEquals(x, y);
        return x.SequenceEqual(y);
    }

    public int GetHashCode(IComparable?[] obj)
    {
        var result = 0;
        foreach (var x in obj)
        {
            result ^= x?.GetHashCode() ?? 0;
        }
        return result;
    }
}
