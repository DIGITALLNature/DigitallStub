// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public interface IOrganizationRequestFake
{
    Type ForType { get; }
    OrganizationResponse Execute(OrganizationRequest organizationRequest, FakeOrganizationService state);
}
