// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class UpdateFake : OrganizationRequestFake<UpdateRequest, UpdateResponse>
{
    public override UpdateResponse Execute(UpdateRequest organizationRequest, FakedDataverse state)
    {
        state.Update(organizationRequest.Target);

        return new UpdateResponse();
    }
}
