// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Text.Json;
using Force.DeepCloner;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public static class CloneExtensions
{
    public static Entity CloneEntity(this Entity entity)
    {
        var cloned = entity.DeepClone();
        // var cloned = new Entity(entity.LogicalName, entity.Id);
        //
        // if (entity.FormattedValues != null)
        // {
        //     foreach (var formattedValue in entity.FormattedValues)
        //     {
        //         cloned.FormattedValues.Add(formattedValue);
        //     }
        // }
        //
        // foreach (var attribute in entity.Attributes)
        // {
        //     cloned.Attributes.Add(attribute.Key,Extensions.CloneAttribute(attribute.Value));
        // }
        //
        //
        // foreach (var keyAttribute in entity.KeyAttributes)
        // {
        //     cloned.KeyAttributes.Add(keyAttribute.Key,Extensions.CloneAttribute(keyAttribute.Value));
        // }

        return cloned;
    }

    public static QueryExpression CloneQuery(this QueryExpression queryExpression)
    {
        var cloned = queryExpression.DeepClone();
        return cloned;
    }
}
