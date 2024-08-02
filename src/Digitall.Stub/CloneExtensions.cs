// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Text.Json;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public static class CloneExtensions
{
    private static readonly JsonSerializerOptions s_serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    public static Entity CloneEntity(this Entity entity)
    {
        var serialized = JsonSerializer.Serialize(entity, s_serializerOptions);
        return JsonSerializer.Deserialize<Entity>(serialized, s_serializerOptions);
    }

    public static QueryExpression CloneQuery(this QueryExpression queryExpression)
    {
        var serialized = JsonSerializer.Serialize(queryExpression, s_serializerOptions);
        return JsonSerializer.Deserialize<QueryExpression>(serialized, s_serializerOptions);
    }


}
