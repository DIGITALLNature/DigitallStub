// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Testing;

public class FakedDataverseBuilder : PluginExecutionContextBuilder
{
    public FakedDataverseBuilder()
    {
        OrganizationService = new FakedDataverse();
        OrganizationService.AddDefaultRequests();
    }

    public new FakedDataverse OrganizationService
    {
        get => base.OrganizationService as FakedDataverse ?? throw new InvalidOperationException(@"¯\_(ツ)_/¯");
        private set { base.OrganizationService = value; }
    }
}
