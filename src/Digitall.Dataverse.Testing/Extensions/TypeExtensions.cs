// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Extensions;

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
            return t == typeof(OptionSetValueCollection);
        }

        public bool IsDateTime()
        {
            return t == typeof(DateTime) || Nullable.GetUnderlyingType(t) == typeof(DateTime);
        }

        public bool IsNullableEnum()
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && t.GetGenericArguments()[0].IsEnum;
        }
    }
}
