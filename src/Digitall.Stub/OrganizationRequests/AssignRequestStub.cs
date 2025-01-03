// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Digitall.Stub.Errors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.OrganizationRequests;

public class AssignRequestStub : OrganizationRequestStub<AssignRequest, AssignResponse>
{
    public override AssignResponse Execute(AssignRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");


        var target = organizationRequest.Target;
        var assignee = organizationRequest.Assignee;

        if (target == null)
        {
            ErrorFactory.ThrowFault("Can not assign without target");
        }

        if (assignee == null)
        {
            ErrorFactory.ThrowFault("Can not assign without assignee");
        }

        var owningTable = assignee.LogicalName switch
        {
            "systemuser" => new KeyValuePair<string, object>("owninguser", assignee),
            "team" => new KeyValuePair<string, object>("owningteam", assignee),
            _ => throw new ArgumentOutOfRangeException(nameof(assignee.LogicalName))
        };

        var assignment = new Entity
        {
            LogicalName = target.LogicalName,
            Id = target.Id,
            Attributes = new AttributeCollection
            {
                { "ownerid", assignee },
                owningTable
            }
        };

        state.Update(assignment);

        return new AssignResponse();
    }
}
