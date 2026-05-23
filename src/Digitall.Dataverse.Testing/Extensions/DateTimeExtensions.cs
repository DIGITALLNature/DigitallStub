// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Digitall.Dataverse.Testing.Extensions;

    public static class DateTimeExtensions
    {
        extension(DateTime dateTime)
        {
            public DateTime ToLastDayOfDeltaWeek(int deltaWeek = 0)
                => dateTime.ToFirstDayOfDeltaWeek(deltaWeek).AddDays(6);

            public DateTime ToFirstDayOfDeltaWeek(int deltaWeek = 0)
            {
                var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
                var diff = ((int)dateTime.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
                return dateTime.Date.AddDays(-diff + 7 * deltaWeek);
            }

            public DateTime ToFirstDayOfMonth(int month)
                => dateTime.AddDays(1 - dateTime.Day).AddMonths(month - dateTime.Month);

            public DateTime ToFirstDayOfMonth()
                => dateTime.ToFirstDayOfMonth(dateTime.Month);

            public DateTime ToLastDayOfMonth(int month)
            {
                var addYears = month > 12 ? month % 12 : 0;
                month -= 12 * addYears;
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
