// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class ArrayComparer : IEqualityComparer<IComparable[]>
{
    public bool Equals(IComparable[] x, IComparable[] y)
    {
        return x.SequenceEqual(y);
    }

    public int GetHashCode(IComparable[] obj)
    {
        int result = 0;
        foreach (IComparable x in obj)
        {
            result ^= x == null ? 0 : x.GetHashCode();
        }
        return result;
    }
}
