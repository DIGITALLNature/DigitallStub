// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing;

public class MetadataService
{
    private readonly Dictionary<string, EntityMetadata> _entityMetadata;
    private readonly Dictionary<string, RelationshipMetadataBase> _relationships;

    public MetadataService(Dictionary<string, EntityMetadata> entityMetadata, Dictionary<string, RelationshipMetadataBase> relationships)
    {
        _entityMetadata = entityMetadata;
        _relationships = relationships;
    }

    public void AddMetadata(EntityMetadata entityMetadata)
    {
        _entityMetadata.Add(entityMetadata.LogicalName, entityMetadata);

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

    public void AddRelationship(RelationshipMetadataBase relationship)
    {
        _relationships[relationship.SchemaName] = relationship;
    }

    public void AddRelationships(IEnumerable<RelationshipMetadataBase> relationships)
    {
        foreach (var relationship in relationships)
        {
            AddRelationship(relationship);
        }
    }

    public RelationshipMetadataBase GetRelationship(string relationshipSchemaName)
    {
        if (_relationships.ContainsKey(relationshipSchemaName))
        {
            return _relationships[relationshipSchemaName];
        }

        return null;
    }
}
