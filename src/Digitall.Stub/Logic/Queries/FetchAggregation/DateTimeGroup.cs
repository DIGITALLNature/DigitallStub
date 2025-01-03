// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Digitall.Stub.Logic.Queries.FetchAggregation;

class DateTimeGroup : FetchGrouping
{
    public DateGroupType Type { get; set; }

    public override IComparable FindGroupValue(object attributeValue)
    {
        if (attributeValue == null) return null;

        if (!(attributeValue is DateTime || attributeValue is DateTime?))
        {
            throw new Exception("Can only do date grouping of DateTime values");
        }

        var d = attributeValue as DateTime?;

        switch (Type)
        {
            case DateGroupType.DateTime:
                return d;

            case DateGroupType.Day:
                return d?.Day;

            case DateGroupType.Week:
                var cal = System.Globalization.DateTimeFormatInfo.InvariantInfo;
                return cal.Calendar.GetWeekOfYear(d.Value, cal.CalendarWeekRule, cal.FirstDayOfWeek);

            case DateGroupType.Month:
                return d?.Month;

            case DateGroupType.Quarter:
                return (d?.Month + 2) / 3;

            case DateGroupType.Year:
                return d?.Year;

            default:
                throw new Exception("Unhandled date group type");
        }
    }
}








