// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

abstract class FetchGrouping
{
    public string Attribute { get; set; }
    public string OutputAlias { get; set; }

    public IComparable Process(Entity entity)
    {
        var attr = entity.Contains(Attribute) ? entity[Attribute] : null;
        return FindGroupValue(attr);
    }

    public abstract IComparable FindGroupValue(object attributeValue);
}
