// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public abstract class OrganizationRequestFake<TIn, TOut> : IOrganizationRequestFake where TIn : OrganizationRequest where TOut : OrganizationResponse
{
    public Type ForType => typeof(TIn);

    public OrganizationResponse Execute(OrganizationRequest organizationRequest, FakeOrganizationService state)
    {
        if(organizationRequest is not TIn @in)
        {
            throw new InvalidCastException($"Cannot cast {organizationRequest.GetType()} to {typeof(TIn)}");
        }
        return Execute(@in, state);
    }

    public abstract TOut Execute(TIn organizationRequest, FakeOrganizationService state);
}
