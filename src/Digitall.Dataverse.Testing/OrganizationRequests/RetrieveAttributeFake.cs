// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveAttributeFake : OrganizationRequestFake<RetrieveAttributeRequest, RetrieveAttributeResponse>
{
    public override RetrieveAttributeResponse Execute(RetrieveAttributeRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityMetadata = fakeOrganizationService.State.EntityMetadata[organizationRequest.EntityLogicalName];
        var attributeMetadata = entityMetadata.Attributes.First(a => a.LogicalName == organizationRequest.LogicalName);

        var results = new ParameterCollection { { nameof(RetrieveAttributeResponse.AttributeMetadata), attributeMetadata } };

        var response = new RetrieveAttributeResponse { Results = results };
        return response;
    }
}
