// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub;
public static class TypeExtensions
{
    public static bool IsOptionSet(this Type t)
    {
        var nullableType = Nullable.GetUnderlyingType(t);
        return t == typeof(OptionSetValue)
               || t.IsEnum
               || nullableType != null && nullableType.IsEnum;
    }


        public static bool IsOptionSetValueCollection(this Type t)
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(OptionSetValueCollection);
        }


    public static bool IsDateTime(this Type t)
    {
        var nullableType = Nullable.GetUnderlyingType(t);
        return t == typeof(DateTime)
               || nullableType != null && nullableType == typeof(DateTime);
    }

    public static bool IsNullableEnum(this Type t)
    {
        return
            t.IsGenericType
            && t.GetGenericTypeDefinition() == typeof(Nullable<>)
            && t.GetGenericArguments()[0].IsEnum;
    }
}
