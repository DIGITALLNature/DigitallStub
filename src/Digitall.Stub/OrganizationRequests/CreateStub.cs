// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class CreateStub : OrganizationRequestStub<CreateRequest, CreateResponse>
{
    public override CreateResponse Execute(CreateRequest organizationRequest, DataverseStub state)
    {
        var guid = state.Create(organizationRequest.Target);

        return new CreateResponse
        {
            ResponseName = "Create",
            Results = new ParameterCollection { { "id", guid } }
        };
    }
}
