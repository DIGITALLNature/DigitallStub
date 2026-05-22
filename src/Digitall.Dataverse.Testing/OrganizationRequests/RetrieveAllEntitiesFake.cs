// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveAllEntitiesFake : OrganizationRequestFake<RetrieveAllEntitiesRequest, RetrieveAllEntitiesResponse>
{
    public override RetrieveAllEntitiesResponse Execute(RetrieveAllEntitiesRequest organizationRequest, FakeOrganizationService state)
    {
        var knownMetadata = state.State.EntityMetadata.Values.ToArray();
        return new RetrieveAllEntitiesResponse
        {
            Results =
            {
                ["EntityMetadata"] = knownMetadata
            }
        };
    }
}
