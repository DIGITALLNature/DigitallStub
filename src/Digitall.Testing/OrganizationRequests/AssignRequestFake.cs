// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using Digitall.Testing.Errors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.OrganizationRequests;

public class AssignRequestFake : OrganizationRequestFake<AssignRequest, AssignResponse>
{
    public override AssignResponse Execute(AssignRequest organizationRequest, FakeOrganizationService state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(organizationRequest);


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
