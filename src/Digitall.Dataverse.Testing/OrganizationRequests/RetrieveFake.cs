// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Extensions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class RetrieveFake : OrganizationRequestFake<RetrieveRequest, RetrieveResponse>
{
    public override RetrieveResponse Execute(RetrieveRequest organizationRequest, FakeOrganizationService state)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(organizationRequest);

        Entity record;
        if (organizationRequest.Target.Id == Guid.Empty && organizationRequest.Target.KeyAttributes.Count > 0)
        {
            record = state.RetrieveWithAlternateKey(organizationRequest.Target.LogicalName, organizationRequest.Target.KeyAttributes, organizationRequest.ColumnSet);
        }
        else
        {
            record = state.RetrieveCore(organizationRequest.Target.LogicalName, organizationRequest.Target.Id, organizationRequest.ColumnSet);
        }

        if (organizationRequest.RelatedEntitiesQuery?.Count > 0)
        {
            PopulateRelatedEntities(organizationRequest, record, state);
        }

        return new RetrieveResponse
        {
            ResponseName = "Retrieve",
            Results = new ParameterCollection { { nameof(RetrieveResponse.Entity), record } }
        };
    }

    private static void PopulateRelatedEntities(RetrieveRequest request, Entity record, FakeOrganizationService state)
    {
        foreach (var kvp in request.RelatedEntitiesQuery)
        {
            var relationship = kvp.Key;
            var relatedQuery = kvp.Value;

            if (!state.State.Relationships.TryGetValue(relationship.SchemaName, out var relationshipMetadata))
                continue;

            EntityCollection relatedEntities;

            switch (relationshipMetadata)
            {
                case OneToManyRelationshipMetadata oneToMany:
                    relatedEntities = RetrieveOneToManyRelated(record, oneToMany, relatedQuery, state);
                    break;

                case ManyToManyRelationshipMetadata manyToMany:
                    relatedEntities = RetrieveManyToManyRelated(record, manyToMany, relatedQuery, state);
                    break;

                default:
                    continue;
            }

            record.RelatedEntities[relationship] = relatedEntities;
        }
    }

    private static EntityCollection RetrieveOneToManyRelated(
        Entity record,
        OneToManyRelationshipMetadata oneToMany,
        QueryBase relatedQuery,
        FakeOrganizationService state)
    {
        var qe = ToQueryExpression(relatedQuery, oneToMany.ReferencingEntity!);
        qe.Criteria ??= new FilterExpression();
        qe.Criteria.AddCondition(oneToMany.ReferencingAttribute!, ConditionOperator.Equal, record.Id);
        return state.RetrieveMultiple(qe);
    }

    private static EntityCollection RetrieveManyToManyRelated(
        Entity record,
        ManyToManyRelationshipMetadata manyToMany,
        QueryBase relatedQuery,
        FakeOrganizationService state)
    {
        var isEntity1 = record.LogicalName == manyToMany.Entity1LogicalName;
        var fromIntersectAttribute = isEntity1 ? manyToMany.Entity1IntersectAttribute : manyToMany.Entity2IntersectAttribute;
        var toIntersectAttribute   = isEntity1 ? manyToMany.Entity2IntersectAttribute : manyToMany.Entity1IntersectAttribute;
        var toEntityName           = isEntity1 ? manyToMany.Entity2LogicalName        : manyToMany.Entity1LogicalName;

        var intersectQuery = new QueryExpression(manyToMany.IntersectEntityName)
        {
            ColumnSet = new ColumnSet(true),
            Criteria = new FilterExpression()
        };
        intersectQuery.Criteria.AddCondition(fromIntersectAttribute!, ConditionOperator.Equal, record.Id);

        var intersectResults = state.RetrieveMultiple(intersectQuery);
        var relatedIds = intersectResults.Entities
            .Select(e => e.GetAttributeValue<Guid>(toIntersectAttribute!))
            .Where(id => id != Guid.Empty)
            .Cast<object>()
            .ToArray();

        if (relatedIds.Length == 0)
            return new EntityCollection { EntityName = toEntityName };

        var primaryIdAttribute = state.State.EntityMetadata.TryGetValue(toEntityName!, out var meta)
                                 && !string.IsNullOrEmpty(meta.PrimaryIdAttribute)
            ? meta.PrimaryIdAttribute!
            : toEntityName + "id";

        var qe = ToQueryExpression(relatedQuery, toEntityName!);
        qe.Criteria ??= new FilterExpression();
        qe.Criteria.AddCondition(primaryIdAttribute, ConditionOperator.In, relatedIds);
        return state.RetrieveMultiple(qe);
    }

    private static QueryExpression ToQueryExpression(QueryBase query, string defaultEntityName)
    {
        return query switch
        {
            QueryExpression qe       => qe.CloneQuery(),
            QueryByAttribute qba     => TranslateQueryByAttribute(qba, defaultEntityName),
            _                        => new QueryExpression(defaultEntityName) { ColumnSet = new ColumnSet(true) }
        };
    }

    private static QueryExpression TranslateQueryByAttribute(QueryByAttribute qba, string defaultEntityName)
    {
        var qe = new QueryExpression(qba.EntityName ?? defaultEntityName)
        {
            ColumnSet = qba.ColumnSet,
            TopCount  = qba.TopCount,
            PageInfo  = qba.PageInfo,
            Criteria  = new FilterExpression()
        };
        for (var i = 0; i < qba.Attributes.Count; i++)
        {
            qe.Criteria.AddCondition(new ConditionExpression(qba.Attributes[i], ConditionOperator.Equal, qba.Values[i]));
        }
        foreach (var order in qba.Orders)
        {
            qe.AddOrder(order.AttributeName, order.OrderType);
        }
        return qe;
    }
}
