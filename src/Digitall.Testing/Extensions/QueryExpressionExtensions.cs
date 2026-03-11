// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Force.DeepCloner;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Extensions;

public static class QueryExpressionExtensions
{
    extension(QueryExpression queryExpression)
    {
        public QueryExpression CloneQuery()
        {
            var cloned = queryExpression.DeepClone();
            return cloned;
        }
    }
}
