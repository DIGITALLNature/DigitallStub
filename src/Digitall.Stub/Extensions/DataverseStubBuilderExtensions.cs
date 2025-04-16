// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using Digitall.Stub.OrganizationRequests;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Extensions;

public static class DataverseStubBuilderExtensions
{
    public static DataverseStubBuilder AddData(this DataverseStubBuilder builder, IEnumerable<Entity> records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static DataverseStubBuilder AddData(this DataverseStubBuilder builder, params Entity[] records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static DataverseStubBuilder AddOrganizationRequests(this DataverseStubBuilder builder, IEnumerable<IOrganizationRequestStub> requestStubs)
    {
        builder.OrganizationService.AddStubs(requestStubs);
        return builder;
    }

    public static DataverseStubBuilder AddOrganizationRequests(this DataverseStubBuilder builder, params IOrganizationRequestStub[] requestStubs)
    {
        builder.OrganizationService.AddStubs(requestStubs);
        return builder;
    }
}
