// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.OrganizationRequests;

public class SetStateStub : OrganizationRequestStub<SetStateRequest, SetStateResponse>
{
    public override SetStateResponse Execute(SetStateRequest organizationRequest, DataverseStub state)
    { 
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        var entityName = organizationRequest.EntityMoniker.LogicalName;
        var entityId = organizationRequest.EntityMoniker.Id;

        var entityToUpdate = new Entity(entityName) { Id = entityId };
        entityToUpdate["statecode"] = organizationRequest.State;
        entityToUpdate["statuscode"] = organizationRequest.Status;
        
        state.Update(entityToUpdate);

        return new SetStateResponse();
    }
}
