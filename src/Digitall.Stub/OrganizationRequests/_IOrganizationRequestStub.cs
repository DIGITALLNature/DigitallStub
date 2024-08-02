// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.OrganizationRequests;

public interface IOrganizationRequestStub
{
    Type ForType { get; }
    OrganizationResponse Execute(OrganizationRequest organizationRequest,DataverseStub state);
}
