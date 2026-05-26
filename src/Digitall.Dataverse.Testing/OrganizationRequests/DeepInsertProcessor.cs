// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

/// <summary>
///     Processes <see cref="Entity.RelatedEntities"/> during Create/Upsert (deep insert).
///     Sub-entities are always created (never updated). Relationship metadata must be registered.
/// </summary>
internal static class DeepInsertProcessor
{
    /// <summary>
    ///     Creates all sub-entities from <paramref name="relatedEntities"/> and links them
    ///     to the parent via the relationship metadata. Processes recursively.
    /// </summary>
    /// <param name="parentLogicalName">Logical name of the parent entity.</param>
    /// <param name="parentId">Id of the already-created parent entity.</param>
    /// <param name="relatedEntities">The RelatedEntities dictionary from the original entity.</param>
    /// <param name="state">The fake organization service instance.</param>
    internal static void Process(
        string parentLogicalName,
        Guid parentId,
        RelatedEntityCollection relatedEntities,
        FakeOrganizationService state)
    {
        if (relatedEntities.Count == 0)
        {
            return;
        }

        foreach (var (relationship, children) in relatedEntities)
        {
            var relationshipMetadata = state.GetRelationship(relationship.SchemaName);

            if (relationshipMetadata == null)
            {
                ErrorFactory.ThrowFault(
                    ErrorCodes.InvalidArgument,
                    $"Relationship '{relationship.SchemaName}' does not exist in the metadata cache. " +
                    "Register it via State.Relationships before using deep insert.");
            }

            switch (relationshipMetadata)
            {
                case OneToManyRelationshipMetadata oneToMany:
                    ProcessOneToMany(parentLogicalName, parentId, oneToMany, children, state);
                    break;
                case ManyToManyRelationshipMetadata manyToMany:
                    ProcessManyToMany(parentLogicalName, parentId, manyToMany, children, state);
                    break;
                default:
                    ErrorFactory.ThrowFault(
                        ErrorCodes.InvalidArgument,
                        $"Relationship metadata type '{relationshipMetadata!.GetType().Name}' is not supported for deep insert");
                    break;
            }
        }
    }

    private static void ProcessOneToMany(
        string parentLogicalName,
        Guid parentId,
        OneToManyRelationshipMetadata metadata,
        EntityCollection children,
        FakeOrganizationService state)
    {
        if (metadata.ReferencedEntity != parentLogicalName)
        {
            ErrorFactory.ThrowFault(
                ErrorCodes.InvalidArgument,
                $"Relationship '{metadata.SchemaName}' references entity '{metadata.ReferencedEntity}' " +
                $"but the parent entity is '{parentLogicalName}'. The relationship metadata does not match the parent.");
        }

        var parentRef = new EntityReference(parentLogicalName, parentId);

        foreach (var child in children.Entities)
        {
            // Set the foreign key (lookup) on the child pointing to the parent
            child[metadata.ReferencingAttribute] = parentRef;
            state.Create(child);
        }
    }

    private static void ProcessManyToMany(
        string parentLogicalName,
        Guid parentId,
        ManyToManyRelationshipMetadata metadata,
        EntityCollection children,
        FakeOrganizationService state)
    {
        if (parentLogicalName != metadata.Entity1LogicalName && parentLogicalName != metadata.Entity2LogicalName)
        {
            ErrorFactory.ThrowFault(
                ErrorCodes.InvalidArgument,
                $"Relationship '{metadata.SchemaName}' is between '{metadata.Entity1LogicalName}' and " +
                $"'{metadata.Entity2LogicalName}' but the parent entity is '{parentLogicalName}'. " +
                "The relationship metadata does not match the parent.");
        }

        var isFrom1To2 = parentLogicalName == metadata.Entity1LogicalName;
        var fromAttribute = isFrom1To2 ? metadata.Entity1IntersectAttribute : metadata.Entity2IntersectAttribute;
        var toAttribute = isFrom1To2 ? metadata.Entity2IntersectAttribute : metadata.Entity1IntersectAttribute;

        foreach (var child in children.Entities)
        {
            // Create the child entity first
            var childId = state.Create(child);

            // Create intersection record to link parent and child
            var intersection = new Entity(metadata.IntersectEntityName)
            {
                Attributes = new AttributeCollection
                {
                    { $"{metadata.IntersectEntityName}id", Guid.NewGuid() },
                    { fromAttribute, parentId },
                    { toAttribute, childId }
                }
            };

            state.Create(intersection);
        }
    }
}
