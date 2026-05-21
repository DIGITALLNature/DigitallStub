// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic.Queries.FetchAggregation;

internal class ComparableEntityReference(EntityReference entityReference) : IComparable
{
    public EntityReference EntityReference { get; } = entityReference;

    int IComparable.CompareTo(object? obj)
    {
        return Equals(obj) ? 0 : 1;
    }

    public override bool Equals(object? obj)
    {
        EntityReference? other;
        if (obj is EntityReference entityRef)
        {
            other = entityRef;
        }
        else if (obj is ComparableEntityReference comparableRef)
        {
            other = comparableRef.EntityReference;
        }
        else
        {
            return false;
        }
        return EntityReference.Id == other.Id && EntityReference.LogicalName == other.LogicalName;
    }

    public override int GetHashCode()
    {
        return (EntityReference.LogicalName?.GetHashCode() ?? 0) ^ EntityReference.Id.GetHashCode();
    }
}
