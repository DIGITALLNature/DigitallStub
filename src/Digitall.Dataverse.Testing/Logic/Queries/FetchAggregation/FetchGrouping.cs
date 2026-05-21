// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic.Queries.FetchAggregation;

internal abstract class FetchGrouping
{
    public required string Attribute { get; init; }
    public required string OutputAlias { get; init; }

    public IComparable? Process(Entity entity)
    {
        var attr = entity.Contains(Attribute) ? entity[Attribute] : null;
        return FindGroupValue(attr);
    }

    protected abstract IComparable? FindGroupValue(object? attributeValue);
}
