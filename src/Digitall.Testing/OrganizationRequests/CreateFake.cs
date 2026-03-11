// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class CreateFake : OrganizationRequestFake<CreateRequest, CreateResponse>
{
    public override CreateResponse Execute(CreateRequest organizationRequest, FakedDataverse state)
    {
        var guid = state.Create(organizationRequest.Target);

        return new CreateResponse
        {
            ResponseName = "Create",
            Results = new ParameterCollection { { nameof (CreateResponse.id), guid } }
        };
    }
}
