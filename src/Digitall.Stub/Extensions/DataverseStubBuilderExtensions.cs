// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Extensions;

public static class DataverseStubBuilderExtensions
{
    public static DataverseStubBuilder WithData(this DataverseStubBuilder builder, params Entity[] records)
    {
        builder.OrganizationService.AddRange(records);
        return builder;
    }

    // TODO add more extension methods e.g. to add stubs
}
