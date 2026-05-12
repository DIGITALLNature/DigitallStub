// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Digitall.Dataverse.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing;

public class EntityTypeResolver
{
    private readonly List<Assembly> _modelAssemblies;
    private readonly Dictionary<string, EntityMetadata> _entityMetadata;

    public EntityTypeResolver(List<Assembly> modelAssemblies, Dictionary<string, EntityMetadata> entityMetadata)
    {
        _modelAssemblies = modelAssemblies;
        _entityMetadata = entityMetadata;
    }

    /// <summary>
    ///     Checks if the specified entity type is known.
    ///     An entity type is considered known if it exists in the metadata or if it is an early bound type.
    /// </summary>
    /// <param name="logicalname">The logical name of the entity.</param>
    /// <param name="EntityType">The Type of the entity if it is known, otherwise null.</param>
    /// <returns>True if the entity type is known, otherwise false.</returns>
    public bool EntityTypeIsKnown(string logicalname, out Type EntityType)
    {
        foreach (var modelAssembly in _modelAssemblies)
        {
            var type = modelAssembly.GetTypes().Where(t => typeof(Entity).IsAssignableFrom(t)).Where(t => t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true).Length > 0)
                .SingleOrDefault(t => ((EntityLogicalNameAttribute)t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true)[0]).LogicalName.Equals(logicalname.ToLower()));
            if (type != null)
            {
                EntityType = type;
                return true;
            }
        }

        EntityType = null;
        return false;
    }

    /// <summary>
    ///     Checks if the specified attribute is known for the given entity.
    ///     An attribute is considered known if it exists in the entity's metadata or if it is a known attribute for the entity's early bound type.
    /// </summary>
    /// <param name="entity">The logical name of the entity.</param>
    /// <param name="attribute">The logical name of the attribute.</param>
    /// <param name="attributeInfo">The PropertyInfo of the attribute if it is known, otherwise null.</param>
    /// <returns>True if the attribute is known, otherwise false.</returns>
    public bool IsKnownAttributeForType(string entity, string attribute, out PropertyInfo attributeInfo)
    {
        attributeInfo = null;
        if (EntityTypeIsKnown(entity, out var entityType))
        {
            attributeInfo = entityType.GetProperties().Where(pi => pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true).Length > 0).FirstOrDefault(pi =>
                (pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true)[0] as AttributeLogicalNameAttribute).LogicalName.Equals(attribute));
        }

        return attributeInfo != null;
    }

    /// <summary>
    ///     Throws an exception if the specified entity type is not known.
    /// </summary>
    /// <param name="entityType">The entity type to check for knowledge.</param>
    public void ThrowIfNotKnownEntityType(string entityType)
    {
        if (!EntityTypeIsKnown(entityType, out _))
        {
            ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoEntity, $"The entity with a name = '{entityType}' with namemapping = 'Logical' was not found in the MetadataCache.");
        }
    }

    /// <summary>
    ///     Throws an exception if the specified attribute is not known for the given entity.
    ///     An attribute is considered known if it exists in the entity's metadata or if it is a known attribute for the entity's early bound type.
    /// </summary>
    /// <param name="entityLogicalName">The logical name of the entity.</param>
    /// <param name="attributeLogicalName">The logical name of the attribute.</param>
    public void ThrowIfNotKnownAttribute(string entityLogicalName, string attributeLogicalName)
    {
        // Check if the attribute is known for the entity
        if (!IsKnownAttributeForType(entityLogicalName, attributeLogicalName, out _))
        {
            // Check if the attribute exists in the entity's metadata
            if (!_entityMetadata.TryGetValue(entityLogicalName, out var entityMetadata) || entityMetadata.Attributes.All(a => a.LogicalName != attributeLogicalName))
            {
                // Throw a FaultException with a specific message
                ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {attributeLogicalName} does not exist on this entity.");
            }
        }
    }
}
