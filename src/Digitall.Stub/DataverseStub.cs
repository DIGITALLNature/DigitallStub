// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Digitall.Stub.Errors;
using Digitall.Stub.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public class DataverseStub(IStubClock clock) : IOrganizationService
{
    public readonly IStubClock Clock = clock;

    public DataverseStub(): this(new RealTimeClock())
    {
    }

    public List<Assembly> ModelAssemblies { get; set; } = SearchProxyTypesAssembly();

    public Dictionary<string, EntityMetadata> EntityMetadata { get; set; } = new();

    internal Dictionary<string, Dictionary<Guid, Entity>> State { get; } = new();

    internal Dictionary<Type,IOrganizationRequestStub> OrganizationRequestStubs { get; } = new();

    private static List<Assembly> SearchProxyTypesAssembly()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        return assemblies
            .Where(assembly => !assembly.FullName.StartsWith("Microsoft.Xrm.Sdk", StringComparison.Ordinal)) // Ignore SDK
            .Where(assembly => assembly.GetCustomAttributes(typeof(ProxyTypesAssemblyAttribute), true).Length != 0)
            .ToList();
    }

    public void AddStub(IOrganizationRequestStub stub)
    {
        OrganizationRequestStubs.Add(stub.ForType, stub);
    }

    public void AddStubs(params IOrganizationRequestStub[] stubs)
    {
        foreach (var stub in stubs)
        {
            AddStub(stub);
        }
    }

    private void AddStubIfNecessary(IOrganizationRequestStub stub)
    {
        if (!OrganizationRequestStubs.ContainsKey(stub.ForType))
        {
            AddStub(stub);
        }
    }

    #region IQueryable

    public IQueryable<T> CreateQuery<T>() where T : Entity
    {
        var typeParameter = typeof(T);

        var logicalName = "";
        if (typeParameter.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true).Length > 0)
        {
            logicalName =
                (typeParameter.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true)[0] as EntityLogicalNameAttribute).LogicalName;
        }

        if (string.IsNullOrWhiteSpace(logicalName))
        {
            throw new ArgumentException("Entity type must have EntityLogicalNameAttribute", nameof(T));
        }

        return CreateQuery<T>(logicalName);
    }

    public IQueryable<T> CreateQuery<T>(string entityLogicalName) where T : Entity
    {
        var entityStateCopy = new List<T>();
        if (!State.TryGetValue(entityLogicalName, out var entityState))
        {
            return entityStateCopy.AsQueryable(); //Empty list
        }

        foreach (var e in entityState.Values)
        {
            entityStateCopy.Add(e.CloneEntity().ToEntity<T>());
        }

        return entityStateCopy.AsQueryable();
    }

    public IQueryable<Entity> CreateQuery(string entityLogicalName) => CreateQuery<Entity>(entityLogicalName);

    #endregion

    public void Add(Entity entity)
    {
        if (!State.TryGetValue(entity.LogicalName, out var value))
        {
            value = new Dictionary<Guid, Entity>();
            State.Add(entity.LogicalName, value);
        }

        value.Add(entity.Id, entity);
    }

    public void AddRange(IEnumerable<Entity> entities) => entities.ToList().ForEach(Add);

    // <summary>
    ///     Checks if the specified entity type is known.
    ///     An entity type is considered known if it exists in the metadata or if it is an early bound type.
    /// </summary>
    /// <param name="logicalname">The logical name of the entity.</param>
    /// <param name="EntityType">The Type of the entity if it is known, otherwise null.</param>
    /// <returns>True if the entity type is known, otherwise false.</returns>
    public bool EntityTypeIsKnow(string logicalname, out Type EntityType)
    {
        foreach (var modelAssembly in ModelAssemblies)
        {
            var type = modelAssembly
                .GetTypes()
                .Where(t => typeof(Entity).IsAssignableFrom(t))
                .Where(t => t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true).Length > 0)
                .SingleOrDefault(t =>
                    ((EntityLogicalNameAttribute)t.GetCustomAttributes(typeof(EntityLogicalNameAttribute), true)[0])
                    .LogicalName.Equals(logicalname.ToLower()));
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
        if (EntityTypeIsKnow(entity, out var entityType))
        {
            attributeInfo = entityType
                .GetProperties()
                .Where(pi => pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true).Length > 0)
                .FirstOrDefault(pi =>
                    (pi.GetCustomAttributes(typeof(AttributeLogicalNameAttribute), true)[0] as
                        AttributeLogicalNameAttribute).LogicalName.Equals(attribute));
        }

        return attributeInfo != null;
    }

    /// <summary>
    ///     Throws an exception if the specified entity type is not known.
    /// </summary>
    /// <param name="entityType">The entity type to check for knowledge.</param>
    public void ThrowIfNotKnownEntityType(string entityType)
    {
        if (!EntityTypeIsKnow(entityType, out _))
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
            if (!EntityMetadata.TryGetValue(entityLogicalName, out var entityMetadata)
                || entityMetadata.Attributes.All(a => a.LogicalName != attributeLogicalName))
            {
                // Throw a FaultException with a specific message
                ErrorFactory.ThrowFault(ErrorCodes.QueryBuilderNoAttribute, $"The attribute {attributeLogicalName} does not exist on this entity.");
            }
        }
    }

    #region IOrganizationServer

    /// <summary>
    ///     Creates a new entity in the Dataverse.
    ///     If the entity's Id property is Guid.Empty, a new Guid is generated and assigned to the entity's Id property.
    ///     If the entity already exists in the Dataverse, a FaultException is thrown with an error code of ErrorCodes.DuplicateRecord.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The Id of the created entity.</returns>
    /// <exception cref="FaultException">Thrown if the entity already exists in the Dataverse.</exception>
    public Guid Create(Entity entity)
    {
        if (entity == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument,"Required field 'Target' is missing");
        }

        var clone = entity.CloneEntity();
        if (clone.Id == Guid.Empty)
        {
            clone.Id = Guid.NewGuid();
        }

        try
        {
            Add(clone);
        }
        catch (ArgumentException)
        {
            ErrorFactory.ThrowFault(ErrorCodes.DuplicateRecord, $"Cannot insert duplicate key.");
        }

        return clone.Id;
    }

    public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
    {
        if (!State.TryGetValue(entityName, out var value))
        {
            ThrowIfNotKnownEntityType(entityName);
        }

        Entity record = null;
        if (value == null || !value.TryGetValue(id, out record))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entityName}' With Id = {id:D} Does Not Exist");
        }

        return record.ProjectAttributes(columnSet, this).CloneEntity();
    }

    public void Update(Entity entity)
    {
        if (entity == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument,"Required field 'Target' is missing");
        }

        if (!State.TryGetValue(entity.LogicalName, out var value))
        {
            ThrowIfNotKnownEntityType(entity.LogicalName);
        }

        if (value == null || !value.TryGetValue(entity.Id, out var record))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entity.LogicalName}' With Id = {entity.Id:D} Does Not Exist");
        }

        value[entity.Id] = entity.CloneEntity();
    }

    public void Delete(string entityName, Guid id)
    {
        if (entityName == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument,"Required member 'LogicalName' missing for field 'Target'");
        }

        if (!State.TryGetValue(entityName, out var value))
        {
            ThrowIfNotKnownEntityType(entityName);
        }

        if (value == null || !value.TryGetValue(id, out var record))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entityName}' With Id = {id:D} Does Not Exist");
        }

        value.Remove(id);
    }

    public OrganizationResponse Execute(OrganizationRequest request)
    {
        Debug.Assert(request != null, nameof(request) + " != null");

        if (OrganizationRequestStubs.TryGetValue(request.GetType(), out var stub))
        {
            return stub.Execute(request, this);
        }

        throw new ArgumentOutOfRangeException(nameof(request));
    }

    public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) => throw new NotImplementedException();

    public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) => throw new NotImplementedException();

    public EntityCollection RetrieveMultiple(QueryBase query)
    {
        AddStubIfNecessary(new RetrieveMultipleStub());
        return ((RetrieveMultipleResponse)Execute(new RetrieveMultipleRequest{Query = query})).EntityCollection;
    }

    #endregion
}
