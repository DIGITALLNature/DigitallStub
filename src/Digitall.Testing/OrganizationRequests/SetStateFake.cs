// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.OrganizationRequests;

public class SetStateFake : OrganizationRequestFake<SetStateRequest, SetStateResponse>
{
    public override SetStateResponse Execute(SetStateRequest organizationRequest, FakeOrganizationService state)
    { 
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityName = organizationRequest.EntityMoniker.LogicalName;
        var entityId = organizationRequest.EntityMoniker.Id;

        var entityToUpdate = new Entity(entityName) { Id = entityId, ["statecode"] = organizationRequest.State, ["statuscode"] = organizationRequest.Status };
        
        state.Update(entityToUpdate);

        return new SetStateResponse();
    }
}
