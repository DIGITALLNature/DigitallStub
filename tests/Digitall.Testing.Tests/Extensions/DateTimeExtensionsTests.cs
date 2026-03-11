// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Globalization;
using AwesomeAssertions;
using Digitall.Testing.Extensions;

namespace Digitall.Testing.Tests.Extensions;

[TestClass]
public class DateTimeExtensionsTests
{
    private static readonly DateTime TestDate = new DateTime(2024, 3, 11); // Monday

    [TestMethod]
    public void ToDayOfWeek_Should_ReturnCorrectDate()
    {
        // 2024-01-01 is Monday.
        // week 1, Monday should be 2024-01-01
        // week 1, Sunday should be 2024-01-07 (if Monday is first day of week)

        var date = new DateTime(2024, 1, 1);

        // Start of year 2024 is Monday.
        // formula: startOfYear.AddDays(7 * (week - 2) + ((dayOfWeek - startOfYear.DayOfWeek + 7) % 7))
        // week 1: 2024-01-01.AddDays(7 * (1 - 2) + (Monday - Monday + 7)%7) = 2024-01-01.AddDays(-7 + 0) = 2023-12-25.
        // Wait, the formula in code is:
        // return startOfYear.AddDays(7 * (week - 2) + ((dayOfWeek - startOfYear.DayOfWeek + 7) % 7));

        // If week = 1 and dayOfWeek = Monday, result is 2023-12-25.
        // If week = 2 and dayOfWeek = Monday, result is 2024-01-01.

        var result = date.ToDayOfWeek(2, DayOfWeek.Monday);
        result.Should().Be(new DateTime(2024, 1, 1));

        result = date.ToDayOfWeek(2, DayOfWeek.Wednesday);
        result.Should().Be(new DateTime(2024, 1, 3));
    }

    [TestMethod]
    public void ToFirstDayOfMonth_Should_ReturnFirstDayOfCurrentMonth()
    {
        var result = TestDate.ToFirstDayOfMonth();
        result.Should().Be(new DateTime(2024, 3, 1));
    }

    [TestMethod]
    public void ToFirstDayOfMonth_WithSpecificMonth_Should_ReturnFirstDayOfThatMonth()
    {
        var result = TestDate.ToFirstDayOfMonth(5);
        result.Should().Be(new DateTime(2024, 5, 1));
    }

    [TestMethod]
    public void ToLastDayOfMonth_Should_ReturnLastDayOfCurrentMonth()
    {
        var result = TestDate.ToLastDayOfMonth();
        result.Should().Be(new DateTime(2024, 3, 31));
    }

    [TestMethod]
    public void ToLastDayOfMonth_WithSpecificMonth_Should_ReturnLastDayOfThatMonth()
    {
        var result = TestDate.ToLastDayOfMonth(2); // Feb 2024 is leap year
        result.Should().Be(new DateTime(2024, 2, 29));
    }

    [TestMethod]
    public void ToFirstDayOfDeltaWeek_Should_ReturnFirstDayOfNextWeek()
    {
        // Culture dependent, but usually Sunday or Monday.
        // We can just verify it's 7 days apart from current week's first day.
        var thisWeek = TestDate.ToFirstDayOfDeltaWeek(0);
        var nextWeek = TestDate.ToFirstDayOfDeltaWeek(1);

        nextWeek.Should().Be(thisWeek.AddDays(7));
    }

    [TestMethod]
    public void ToLastDayOfDeltaWeek_Should_BeSixDaysAfterFirstDay()
    {
        var firstDay = TestDate.ToFirstDayOfDeltaWeek(0);
        var lastDay = TestDate.ToLastDayOfDeltaWeek(0);

        lastDay.Should().Be(firstDay.AddDays(6));
    }
}
