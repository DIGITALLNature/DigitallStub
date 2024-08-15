// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class UpdateStub : OrganizationRequestStub<UpdateRequest, UpdateResponse>
{
    public override UpdateResponse Execute(UpdateRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        state.Update(organizationRequest.Target);

        return new UpdateResponse();
    }
}
