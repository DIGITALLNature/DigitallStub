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
public class ServiceClientBuilder : IFakeDataverseBuilder<FakeOrganizationServiceAsync>
{
    /// <summary>
    /// Creates a new instance of <see cref="ServiceClientBuilder"/> with a default <see cref="FakeOrganizationServiceAsync"/>.
    /// </summary>
    public ServiceClientBuilder()
    {
        OrganizationService = new FakeOrganizationServiceAsync();
        OrganizationService.AddDefaultRequests();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ServiceClientBuilder"/> with a <see cref="FakeOrganizationServiceAsync"/>
    /// using the given <see cref="TimeProvider"/>.
    /// </summary>
    public ServiceClientBuilder(TimeProvider timeProvider)
    {
        OrganizationService = new FakeOrganizationServiceAsync(timeProvider);
        OrganizationService.AddDefaultRequests();
    }

    public FakeOrganizationServiceAsync OrganizationService { get; }

    /// <summary>
    /// Returns the <see cref="IOrganizationService"/> for use in non-async scenarios.
    /// </summary>
    public IOrganizationService BuildOrganizationService() => OrganizationService;

    /// <summary>
    /// Returns the <see cref="IOrganizationServiceAsync2"/> for use in async/ServiceClient scenarios.
    /// </summary>
    public IOrganizationServiceAsync2 BuildOrganizationServiceAsync() => OrganizationService;
}
