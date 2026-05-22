// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class UpdateFake : OrganizationRequestFake<UpdateRequest, UpdateResponse>
{
    public override UpdateResponse Execute(UpdateRequest organizationRequest, FakeOrganizationService state)
    {
        state.UpdateCore(organizationRequest.Target);

        return new UpdateResponse();
    }
}
