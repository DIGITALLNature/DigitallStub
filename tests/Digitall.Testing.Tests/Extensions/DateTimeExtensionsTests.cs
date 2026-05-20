// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Testing.Extensions;

namespace Digitall.Testing.Tests.Extensions;

public class DateTimeExtensionsTests
{
    private static readonly DateTime s_testDate = new(2024, 3, 11); // Monday

    [Test]
    public async Task ToDayOfWeek_Should_ReturnCorrectDate()
    {
        var date = new DateTime(2024, 1, 1);

        var result = date.ToDayOfWeek(2, DayOfWeek.Monday);
        await Assert.That(result).IsEqualTo(new DateTime(2024, 1, 1));

        result = date.ToDayOfWeek(2, DayOfWeek.Wednesday);
        await Assert.That(result).IsEqualTo(new DateTime(2024, 1, 3));
    }

    [Test]
    public async Task ToFirstDayOfMonth_Should_ReturnFirstDayOfCurrentMonth()
    {
        var result = s_testDate.ToFirstDayOfMonth();
        await Assert.That(result).IsEqualTo(new DateTime(2024, 3, 1));
    }

    [Test]
    public async Task ToFirstDayOfMonth_WithSpecificMonth_Should_ReturnFirstDayOfThatMonth()
    {
        var result = s_testDate.ToFirstDayOfMonth(5);
        await Assert.That(result).IsEqualTo(new DateTime(2024, 5, 1));
    }

    [Test]
    public async Task ToLastDayOfMonth_Should_ReturnLastDayOfCurrentMonth()
    {
        var result = s_testDate.ToLastDayOfMonth();
        await Assert.That(result).IsEqualTo(new DateTime(2024, 3, 31));
    }

    [Test]
    public async Task ToLastDayOfMonth_WithSpecificMonth_Should_ReturnLastDayOfThatMonth()
    {
        var result = s_testDate.ToLastDayOfMonth(2); // Feb 2024 is leap year
        await Assert.That(result).IsEqualTo(new DateTime(2024, 2, 29));
    }

    [Test]
    public async Task ToFirstDayOfDeltaWeek_Should_ReturnFirstDayOfNextWeek()
    {
        var thisWeek = s_testDate.ToFirstDayOfDeltaWeek();
        var nextWeek = s_testDate.ToFirstDayOfDeltaWeek(1);

        await Assert.That(nextWeek).IsEqualTo(thisWeek.AddDays(7));
    }

    [Test]
    public async Task ToLastDayOfDeltaWeek_Should_BeSixDaysAfterFirstDay()
    {
        var firstDay = s_testDate.ToFirstDayOfDeltaWeek();
        var lastDay = s_testDate.ToLastDayOfDeltaWeek();

        await Assert.That(lastDay).IsEqualTo(firstDay.AddDays(6));
    }
}
