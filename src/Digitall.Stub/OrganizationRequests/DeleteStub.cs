// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class DeleteStub : OrganizationRequestStub<DeleteRequest, DeleteResponse>
{
    public override DeleteResponse Execute(DeleteRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        state.Delete(organizationRequest.Target.LogicalName, organizationRequest.Target.Id);

        return new DeleteResponse();
    }
}
