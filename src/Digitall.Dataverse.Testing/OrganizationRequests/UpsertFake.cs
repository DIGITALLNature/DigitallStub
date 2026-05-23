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

        var target = organizationRequest.Target;
        var entityLogicalName = target.LogicalName;
        var entityId = target.Id;

        bool recordCreated;
        if (state.EntityExists(entityLogicalName, entityId))
        {
            recordCreated = false;
            state.Update(target);

            // Deep insert: sub-entities are always created even when the parent is updated
            if (target.RelatedEntities.Count > 0)
            {
                DeepInsertProcessor.Process(entityLogicalName, entityId, target.RelatedEntities, state);
            }
        }
        else
        {
            recordCreated = true;
            // state.Create routes through CreateFake which handles deep insert
            entityId = state.Create(target);
        }

        var result = new UpsertResponse();
        result.Results.Add("RecordCreated", recordCreated);
        result.Results.Add("Target", new EntityReference(entityLogicalName, entityId));
        return result;
    }
}
