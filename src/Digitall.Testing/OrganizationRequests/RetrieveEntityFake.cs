// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class RetrieveEntityFake : OrganizationRequestFake<RetrieveEntityRequest, RetrieveEntityResponse>
{
    public override RetrieveEntityResponse Execute(RetrieveEntityRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityMetadata = fakeOrganizationService.State.EntityMetadata[organizationRequest.LogicalName];

        var results = new ParameterCollection {
            { nameof (RetrieveEntityResponse.EntityMetadata), entityMetadata }
        };

        var response = new RetrieveEntityResponse
        {
           Results = results
        };
        return response;
    }
}

