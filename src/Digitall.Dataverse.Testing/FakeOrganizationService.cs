// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Reflection;
using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Extensions;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing;

public class FakeOrganizationService(TimeProvider timeProvider, FakeOrganizationServiceState state) : IOrganizationService
{
    public TimeProvider TimeProvider { get; } = timeProvider;
    public FakeDataverseOptions Options { get; } = new();

    public FakeOrganizationServiceState State { get; } = state;

    // ReSharper disable once UnusedMember.Global : Public API
    public FakeOrganizationService(FakeOrganizationServiceState state) : this(TimeProvider.System, state)
    {
    }

    public FakeOrganizationService(TimeProvider timeProvider) : this(timeProvider, new FakeOrganizationServiceState())
    {
    }

    public FakeOrganizationService() : this(TimeProvider.System)
    {
    }

    private Dictionary<string, Dictionary<Guid, Entity>> ServiceState => State.Entities;

    private EntityTypeResolver? _typeResolver;
    private EntityTypeResolver TypeResolver => _typeResolver ??= new EntityTypeResolver(State.ModelAssemblies, State.EntityMetadata);

    /// <summary>
    /// Invalidates the type resolver cache. Call after modifying ModelAssemblies or EntityMetadata.
    /// </summary>
    // ReSharper disable once UnusedMember.Global : Public API
    public void InvalidateTypeResolverCache()
    {
        _typeResolver = null;
    }

    private Dictionary<Type, IOrganizationRequestFake> OrganizationRequestFakes { get; } = new();

    /// <summary>
    /// Checks if a record exists in the internal state without throwing.
    /// </summary>
    internal bool EntityExists(string logicalName, Guid id) => ServiceState.TryGetValue(logicalName, out var entities) && entities.ContainsKey(id);

    public void AddRequest(IOrganizationRequestFake fake)
    {
        OrganizationRequestFakes.Add(fake.ForType, fake);
    }

    public void AddRequests(IEnumerable<IOrganizationRequestFake> requests)
    {
        foreach (var request in requests)
        {
            AddRequest(request);
        }
    }

    public void AddRequests(params IOrganizationRequestFake[] requests)
    {
        foreach (var request in requests)
        {
            AddRequest(request);
        }
    }

    public void AddMetadata(EntityMetadata entityMetadata)
    {
        State.EntityMetadata.Add(entityMetadata.LogicalName, entityMetadata);

        var relationships = new List<RelationshipMetadataBase>();
        if (entityMetadata.ManyToManyRelationships != null)
        {
            relationships.AddRange(entityMetadata.ManyToManyRelationships);
        }

        if (entityMetadata.OneToManyRelationships != null)
        {
            relationships.AddRange(entityMetadata.OneToManyRelationships);
        }

        if (entityMetadata.ManyToOneRelationships != null)
        {
            relationships.AddRange(entityMetadata.ManyToOneRelationships);
        }

        AddRelationships(relationships);
    }

    public void AddMetadata(IEnumerable<EntityMetadata> entityMetadata)
    {
        foreach (var metadata in entityMetadata)
        {
            AddMetadata(metadata);
        }
    }

    public void AddMetadata(params EntityMetadata[] entityMetadata)
    {
        foreach (var metadata in entityMetadata)
        {
            AddMetadata(metadata);
        }
    }

    public void AddRelationship(RelationshipMetadataBase relationship)
    {
        State.Relationships[relationship.SchemaName] = relationship;
    }

    public void AddRelationships(IEnumerable<RelationshipMetadataBase> relationships)
    {
        foreach (var relationship in relationships)
        {
            AddRelationship(relationship);
        }
    }

    public void AddRelationships(params RelationshipMetadataBase[] relationships)
    {
        foreach (var relationship in relationships)
        {
            AddRelationship(relationship);
        }
    }

    public void AddDefaultRequests()
    {
        Assembly a = typeof(IOrganizationRequestFake).Assembly;
        var requests = a.GetTypes().Where(type =>
            type.IsClass && type is { IsAbstract: false, Namespace: "Digitall.Dataverse.Testing.OrganizationRequests" } && typeof(IOrganizationRequestFake).IsAssignableFrom(type)).ToList();

        foreach (var request in requests)
        {
            if (Activator.CreateInstance(request) is IOrganizationRequestFake fake)
            {
                AddRequestIfNecessary(fake);
            }
        }
    }

    private void AddRequestIfNecessary(IOrganizationRequestFake fake)
    {
        if (!OrganizationRequestFakes.ContainsKey(fake.ForType))
        {
            AddRequest(fake);
        }
    }

    #region IQueryable

    public IQueryable<T> CreateQuery<T>() where T : Entity
    {
        var logicalName = TypeResolver.GetLogicalName(typeof(T))
                          ?? typeof(T).GetCustomAttribute<EntityLogicalNameAttribute>()?.LogicalName
                          ?? throw new ArgumentException(
                              $"Entity type '{typeof(T).Name}' could not be resolved to a logical name. " +
                              "Ensure the type is annotated with [EntityLogicalName] and its assembly is registered " +
                              "as a ProxyTypesAssembly or added to ModelAssemblies.", nameof(T));

        return CreateQuery<T>(logicalName);
    }

    public IQueryable<T> CreateQuery<T>(string entityLogicalName) where T : Entity
    {
        var entityStateCopy = new List<T>();
        if (!ServiceState.TryGetValue(entityLogicalName, out var entityState))
        {
            return entityStateCopy.AsQueryable(); //Empty list
        }

        var primaryIdAttribute = GetPrimaryIdAttribute(entityLogicalName);

        entityStateCopy.AddRange(entityState.Values.Select(e =>
        {
            var clone = e.CloneEntity();
            EnsurePrimaryIdAttribute(clone, primaryIdAttribute);
            return typeof(T) == typeof(Entity) ? (T)clone : clone.ToEntity<T>();
        }));

        return entityStateCopy.AsQueryable();
    }

    /// <summary>
    ///     Determines the primary id attribute name for the given entity.
    ///     Uses <see cref="EntityMetadata.PrimaryIdAttribute"/> when available,
    ///     otherwise falls back to the Dataverse convention <c>&lt;entityLogicalName&gt;id</c>.
    /// </summary>
    private string GetPrimaryIdAttribute(string entityLogicalName)
    {
        if (State.EntityMetadata.TryGetValue(entityLogicalName, out var metadata) && !string.IsNullOrWhiteSpace(metadata.PrimaryIdAttribute))
        {
            return metadata.PrimaryIdAttribute!;
        }

        return entityLogicalName + "id";
    }

    /// <summary>
    ///     Ensures that the primary id attribute is present in the entity's attribute collection.
    ///     This allows filters (e.g. <c>systemuserid == &lt;guid&gt;</c>) to match against <see cref="Entity.Id"/>
    ///     even when the attribute was not explicitly set on the stored entity.
    /// </summary>
    private static void EnsurePrimaryIdAttribute(Entity entity, string primaryIdAttribute)
    {
        if (entity.Id == Guid.Empty) return;
        if (entity.Attributes.ContainsKey(primaryIdAttribute)) return;

        entity[primaryIdAttribute] = entity.Id;
    }

    public IQueryable<Entity> CreateQuery(string entityLogicalName) => CreateQuery<Entity>(entityLogicalName);

    #endregion

    public void Add(Entity entity)
    {
        if (!ServiceState.TryGetValue(entity.LogicalName, out var value))
        {
            value = new Dictionary<Guid, Entity>();
            ServiceState.Add(entity.LogicalName, value);
        }

        foreach (var entityRef in entity.Attributes.Values.OfType<EntityReference>().Where(er => er.KeyAttributes?.Count > 0))
        {
            if (!ServiceState.TryGetValue(entityRef.LogicalName, out var refState)) continue;

            var match = refState.Values.SingleOrDefault(e => entityRef.KeyAttributes.All(k => e.Contains(k.Key) && e[k.Key].Equals(k.Value)));

            if (match is null) continue;

            entityRef.KeyAttributes = [];
            entityRef.Id = match.Id;
        }

        value.Add(entity.Id, entity);
    }

    public void AddRange(IEnumerable<Entity> entities) => entities.ToList().ForEach(Add);

    private RelationshipMetadataBase? GetRelationship(string relationshipSchemaName)
    {
        return State.Relationships.GetValueOrDefault(relationshipSchemaName);
    }

    /// <summary>
    ///     Checks if the specified entity type is known.
    ///     An entity type is considered known if it exists in the metadata or if it is an early bound type.
    /// </summary>
    /// <param name="logicalname">The logical name of the entity.</param>
    /// <param name="entityType">The Type of the entity if it is known, otherwise null.</param>
    /// <returns>True if the entity type is known, otherwise false.</returns>
    public bool EntityTypeIsKnown(string logicalname, out Type? entityType) => TypeResolver.EntityTypeIsKnown(logicalname, out entityType);

    /// <summary>
    ///     Checks if the specified attribute is known for the given entity.
    /// </summary>
    public bool IsKnownAttributeForType(string entity, string attribute, out PropertyInfo? attributeInfo) => TypeResolver.IsKnownAttributeForType(entity, attribute, out attributeInfo);

    /// <summary>
    ///     Throws an exception if the specified entity type is not known.
    /// </summary>
    public void ThrowIfNotKnownEntityType(string entityType) => TypeResolver.ThrowIfNotKnownEntityType(entityType);

    /// <summary>
    ///     Throws an exception if the specified attribute is not known for the given entity.
    /// </summary>
    public void ThrowIfNotKnownAttribute(string entityLogicalName, string attributeLogicalName) => TypeResolver.ThrowIfNotKnownAttribute(entityLogicalName, attributeLogicalName);

    /// <summary>
    ///     Converts a plain Entity to its registered proxy type using cached delegates.
    /// </summary>
    public Entity ConvertToProxyType(Entity entity) => TypeResolver.ConvertToProxyType(entity);

    #region IOrganizationService

    /// <summary>
    ///     Creates a new entity in the Dataverse.
    ///     If the entity's Id property is Guid.Empty, a new Guid is generated and assigned to the entity's Id property.
    ///     If the entity already exists in the Dataverse, a FaultException is thrown with an error code of ErrorCodes.DuplicateRecord.
    /// </summary>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The Id of the created entity.</returns>
    /// <exception cref="FaultException">Thrown if the entity already exists in the Dataverse.</exception>
    public Guid Create(Entity? entity)
    {
        if (entity == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "Required field 'Target' is missing");
        }

        var clone = entity.CloneEntity();
        if (clone.Id == Guid.Empty)
        {
            clone.Id = Guid.NewGuid();
        }

        var defaultStateCode = State.GetDefaultStateCode(clone.LogicalName);
        if (!clone.Contains("statecode")) clone["statecode"] = defaultStateCode;
        // Resolve statuscode based on the actual statecode (passed or defaulted)
        if (!clone.Contains("statuscode"))
        {
            var resolvedStatecode = clone.GetAttributeValue<OptionSetValue>("statecode");
            clone["statuscode"] = State.GetDefaultStatusCode(clone.LogicalName, resolvedStatecode.Value);
        }

        // Default ownerid for user-owned entities
        if (!clone.Contains("ownerid"))
        {
            var isOrganizationOwned = State.EntityMetadata.TryGetValue(clone.LogicalName, out var metadata)
                                      && metadata.OwnershipType == OwnershipTypes.OrganizationOwned;

            if (!isOrganizationOwned && Options.UserId != Guid.Empty)
            {
                clone["ownerid"] = new EntityReference("systemuser", Options.UserId);
            }
        }

        try
        {
            Add(clone);
        }
        catch (ArgumentException)
        {
            ErrorFactory.ThrowFault(ErrorCodes.DuplicateRecord, "Cannot insert duplicate key.");
        }

        return clone.Id;
    }

    public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet)
    {
        if (!ServiceState.TryGetValue(entityName, out var value))
        {
            ThrowIfNotKnownEntityType(entityName);
        }

        Entity? record = null;
        if (value == null || !value.TryGetValue(id, out record))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entityName}' With Id = {id:D} Does Not Exist");
        }

        return record.ProjectAttributes(columnSet, this).CloneEntity();
    }

    public void Update(Entity? entity)
    {
        if (entity == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "Required field 'Target' is missing");
        }

        if (!ServiceState.TryGetValue(entity.LogicalName, out var value))
        {
            ThrowIfNotKnownEntityType(entity.LogicalName);
        }

        if (value == null || !value.TryGetValue(entity.Id, out _))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entity.LogicalName}' With Id = {entity.Id:D} Does Not Exist");
        }

        var merged = value[entity.Id].CloneEntity();

        foreach (var attr in entity.Attributes)
            merged.Attributes[attr.Key] = attr.Value;

        foreach (var fv in entity.FormattedValues)
            merged.FormattedValues[fv.Key] = fv.Value;

        foreach (var ka in entity.KeyAttributes)
            merged.KeyAttributes[ka.Key] = ka.Value;

        value[entity.Id] = merged;
    }

    public void Delete(string? entityName, Guid id)
    {
        if (entityName == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "Required member 'LogicalName' missing for field 'Target'");
        }

        if (!ServiceState.TryGetValue(entityName, out var value))
        {
            ThrowIfNotKnownEntityType(entityName);
        }

        if (value == null || !value.TryGetValue(id, out _))
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entityName}' With Id = {id:D} Does Not Exist");
        }

        value.Remove(id);
    }

    public OrganizationResponse Execute(OrganizationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return OrganizationRequestFakes.TryGetValue(request.GetType(), out var fake)
            ? fake.Execute(request, this)
            : throw new ArgumentOutOfRangeException(nameof(request), $"No implementation found for request of type {request.GetType().Name}");
    }

    public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        var relationshipMetadata = GetRelationship(relationship.SchemaName);

        if (relationshipMetadata == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, $"Relationship {relationship.SchemaName} does not exist in the metadata cache");
        }


        foreach (var relatedEntityReference in relatedEntities)
        {
            switch (relationshipMetadata)
            {
                case ManyToManyRelationshipMetadata manyToManyRelationshipMetadata:
                    {
                        var isFrom1To2 = entityName == manyToManyRelationshipMetadata.Entity1LogicalName;
                        var fromAttribute = isFrom1To2 ? manyToManyRelationshipMetadata.Entity1IntersectAttribute : manyToManyRelationshipMetadata.Entity2IntersectAttribute;
                        var toAttribute = isFrom1To2 ? manyToManyRelationshipMetadata.Entity2IntersectAttribute : manyToManyRelationshipMetadata.Entity1IntersectAttribute;
                        var fromEntityName = isFrom1To2 ? manyToManyRelationshipMetadata.Entity1LogicalName : manyToManyRelationshipMetadata.Entity2LogicalName;
                        var toEntityName = isFrom1To2 ? manyToManyRelationshipMetadata.Entity2LogicalName : manyToManyRelationshipMetadata.Entity1LogicalName;

                        //Check records exist
                        var targetExists = CreateQuery(fromEntityName).FirstOrDefault(e => e.Id == entityId) != null;

                        if (!targetExists)
                        {
                            throw new Exception($"{fromEntityName} with Id {entityId.ToString()} doesn't exist");
                        }

                        var relatedExists = CreateQuery(toEntityName).FirstOrDefault(e => e.Id == relatedEntityReference.Id) != null;

                        if (!relatedExists)
                        {
                            throw new Exception($"{toEntityName} with Id {relatedEntityReference.Id.ToString()} doesn't exist");
                        }

                        var association = new Entity(manyToManyRelationshipMetadata.IntersectEntityName)
                        {
                            Attributes = new AttributeCollection
                            {
                                { $"{manyToManyRelationshipMetadata.IntersectEntityName}id", Guid.NewGuid() }, { fromAttribute, entityId }, { toAttribute, relatedEntityReference.Id }
                            }
                        };

                        Create(association);
                        break;
                    }
                case OneToManyRelationshipMetadata oneToManyRelationshipMetadata:
                    {
                        //Get entity to update
                        var entityToUpdate = new Entity(relatedEntityReference.LogicalName)
                        {
                            Id = relatedEntityReference.Id, [oneToManyRelationshipMetadata.ReferencingAttribute] = new EntityReference(entityName, entityId)
                        };

                        Update(entityToUpdate);
                        break;
                    }
                default:
                    throw new ArgumentException("RelationShip Metadata is not typed correctly");
            }
        }
    }


    public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        var relationshipMetadata = GetRelationship(relationship.SchemaName);

        if (relationshipMetadata == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, $"Relationship {relationship.SchemaName} does not exist in the metadata cache");
        }


        foreach (var relatedEntity in relatedEntities)
        {
            if (relationshipMetadata is ManyToManyRelationshipMetadata manyToManyRelationshipMetadata)
            {
                var isFrom1To2 = entityName == manyToManyRelationshipMetadata.Entity1LogicalName;
                var fromAttribute = isFrom1To2 ? manyToManyRelationshipMetadata.Entity1IntersectAttribute : manyToManyRelationshipMetadata.Entity2IntersectAttribute;
                var toAttribute = isFrom1To2 ? manyToManyRelationshipMetadata.Entity2IntersectAttribute : manyToManyRelationshipMetadata.Entity1IntersectAttribute;

                var query = new QueryExpression(manyToManyRelationshipMetadata.IntersectEntityName) { ColumnSet = new ColumnSet(true), Criteria = new FilterExpression(LogicalOperator.And) };

                query.Criteria.AddCondition(new ConditionExpression(fromAttribute, ConditionOperator.Equal, entityId));
                query.Criteria.AddCondition(new ConditionExpression(toAttribute, ConditionOperator.Equal, relatedEntity.Id));

                var results = RetrieveMultiple(query);

                if (results.Entities.Count == 1)
                {
                    Delete(manyToManyRelationshipMetadata.IntersectEntityName, results.Entities.First().Id);
                }
            }
            else
            {
                throw new ArgumentException("RelationShip Metadata is not ManyToManyRelationshipMetadata");
            }
        }
    }

    public EntityCollection RetrieveMultiple(QueryBase query)
    {
        AddRequestIfNecessary(new RetrieveMultipleFake());
        return ((RetrieveMultipleResponse)Execute(new RetrieveMultipleRequest { Query = query })).EntityCollection;
    }

    #endregion

    public Entity RetrieveWithAlternateKey(string entityName, KeyAttributeCollection keys, ColumnSet columnSet)
    {
        if (!ServiceState.TryGetValue(entityName, out var value))
        {
            ThrowIfNotKnownEntityType(entityName);
        }

        var record = value?.Values.SingleOrDefault(row => keys.All(key => row.Attributes.ContainsKey(key.Key) && row.Attributes[key.Key] != null && row.Attributes[key.Key].Equals(key.Value)));
        if (record == null)
        {
            ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, $"Entity '{entityName}' With Key = {string.Join(",", keys.Keys)} Does Not Exist");
        }

        return record.ProjectAttributes(columnSet, this).CloneEntity();
    }
}
