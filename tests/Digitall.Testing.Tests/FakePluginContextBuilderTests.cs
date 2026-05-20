// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Extensions.Time.Testing;

namespace Digitall.Testing.Tests;

public class FakePluginContextBuilderTests
{
    [Test]
    public async Task GetOrganizationService_Should_Return_FakeOrganizationService()
    {
        var service = new FakePluginContextBuilder().GetOrganizationService();

        await Assert.That(service).IsNotNull();
        await Assert.That(service).IsTypeOf<FakeOrganizationService>();
    }

    [Test]
    public async Task FakePluginContextBuilder_With_Custom_TimeProvider()
    {
        var service = new FakePluginContextBuilder(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetOrganizationService();

        await Assert.That(service).IsNotNull();
        await Assert.That(service).IsTypeOf<FakeOrganizationService>();
        await Assert.That(service.TimeProvider.GetUtcNow().Year).IsEqualTo(2000);
    }

    [Test]
    public async Task FakePluginContextBuilder_With_Custom_FakeOrganizationService()
    {
        var fakeOrganizationService = new FakeOrganizationService(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        var service = new FakePluginContextBuilder(fakeOrganizationService).GetOrganizationService();

        await Assert.That(service).IsNotNull();
        await Assert.That(service).IsTypeOf<FakeOrganizationService>();
        await Assert.That(service.TimeProvider.GetUtcNow().Year).IsEqualTo(2000);
    }
}
