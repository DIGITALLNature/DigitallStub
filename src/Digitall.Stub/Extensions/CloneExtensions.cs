// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Force.DeepCloner;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public static class CloneExtensions
{
    public static Entity CloneEntity(this Entity entity)
    {
        var cloned = entity.DeepClone();
        return cloned;
    }

    public static QueryExpression CloneQuery(this QueryExpression queryExpression)
    {
        var cloned = queryExpression.DeepClone();
        return cloned;
    }
}
