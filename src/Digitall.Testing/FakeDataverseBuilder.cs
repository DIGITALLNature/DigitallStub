// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing;

/// <summary>
/// Builder for Power Platform ServiceClient scenarios.
/// Initializes and exposes <see cref="IOrganizationService"/> and <see cref="IOrganizationServiceAsync"/>.
/// </summary>
public class FakeServiceClientBuilder : IFakeDataverseBuilder<FakeOrganizationServiceAsync>
{
    private readonly FakeOrganizationServiceAsync _organizationService;

    /// <summary>
    /// Creates a new instance of <see cref="FakeServiceClientBuilder"/> with a default <see cref="FakeOrganizationServiceAsync"/>.
    /// </summary>
    public FakeServiceClientBuilder()
    {
        _organizationService = new FakeOrganizationServiceAsync();
        _organizationService.AddDefaultRequests();
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakeServiceClientBuilder"/> with a <see cref="FakeOrganizationServiceAsync"/>
    /// using the given <see cref="TimeProvider"/>.
    /// </summary>
    public FakeServiceClientBuilder(TimeProvider timeProvider)
    {
        _organizationService = new FakeOrganizationServiceAsync(timeProvider);
        _organizationService.AddDefaultRequests();
    }

    public FakeOrganizationServiceAsync GetOrganizationService() => _organizationService;
}
