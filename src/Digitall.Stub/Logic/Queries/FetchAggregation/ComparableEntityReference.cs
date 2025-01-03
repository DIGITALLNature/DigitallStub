// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class ComparableEntityReference : IComparable
{
    public EntityReference entityReference { get; private set; }

    public ComparableEntityReference(EntityReference entityReference)
    {
        this.entityReference = entityReference;
    }

    int IComparable.CompareTo(object obj)
    {
        return Equals(obj) ? 0 : 1;
    }

    public override bool Equals(object obj)
    {
        EntityReference other;
        if (obj is EntityReference)
        {
            other = obj as EntityReference;
        }
        else if (obj is ComparableEntityReference)
        {
            other = (obj as ComparableEntityReference).entityReference;
        }
        else
        {
            return false;
        }
        return entityReference.Id == other.Id && entityReference.LogicalName == other.LogicalName;
    }

    public override int GetHashCode()
    {
        return (entityReference.LogicalName == null ? 0 : entityReference.LogicalName.GetHashCode()) ^ entityReference.Id.GetHashCode();
    }
}
