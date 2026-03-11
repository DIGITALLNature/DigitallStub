// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class DeleteFake : OrganizationRequestFake<DeleteRequest, DeleteResponse>
{
    public override DeleteResponse Execute(DeleteRequest organizationRequest, FakeOrganizationService state)
    {
        state.Delete(organizationRequest.Target.LogicalName, organizationRequest.Target.Id);

        return new DeleteResponse();
    }
}
