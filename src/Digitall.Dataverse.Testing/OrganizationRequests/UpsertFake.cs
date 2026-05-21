// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class UpsertFake : OrganizationRequestFake<UpsertRequest, UpsertResponse>
{
    public override UpsertResponse Execute(UpsertRequest organizationRequest, FakeOrganizationService state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityLogicalName = organizationRequest.Target.LogicalName;
        var entityId = organizationRequest.Target.Id;

        bool recordCreated;
        if (state.EntityExists(entityLogicalName, entityId))
        {
            recordCreated = false;
            state.Update(organizationRequest.Target);
        }
        else
        {
            recordCreated = true;
            entityId = state.Create(organizationRequest.Target);
        }

        var result = new UpsertResponse();
        result.Results.Add("RecordCreated", recordCreated);
        result.Results.Add("Target", new EntityReference(entityLogicalName, entityId));
        return result;
    }
}
