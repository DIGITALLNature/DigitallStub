// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Stub;

public class DataverseStubBuilder : PluginExecutionContextBuilder
{
    public DataverseStubBuilder()
    {
        OrganizationService = new DataverseStub();
        OrganizationService.AddDefaultStubs();
    }

    public new DataverseStub OrganizationService
    {
        get => base.OrganizationService as DataverseStub ?? throw new InvalidOperationException(@"¯\_(ツ)_/¯");
        set { base.OrganizationService = value; }
    }
}
