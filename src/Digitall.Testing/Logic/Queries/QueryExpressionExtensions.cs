// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Logic.Queries;

public static class QueryExpressionExtensions
{
    public static string GetEntityNameFromAlias(this QueryExpression queryExpression, string? alias)
    {
        if (alias == null)
            return queryExpression.EntityName;

        var linkedEntity = queryExpression.LinkEntities
            .FirstOrDefault(le => le.EntityAlias != null && le.EntityAlias.Equals(alias, StringComparison.Ordinal));

        if (linkedEntity != null)
        {
            return linkedEntity.LinkToEntityName;
        }

        //If the alias wasn't found, it means it  could be any of the EntityNames
        return alias;
    }
}
