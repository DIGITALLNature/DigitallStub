// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Microsoft.Extensions.Time.Testing;

namespace Digitall.Testing.Tests;

[TestClass]
public class FakePluginContextBuilderTests
{
    [TestMethod]
    public void GetOrganizationService_Should_Return_FakeOrganizationService()
    {
        var service = new FakePluginContextBuilder().GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
    }

    [TestMethod]
    public void FakePluginContextBuilder_With_Custom_TimeProvider()
    {
        var service = new FakePluginContextBuilder(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))).GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
        service.TimeProvider.GetUtcNow().Year.Should().Be(2000);
    }

    [TestMethod]
    public void FakePluginContextBuilder_With_Custom_FakeOrganizationService()
    {
        var fakeOrganizationService = new FakeOrganizationService(new FakeTimeProvider(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero)));
        var service = new FakePluginContextBuilder(fakeOrganizationService).GetOrganizationService();

        service.Should().NotBeNull().And.BeOfType<FakeOrganizationService>();
        service.TimeProvider.GetUtcNow().Year.Should().Be(2000);
    }
}
