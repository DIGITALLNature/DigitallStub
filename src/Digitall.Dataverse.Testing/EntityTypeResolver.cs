// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Linq.Expressions;
using System.Reflection;
using Digitall.Dataverse.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing;

/// <summary>
/// Resolves entity types and attribute metadata from model assemblies with caching.
/// </summary>
public class EntityTypeResolver(List<Assembly> modelAssemblies, Dictionary<string, EntityMetadata> entityMetadata)
{
    private Dictionary<string, Type>? _entityTypeCache;
    private Dictionary<string, Dictionary<string, PropertyInfo>>? _attributeCache;
    private Dictionary<Type, string>? _reverseEntityTypeCache;

    /// <summary>
    /// Invalidates the internal caches. Call when ModelAssemblies changes.
    /// </summary>
    // ReSharper disable once UnusedMember.Global : Public API
    public void InvalidateCache()
    {
        _entityTypeCache = null;
        _attributeCache = null;
        _proxyConverterCache = null;
        _reverseEntityTypeCache = null;
    }

    private Dictionary<string, Type> EntityTypeCache => _entityTypeCache ??= BuildEntityTypeCache();

    private Dictionary<string, Dictionary<string, PropertyInfo>> AttributeCache => _attributeCache ??= BuildAttributeCache();

    private Dictionary<Type, string> ReverseEntityTypeCache => _reverseEntityTypeCache ??= BuildReverseEntityTypeCache();

    private Dictionary<Type, string> BuildReverseEntityTypeCache()
    {
        var cache = new Dictionary<Type, string>();
        foreach (var (logicalName, type) in EntityTypeCache)
        {
            cache.TryAdd(type, logicalName);
        }

        return cache;
    }

    private Dictionary<string, Type> BuildEntityTypeCache()
    {
        var cache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        foreach (var assembly in modelAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(Entity).IsAssignableFrom(type)) continue;

                var attr = type.GetCustomAttribute<EntityLogicalNameAttribute>();
                if (attr != null)
                {
                    cache.TryAdd(attr.LogicalName, type);
                }
            }
        }

        return cache;
    }

    private Dictionary<string, Dictionary<string, PropertyInfo>> BuildAttributeCache()
    {
        var cache = new Dictionary<string, Dictionary<string, PropertyInfo>>(StringComparer.OrdinalIgnoreCase);
        foreach (var (logicalName, type) in EntityTypeCache)
        {
            var props = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var pi in type.GetProperties())
            {
                var attr = pi.GetCustomAttribute<AttributeLogicalNameAttribute>();
                if (attr != null)
                {
                    props.TryAdd(attr.LogicalName, pi);
                }
            }

            cache[logicalName] = props;
        }

        return cache;
    }

    /// <summary>
    /// Checks if the specified entity type is known (early bound or in metadata).
    /// </summary>
    public bool EntityTypeIsKnown(string logicalName, out Type? entityType)
    {
        return EntityTypeCache.TryGetValue(logicalName, out entityType);
    }

    /// <summary>
    /// Resolves the logical name for a given entity type. Returns null if not found.
    /// </summary>
    public string? GetLogicalName(Type entityType)
    {
        return ReverseEntityTypeCache.GetValueOrDefault(entityType);
    }

    /// <summary>
    /// Checks if the specified attribute is known for the given entity.
    /// </summary>
    public bool IsKnownAttributeForType(string entity, string attribute, out PropertyInfo? attributeInfo)
    {
        attributeInfo = null;
        return AttributeCache.TryGetValue(entity, out var props) && props.TryGetValue(attribute, out attributeInfo);
    }

    /// <summary>
    /// Throws an exception if the specified entity type is not known.
    /// An entity type is considered known when it either has a registered proxy type or has metadata registered via <see cref="FakeOrganizationService.AddMetadata"/>.
    /// </summary>
    public void ThrowIfNotKnownEntityType(string entityType)
    {
        if (!EntityTypeIsKnown(entityType, out _) && !entityMetadata.ContainsKey(entityType))
        {
            ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoEntity, $"The entity with a name = '{entityType}' with namemapping = 'Logical' was not found in the MetadataCache.");
        }
    }

    /// <summary>
    /// Throws an exception if the specified attribute is not known for the given entity.
    /// </summary>
    public void ThrowIfNotKnownAttribute(string entityLogicalName, string attributeLogicalName)
    {
        if (IsKnownAttributeForType(entityLogicalName, attributeLogicalName, out _)) return;

        if (!entityMetadata.TryGetValue(entityLogicalName, out var metadata) || metadata.Attributes?.All(a => a.LogicalName != attributeLogicalName) != false)
        {
            ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {attributeLogicalName} does not exist on this entity.");
        }
    }

    private Dictionary<string, Func<Entity, Entity>>? _proxyConverterCache;

    private Dictionary<string, Func<Entity, Entity>> ProxyConverterCache => _proxyConverterCache ??= BuildProxyConverterCache();

    private Dictionary<string, Func<Entity, Entity>> BuildProxyConverterCache()
    {
        var cache = new Dictionary<string, Func<Entity, Entity>>(StringComparer.OrdinalIgnoreCase);
        var toEntityMethod = typeof(Entity).GetMethod(nameof(Entity.ToEntity))!;
        var param = Expression.Parameter(typeof(Entity), "entity");

        foreach (var (logicalName, type) in EntityTypeCache)
        {
            if (type == typeof(Entity)) continue;

            var call = Expression.Call(param, toEntityMethod.MakeGenericMethod(type));
            var cast = Expression.Convert(call, typeof(Entity));
            cache[logicalName] = Expression.Lambda<Func<Entity, Entity>>(cast, param).Compile();
        }

        return cache;
    }

    /// <summary>
    /// Converts a plain Entity to its registered proxy type (if known).
    /// Uses compiled delegates — no per-call reflection overhead.
    /// </summary>
    public Entity ConvertToProxyType(Entity entity)
    {
        return ProxyConverterCache.TryGetValue(entity.LogicalName, out var converter)
            ? converter(entity)
            : entity;
    }
}
