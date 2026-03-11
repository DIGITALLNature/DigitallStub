// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Extensions;

public static class TypeExtensions
{
    extension(Type t)
    {
        public bool IsOptionSet()
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(OptionSetValue) || t.IsEnum || nullableType is { IsEnum: true };
        }

        public bool IsOptionSetValueCollection()
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(OptionSetValueCollection);
        }

        public bool IsDateTime()
        {
            var nullableType = Nullable.GetUnderlyingType(t);
            return t == typeof(DateTime) || nullableType != null && nullableType == typeof(DateTime);
        }

        public bool IsNullableEnum()
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum;
        }
    }
}
