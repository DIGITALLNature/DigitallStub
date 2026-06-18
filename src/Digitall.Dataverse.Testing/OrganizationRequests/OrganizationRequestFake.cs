// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public abstract class OrganizationRequestFake<TRequest, TResponse> : IOrganizationRequestFake where TRequest : OrganizationRequest where TResponse : OrganizationResponse
{
    public Type ForType => typeof(TRequest);

    public OrganizationResponse Execute(OrganizationRequest organizationRequest, FakeOrganizationService fakeOrganizationService)
    {
        return organizationRequest is TRequest request ? Execute(request, fakeOrganizationService) : throw new InvalidCastException($"Cannot cast {organizationRequest.GetType()} to {typeof(TRequest)}");
    }

    public abstract TResponse Execute(TRequest organizationRequest, FakeOrganizationService fakeOrganizationService);
}
