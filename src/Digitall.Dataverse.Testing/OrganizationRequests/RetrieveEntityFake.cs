// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveEntityFake : OrganizationRequestFake<RetrieveEntityRequest, RetrieveEntityResponse>
{
    public override RetrieveEntityResponse Execute(RetrieveEntityRequest organizationRequest, FakeOrganizationService state)
    {
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        var entityMetadata = state.EntityMetadata[organizationRequest.LogicalName];

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

