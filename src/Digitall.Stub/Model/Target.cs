// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Model;

public record Target
{
    public Target(Entity target)
    {
        Id = target.Id;
        LogicalName = target.LogicalName;
        Value = target;
    }

    public Target(EntityReference target)
    {
        Id = target.Id;
        LogicalName = target.LogicalName;
        Value = target;
    }

    public Guid Id { get; }
    public string LogicalName { get; }
    public object Value { get; }
}
