// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveAllEntitiesFake : OrganizationRequestFake<RetrieveAllEntitiesRequest, RetrieveAllEntitiesResponse>
{
    public override RetrieveAllEntitiesResponse Execute(RetrieveAllEntitiesRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        var knownMetadata = fakeOrganizationService.State.EntityMetadata.Values.ToArray();
        return new RetrieveAllEntitiesResponse
        {
            Results =
            {
                ["EntityMetadata"] = knownMetadata
            }
        };
    }
}
