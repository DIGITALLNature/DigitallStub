// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Logic;

public class XrmOrderByAttributeComparer : IComparer<object?>
{
    public int Compare(object? objectA, object? objectB)
    {
        while (true)
        {
            if (objectA == null && objectB == null) return 0;

            if (objectA == null) return -1;
            if (objectB == null) return 1;

            switch (objectA)
            {
                case OptionSetValue optA:
                    return optA.Value.CompareTo(((OptionSetValue)objectB).Value);

                case EntityReference refA:
                {
                    var refB = (EntityReference)objectB;
                    if (refA.Name == null && refB.Name == null) return 0;
                    if (refA.Name == null) return -1;
                    if (refB.Name == null) return 1;
                    return string.Compare(refA.Name, refB.Name, StringComparison.Ordinal);
                }

                case Money moneyA:
                    return moneyA.Value.CompareTo(((Money)objectB).Value);

                case string strA:
                    return string.CompareOrdinal(strA, (string)objectB);

                case int intA:
                    return intA.CompareTo((int)objectB);

                case DateTime dtA:
                    return dtA.CompareTo((DateTime)objectB);

                case Guid guidA:
                    return guidA.CompareTo((Guid)objectB);

                case decimal decA:
                    return decA.CompareTo((decimal)objectB);

                case double dblA:
                    return dblA.CompareTo((double)objectB);

                case float fltA:
                    return fltA.CompareTo((float)objectB);

                case bool boolA:
                    return boolA.CompareTo((bool)objectB);

                case AliasedValue aliasedA:
                    objectA = aliasedA.Value;
                    objectB = (objectB as AliasedValue)?.Value;
                    continue;

                default:
                    return 0;
            }
        }
    }
}
