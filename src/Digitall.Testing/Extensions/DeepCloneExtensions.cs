// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics.CodeAnalysis;

namespace Digitall.Testing.Extensions;

internal static class DeepCloneExtensions
{
    [return: NotNullIfNotNull(nameof(obj))]
    public static T? DeepClone<T>(this T? obj)
    {
        return FastCloner.FastCloner.DeepClone(obj);
    }
}
