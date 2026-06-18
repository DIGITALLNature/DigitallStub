// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveAttributeFake : IOrganizationRequestFake
{
    public Type ForType => typeof(RetrieveAttributeRequest);

    public OrganizationResponse Execute(OrganizationRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        return organizationRequest is RetrieveAttributeRequest retrieveAttributeRequest
            ? Execute(retrieveAttributeRequest, fakeOrganizationService)
            : throw new InvalidCastException($"Cannot cast {organizationRequest.GetType()} to {typeof(RetrieveAttributeRequest)}");
    }

    public RetrieveAttributeResponse Execute(RetrieveAttributeRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        var entityMetadata = fakeOrganizationService.State.EntityMetadata[organizationRequest.EntityLogicalName];
        var attributeMetadata = entityMetadata.Attributes.First(a => a.LogicalName == organizationRequest.LogicalName);

        var results = new ParameterCollection { { nameof(RetrieveAttributeResponse.AttributeMetadata), attributeMetadata } };

        var response = new RetrieveAttributeResponse { Results = results };
        return response;
    }
}
