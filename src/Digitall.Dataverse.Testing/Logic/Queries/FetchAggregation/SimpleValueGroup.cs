// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic.Queries.FetchAggregation;

internal class SimpleValueGroup : FetchGrouping
{
    protected override IComparable? FindGroupValue(object? attributeValue)
    {
        if (attributeValue is EntityReference entityRef)
        {
            return new ComparableEntityReference(entityRef);
        }

        return attributeValue as IComparable;
    }
}
