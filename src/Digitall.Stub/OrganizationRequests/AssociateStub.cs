// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class AssociateStub : OrganizationRequestStub<AssociateRequest, AssociateResponse>
{
    public override AssociateResponse Execute(AssociateRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        state.Associate(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.Relationship, organizationRequest.RelatedEntities);

        return new AssociateResponse();
    }
}
