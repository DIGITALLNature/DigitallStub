// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Diagnostics;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.OrganizationRequests;

public abstract class OrganizationRequestStub<TIn, TOut> : IOrganizationRequestStub where TIn : OrganizationRequest where TOut : OrganizationResponse
{
    public Type ForType => typeof(TIn);

    public OrganizationResponse Execute(OrganizationRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        if(organizationRequest is not TIn @in)
        {
            throw new InvalidCastException($"Cannot cast {organizationRequest.GetType()} to {typeof(TIn)}");
        }
        return Execute(@in, state);
    }

    public abstract TOut Execute(TIn organizationRequest,DataverseStub state);
}
