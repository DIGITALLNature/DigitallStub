// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class UpsertStub : OrganizationRequestStub<UpsertRequest, UpsertResponse>
{
    public override UpsertResponse Execute(UpsertRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        var entityLogicalName = organizationRequest.Target.LogicalName;
        var entityId = organizationRequest.Target.Id;

        bool recordCreated;
        if (state.State.ContainsKey(entityLogicalName) && state.State[entityLogicalName].ContainsKey(entityId))
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
