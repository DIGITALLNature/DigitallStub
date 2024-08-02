// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Logic;

public class XrmOrderByAttributeComparer : IComparer<object>
{
    public int Compare(Object objectA, Object objectB)
    {
        if (objectA == null && objectB == null) return 0;  //Equal

        if (objectA == null)
            return -1;
        if (objectB == null)
            return 1;

        Type attributeType = objectA.GetType();

        if (attributeType == typeof(OptionSetValue))
        {
            // we'll want the text value
            OptionSetValue attributeValueA = (OptionSetValue)(objectA);
            OptionSetValue attributeValueB = (OptionSetValue)(objectB);
            return attributeValueA.Value.CompareTo(attributeValueB.Value);
        }

        if (attributeType == typeof(EntityReference))
        {
            // Name might well be Null in an entity reference?
            EntityReference entityRefA = (EntityReference)objectA;
            EntityReference entityRefB = (EntityReference)objectB;

            if (entityRefA.Name == null && entityRefB.Name == null) return 0;  //Equal

            if (entityRefA.Name == null)
                return -1;
            if (entityRefB.Name == null)
                return 1;

            return entityRefA.Name.CompareTo(entityRefB.Name);
        }

        if (attributeType == typeof(Money))
        {
            var valueA = ((Money)objectA).Value;
            var valueB = ((Money)objectB).Value;
            var x = valueA.CompareTo(valueB);
            return x;
        }

        if (attributeType == typeof(string))
        {
            return String.Compare(objectA.ToString(), objectB.ToString());
        }

        if (attributeType == typeof(int))
        {
            return ((int)objectA).CompareTo(((int)objectB));
        }

        if (attributeType == typeof(DateTime))
        {
            return ((DateTime)objectA).CompareTo((DateTime)objectB);
        }

        if (attributeType == typeof(Guid))
        {
            return ((Guid)objectA).CompareTo((Guid)objectB);
        }

        if (attributeType == typeof(decimal))
        {
            return ((decimal)objectA).CompareTo((decimal)objectB);
        }

        if (attributeType == typeof(double))
        {
            return ((double)objectA).CompareTo((double)objectB);
        }

        if (attributeType == typeof(float))
        {
            return ((float)objectA).CompareTo((float)objectB);
        }

        if (attributeType == typeof(bool))
        {
            return ((bool)objectA).CompareTo((bool)objectB);
        }

        if (attributeType == typeof(AliasedValue))
        {
            return Compare((objectA as AliasedValue)?.Value, (objectB as AliasedValue)?.Value);
        }

        return 0;
    }
}
