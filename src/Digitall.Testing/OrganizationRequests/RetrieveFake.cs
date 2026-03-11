// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class RetrieveFake : OrganizationRequestFake<RetrieveRequest, RetrieveResponse>
{
    public override RetrieveResponse Execute(RetrieveRequest organizationRequest, FakedDataverse state)
    {
        Entity record = state.Retrieve(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.ColumnSet);

        return new RetrieveResponse
        {
            ResponseName = "Retrieve",
            Results = new ParameterCollection { { nameof (RetrieveResponse.Entity), record } }
        };
    }
}
