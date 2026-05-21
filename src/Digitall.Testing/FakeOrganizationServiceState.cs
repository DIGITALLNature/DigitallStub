// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Testing;

public class FakeOrganizationServiceState
{
    public List<Assembly> ModelAssemblies { get; } = SearchProxyTypesAssembly();

    public Dictionary<string, EntityMetadata> EntityMetadata { get; } = new();

    public Dictionary<string, RelationshipMetadataBase> Relationships { get; } = new();

    internal Dictionary<string, Dictionary<Guid, Entity>> Entities { get; } = new(); // statt "State"

    private static List<Assembly> SearchProxyTypesAssembly()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies.Where(assembly => !assembly.FullName!.StartsWith("Microsoft.Xrm.Sdk", StringComparison.Ordinal)) // Ignore SDK
            .Where(assembly => assembly.GetCustomAttributes(typeof(ProxyTypesAssemblyAttribute), true).Length != 0).ToList();
    }
}
