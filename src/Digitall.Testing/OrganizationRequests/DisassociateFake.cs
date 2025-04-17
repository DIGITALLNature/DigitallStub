// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class DisassociateFake : OrganizationRequestFake<DisassociateRequest, DisassociateResponse>
{
    public override DisassociateResponse Execute(DisassociateRequest organizationRequest, FakedDataverse state)
    {
        state.Disassociate(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.Relationship, organizationRequest.RelatedEntities);

        return new DisassociateResponse();
    }
}
