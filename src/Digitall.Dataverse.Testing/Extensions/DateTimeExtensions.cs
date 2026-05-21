// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Digitall.Dataverse.Testing.Extensions;

    public static class DateTimeExtensions
    {
        extension(DateTime dateTime)
        {
            public DateTime ToDayOfWeek(int week, DayOfWeek dayOfWeek)
            {
                DateTime startOfYear = dateTime.AddDays(1 - dateTime.DayOfYear);
                return startOfYear.AddDays(7 * (week - 2) + (dayOfWeek - startOfYear.DayOfWeek + 7) % 7);
            }

            // ReSharper disable once MemberCanBePrivate.Global : Public API
            public DateTime ToDayOfDeltaWeek(int deltaWeek, DayOfWeek dayOfWeek)
                => dateTime.ToDayOfWeek(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime
                    , CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule
                    , CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek) + deltaWeek, dayOfWeek);

            public DateTime ToLastDayOfDeltaWeek(int deltaWeek = 0)
                => dateTime.ToDayOfDeltaWeek(deltaWeek, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek).AddDays(6);

            public DateTime ToFirstDayOfDeltaWeek(int deltaWeek = 0)
                => dateTime.ToDayOfDeltaWeek(deltaWeek, CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);

            public DateTime ToFirstDayOfMonth(int month)
                => dateTime.AddDays(1 - dateTime.Day).AddMonths(month - dateTime.Month);

            public DateTime ToFirstDayOfMonth()
                => dateTime.ToFirstDayOfMonth(dateTime.Month);

            public DateTime ToLastDayOfMonth(int month)
            {
                var addYears = month > 12 ? month % 12 : 0;
                month = month - 12 * addYears;
                return dateTime
                    .AddDays(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTime.Year + addYears, month) - dateTime.Day)
                    .AddMonths(month - dateTime.Month).AddYears(addYears);
            }

            public DateTime ToLastDayOfMonth()
                => dateTime.ToLastDayOfMonth(dateTime.Month);
        }
    }

    // ReSharper disable once UnusedType.Global
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public enum FiscalPeriod
    {
        Annually = 2000,
        SemiAnnually = 2001,
        Quarterly = 2002,
        Monthly = 2003,
        FourWeek = 2004
    }
