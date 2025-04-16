// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using Digitall.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Extensions;

public static class FakedDataverseBuilderExtensions
{
    public static FakedDataverseBuilder AddData(this FakedDataverseBuilder builder, IEnumerable<Entity> records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static FakedDataverseBuilder AddData(this FakedDataverseBuilder builder, params Entity[] records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    public static FakedDataverseBuilder AddOrganizationRequests(this FakedDataverseBuilder builder, IEnumerable<IOrganizationRequestFake> requests)
    {
        builder.OrganizationService.AddRequests(requests);
        return builder;
    }

    public static FakedDataverseBuilder AddOrganizationRequests(this FakedDataverseBuilder builder, params IOrganizationRequestFake[] requests)
    {
        builder.OrganizationService.AddRequests(requests);
        return builder;
    }
}
