// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class SimpleValueGroup : FetchGrouping
{
    public override IComparable FindGroupValue(object attributeValue)
    {
        if (attributeValue is EntityReference)
        {
            return new ComparableEntityReference(attributeValue as EntityReference) as IComparable;
        }
        else
        {
            return attributeValue as IComparable;
        }
    }
}
