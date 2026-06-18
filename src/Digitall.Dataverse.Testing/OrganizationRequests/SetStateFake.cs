// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class SetStateFake : OrganizationRequestFake<SetStateRequest, SetStateResponse>
{
    public override SetStateResponse Execute(SetStateRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(fakeOrganizationService);
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityName = organizationRequest.EntityMoniker.LogicalName;
        var entityId = organizationRequest.EntityMoniker.Id;

        var statusCode = organizationRequest.Status.Value == -1
            ? fakeOrganizationService.State.GetDefaultStatusCode(entityName, organizationRequest.State.Value)
            : organizationRequest.Status;

        var entityToUpdate = new Entity(entityName)
        {
            Id = entityId,
            ["statecode"] = organizationRequest.State,
            ["statuscode"] = statusCode
        };

        fakeOrganizationService.Update(entityToUpdate);

        return new SetStateResponse();
    }
}
