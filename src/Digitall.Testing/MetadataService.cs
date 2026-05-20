// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Testing;

public class MetadataService(Dictionary<string, EntityMetadata> entityMetadata, Dictionary<string, RelationshipMetadataBase> relationships)
{
    public void AddMetadata(EntityMetadata metadata)
    {
        entityMetadata.Add(metadata.LogicalName, metadata);

        var relationshipsList = new List<RelationshipMetadataBase>();
        if (metadata.ManyToManyRelationships != null)
        {
            relationshipsList.AddRange(metadata.ManyToManyRelationships);
        }

        if (metadata.OneToManyRelationships != null)
        {
            relationshipsList.AddRange(metadata.OneToManyRelationships);
        }

        if (metadata.ManyToOneRelationships != null)
        {
            relationshipsList.AddRange(metadata.ManyToOneRelationships);
        }

        AddRelationships(relationshipsList);
    }

    public void AddMetadata(IEnumerable<EntityMetadata> metadataCollection)
    {
        foreach (var metadata in metadataCollection)
        {
            AddMetadata(metadata);
        }
    }

    public void AddRelationship(RelationshipMetadataBase relationship)
    {
        relationships[relationship.SchemaName] = relationship;
    }

    public void AddRelationships(IEnumerable<RelationshipMetadataBase> items)
    {
        foreach (var relationship in items)
        {
            AddRelationship(relationship);
        }
    }

    public RelationshipMetadataBase? GetRelationship(string relationshipSchemaName)
    {
        return relationships.GetValueOrDefault(relationshipSchemaName);
    }
}
