// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class CreateFake : OrganizationRequestFake<CreateRequest, CreateResponse>
{
    public override CreateResponse Execute(CreateRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        var target = organizationRequest.Target;
        var guid = fakeOrganizationService.CreateCore(target);

        // Deep insert: create sub-entities from RelatedEntities
        if (target.RelatedEntities.Count > 0)
        {
            DeepInsertProcessor.Process(target.LogicalName, guid, target.RelatedEntities, fakeOrganizationService);
        }

        return new CreateResponse
        {
            ResponseName = "Create",
            Results = new ParameterCollection { { nameof (CreateResponse.id), guid } }
        };
    }
}
