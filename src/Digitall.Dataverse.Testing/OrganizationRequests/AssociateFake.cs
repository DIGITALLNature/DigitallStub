// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Errors;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class AssociateFake : OrganizationRequestFake<AssociateRequest, AssociateResponse>
{
    public override AssociateResponse Execute(AssociateRequest organizationRequest, FakeOrganizationService state)
    {
        var entityName = organizationRequest.Target.LogicalName;
        var entityId = organizationRequest.Target.Id;
        var relationship = organizationRequest.Relationship;
        var relatedEntities = organizationRequest.RelatedEntities;

        var relationshipMetadata = state.GetRelationship(relationship.SchemaName);

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
                        var targetExists = state.CreateQuery(fromEntityName).FirstOrDefault(e => e.Id == entityId) != null;

                        if (!targetExists)
                        {
                            throw new Exception($"{fromEntityName} with Id {entityId.ToString()} doesn't exist");
                        }

                        var relatedExists = state.CreateQuery(toEntityName).FirstOrDefault(e => e.Id == relatedEntityReference.Id) != null;

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

                        state.Create(association);
                        break;
                    }
                case OneToManyRelationshipMetadata oneToManyRelationshipMetadata:
                    {
                        //Get entity to update
                        var entityToUpdate = new Entity(relatedEntityReference.LogicalName)
                        {
                            Id = relatedEntityReference.Id, [oneToManyRelationshipMetadata.ReferencingAttribute] = new EntityReference(entityName, entityId)
                        };

                        state.Update(entityToUpdate);
                        break;
                    }
                default:
                    throw new ArgumentException("RelationShip Metadata is not typed correctly");
            }
        }

        return new AssociateResponse();
    }
}
