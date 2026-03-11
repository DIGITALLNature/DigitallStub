// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class AssociateFake : OrganizationRequestFake<AssociateRequest, AssociateResponse>
{
    public override AssociateResponse Execute(AssociateRequest organizationRequest, FakeOrganizationService state)
    {
        state.Associate(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.Relationship, organizationRequest.RelatedEntities);

        return new AssociateResponse();
    }
}
