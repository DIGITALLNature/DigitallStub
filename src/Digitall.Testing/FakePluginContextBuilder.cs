// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.


namespace Digitall.Testing;

/// <summary>
/// A <see cref="PluginExecutionContextBuilder"/> that defaults <see cref="PluginExecutionContextBuilder.OrganizationService"/>
/// to a fully initialized <see cref="FakeOrganizationService"/> instance.
/// </summary>
public class FakePluginContextBuilder : PluginExecutionContextBuilder, IFakeDataverseBuilder<FakeOrganizationService>
{
    private readonly TimeProvider _timeProvider;

    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with a default <see cref="FakeOrganizationService"/>.
    /// </summary>
    public FakePluginContextBuilder() : this(TimeProvider.System)
    {
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with a <see cref="FakeOrganizationService"/>
    /// using the given <see cref="TimeProvider"/>.
    /// </summary>
    public FakePluginContextBuilder(TimeProvider timeProvider) : base(ConfigureOrganizationService(timeProvider))
    {
        _timeProvider = timeProvider;
    }

    /// <summary>
    /// Creates a new instance of <see cref="FakePluginContextBuilder"/> with the provided <see cref="FakeOrganizationService"/> instance.
    /// </summary>
    public FakePluginContextBuilder(FakeOrganizationService organizationService) : base(organizationService)
    {
        _timeProvider = organizationService.TimeProvider;
    }

    /// <summary>
    /// Strongly typed access to the underlying <see cref="FakeOrganizationService"/>.
    /// </summary>
    public FakeOrganizationService GetOrganizationService() =>
        OrganizationService as FakeOrganizationService ?? throw new InvalidOperationException("OrganizationService is not a FakeOrganizationService.");

    protected override void ConfigureServices(IServiceProviderMock serviceProvider)
    {
        base.ConfigureServices(serviceProvider);
        serviceProvider.GetService(typeof(TimeProvider)).Returns(_timeProvider);
    }

    private static FakeOrganizationService ConfigureOrganizationService(TimeProvider timeProvider)
    {
        var organizationService = new FakeOrganizationService(timeProvider);
        organizationService.AddDefaultRequests();

        return organizationService;
    }
}
