// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Testing;

/// <summary>
/// A <see cref="PluginExecutionContextBuilder"/> that defaults <see cref="PluginExecutionContextBuilder.OrganizationService"/>
/// to a fully initialized <see cref="FakeOrganizationService"/> instance.
/// </summary>
public class FakePluginContextBuilder : PluginExecutionContextBuilder, IFakeDataverseBuilder<FakeOrganizationService>
{
    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with a default <see cref="FakeOrganizationService"/>.
    /// </summary>
    public FakePluginContextBuilder()
    {
        var svc = new FakeOrganizationService();
        svc.AddDefaultRequests();
        OrganizationService = svc;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with a <see cref="FakeOrganizationService"/>
    /// using the given <see cref="TimeProvider"/>.
    /// </summary>
    public FakePluginContextBuilder(TimeProvider timeProvider)
    {
        var svc = new FakeOrganizationService(timeProvider);
        svc.AddDefaultRequests();
        OrganizationService = svc;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with the provided <see cref="FakeOrganizationService"/> instance.
    /// </summary>
    public FakePluginContextBuilder(FakeOrganizationService fakeOrganizationService)
    {
        OrganizationService = fakeOrganizationService;
    }

    /// <summary>
    /// Strongly typed access to the underlying <see cref="FakeOrganizationService"/>.
    /// </summary>
    public new FakeOrganizationService OrganizationService
    {
        get => base.OrganizationService as FakeOrganizationService ?? throw new InvalidOperationException("OrganizationService is not a FakeOrganizationService.");
        private set => base.OrganizationService = value;
    }
}
