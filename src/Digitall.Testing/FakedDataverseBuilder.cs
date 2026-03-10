// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing;

public class FakedDataverseBuilder : PluginExecutionContextBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="FakedDataverseBuilder"/> with a default instance of <see cref="FakedDataverse"/>.
    /// </summary>
    public FakedDataverseBuilder()
    {
        OrganizationService = new FakedDataverse();
        OrganizationService.AddDefaultRequests();
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakedDataverseBuilder"/> with an instance of <see cref="FakedDataverse"/>
    /// that uses the given <see cref="TimeProvider"/>.
    /// </summary>
    public FakedDataverseBuilder(TimeProvider timeProvider)
    {
        // Create a new FakedDataverse instance with the given TimeProvider
        OrganizationService = new FakedDataverse(timeProvider);

        // Add all default requests to the OrganizationService
        OrganizationService.AddDefaultRequests();
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakedDataverseBuilder"/> with the provided <see cref="FakedDataverse"/> instance.
    /// </summary>
    /// <param name="fakedDataverse">An existing instance of <see cref="FakedDataverse"/> to use as the organization service.</param>
    public FakedDataverseBuilder(FakedDataverse fakedDataverse)
    {
        // Assign the provided FakedDataverse instance to the OrganizationService
        OrganizationService = fakedDataverse;
    }

    public new FakedDataverse OrganizationService
    {
        get => base.OrganizationService as FakedDataverse ?? throw new InvalidOperationException(@"¯\_(ツ)_/¯");
        private set { base.OrganizationService = value; }
    }

    public void AddConfig(string key, string defaultvalue, string? value = null)
    {
        var envVarDef = new Entity("environmentvariabledefinition")
        {
            Id = Guid.NewGuid(),
            ["schemaname"] = key,
            ["defaultvalue"] = defaultvalue
        };
        OrganizationService.Add(envVarDef);

        if (!string.IsNullOrWhiteSpace(value))
        {
            var envVarVal = new Entity("environmentvariablevalue")
            {
                Id = Guid.NewGuid(),
                ["environmentvariabledefinitionid"] = envVarDef.ToEntityReference(),
                ["value"] = value
            };
            OrganizationService.Add(envVarVal);
        }
    }
}
