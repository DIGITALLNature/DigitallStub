// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.


namespace Digitall.Testing.Logic.Queries.FetchAggregation;

class DateTimeGroup : FetchGrouping
{
    public DateGroupType Type { get; init; }

    protected override IComparable? FindGroupValue(object? attributeValue)
    {
        if (attributeValue is not DateTime d)
        {
            if (attributeValue == null) return null;
            throw new Exception("Can only do date grouping of DateTime values");
        }

        return Type switch
        {
            DateGroupType.DateTime => d,
            DateGroupType.Day => d.Day,
            DateGroupType.Week => System.Globalization.DateTimeFormatInfo.InvariantInfo.Calendar
                .GetWeekOfYear(d, System.Globalization.DateTimeFormatInfo.InvariantInfo.CalendarWeekRule,
                    System.Globalization.DateTimeFormatInfo.InvariantInfo.FirstDayOfWeek),
            DateGroupType.Month => d.Month,
            DateGroupType.Quarter => (d.Month + 2) / 3,
            DateGroupType.Year => d.Year,
            _ => throw new Exception("Unhandled date group type")
        };
    }
}








