// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Digitall.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Testing;

/// <summary>
/// Resolves entity types and attribute metadata from model assemblies with caching.
/// </summary>
public class EntityTypeResolver
{
    private readonly List<Assembly> _modelAssemblies;
    private readonly Dictionary<string, EntityMetadata> _entityMetadata;

    private Dictionary<string, Type>? _entityTypeCache;
    private Dictionary<string, Dictionary<string, PropertyInfo>>? _attributeCache;

    public EntityTypeResolver(List<Assembly> modelAssemblies, Dictionary<string, EntityMetadata> entityMetadata)
    {
        _modelAssemblies = modelAssemblies;
        _entityMetadata = entityMetadata;
    }

    /// <summary>
    /// Invalidates the internal caches. Call when ModelAssemblies changes.
    /// </summary>
    public void InvalidateCache()
    {
        _entityTypeCache = null;
        _attributeCache = null;
    }

    private Dictionary<string, Type> EntityTypeCache => _entityTypeCache ??= BuildEntityTypeCache();

    private Dictionary<string, Dictionary<string, PropertyInfo>> AttributeCache => _attributeCache ??= BuildAttributeCache();

    private Dictionary<string, Type> BuildEntityTypeCache()
    {
        var cache = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        foreach (var assembly in _modelAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(Entity).IsAssignableFrom(type))
                    continue;

                var attr = type.GetCustomAttribute<EntityLogicalNameAttribute>();
                if (attr != null && !cache.ContainsKey(attr.LogicalName))
                {
                    cache[attr.LogicalName] = type;
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
                if (attr != null && !props.ContainsKey(attr.LogicalName))
                {
                    props[attr.LogicalName] = pi;
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
    /// Checks if the specified attribute is known for the given entity.
    /// </summary>
    public bool IsKnownAttributeForType(string entity, string attribute, out PropertyInfo? attributeInfo)
    {
        attributeInfo = null;
        if (AttributeCache.TryGetValue(entity, out var props))
        {
            return props.TryGetValue(attribute, out attributeInfo);
        }
        return false;
    }

    /// <summary>
    /// Throws an exception if the specified entity type is not known.
    /// </summary>
    public void ThrowIfNotKnownEntityType(string entityType)
    {
        if (!EntityTypeIsKnown(entityType, out _))
        {
            ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoEntity, $"The entity with a name = '{entityType}' with namemapping = 'Logical' was not found in the MetadataCache.");
        }
    }

    /// <summary>
    /// Throws an exception if the specified attribute is not known for the given entity.
    /// </summary>
    public void ThrowIfNotKnownAttribute(string entityLogicalName, string attributeLogicalName)
    {
        if (!IsKnownAttributeForType(entityLogicalName, attributeLogicalName, out _))
        {
            if (!_entityMetadata.TryGetValue(entityLogicalName, out var entityMetadata) || entityMetadata.Attributes.All(a => a.LogicalName != attributeLogicalName))
            {
                ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {attributeLogicalName} does not exist on this entity.");
            }
        }
    }
}
