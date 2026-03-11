// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Diagnostics;
using System.Linq;
using Digitall.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.OrganizationRequests;

public class RetrieveFake : OrganizationRequestFake<RetrieveRequest, RetrieveResponse>
{
    public override RetrieveResponse Execute(RetrieveRequest organizationRequest, FakedDataverse state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        Entity record;
        if (organizationRequest.Target.Id == Guid.Empty && organizationRequest.Target.KeyAttributes.Count > 0)
        {
            record = state.RetrieveWithAlternateKey(organizationRequest.Target.LogicalName, organizationRequest.Target.KeyAttributes, organizationRequest.ColumnSet);
        }
        else
        {
            record = state.Retrieve(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.ColumnSet);
        }

        return new RetrieveResponse
        {
            ResponseName = "Retrieve",
            Results = new ParameterCollection { { nameof (RetrieveResponse.Entity), record } }
        };
    }
}
