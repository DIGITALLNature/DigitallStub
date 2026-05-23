// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Logic.Queries;

public static class QueryExpressionExtensions
{
    public static string GetEntityNameFromAlias(this QueryExpression queryExpression, string? alias)
    {
        if (alias == null) return queryExpression.EntityName;

        var linkedEntity = FindLinkEntityByAlias(queryExpression.LinkEntities, alias);

        return linkedEntity != null ? linkedEntity.LinkToEntityName : alias; //If the alias wasn't found, it means it could be any of the EntityNames
    }

    private static LinkEntity? FindLinkEntityByAlias(DataCollection<LinkEntity> linkEntities, string alias)
    {
        foreach (var le in linkEntities)
        {
            if (le.EntityAlias != null && le.EntityAlias.Equals(alias, StringComparison.Ordinal))
            {
                return le;
            }

            // Recursively search nested LinkEntities
            var nested = FindLinkEntityByAlias(le.LinkEntities, alias);
            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }
}
