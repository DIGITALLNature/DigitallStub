// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing;

public class FakeOrganizationServiceAsync(TimeProvider timeProvider) : FakeOrganizationService(timeProvider), IOrganizationServiceAsync2
{
    public FakeOrganizationServiceAsync() : this(TimeProvider.System)
    {
    }

    // IOrganizationServiceAsync
    public Task<Guid> CreateAsync(Entity entity) => Task.FromResult(Create(entity));
    public Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet) => Task.FromResult(Retrieve(entityName, id, columnSet));

    public Task UpdateAsync(Entity entity)
    {
        Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string entityName, Guid id)
    {
        Delete(entityName, id);
        return Task.CompletedTask;
    }

    public Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request) => Task.FromResult(Execute(request));

    public Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        Associate(entityName, entityId, relationship, relatedEntities);
        return Task.CompletedTask;
    }

    public Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
    {
        Disassociate(entityName, entityId, relationship, relatedEntities);
        return Task.CompletedTask;
    }

    public Task<EntityCollection> RetrieveMultipleAsync(QueryBase query) => Task.FromResult(RetrieveMultiple(query));

    // IOrganizationServiceAsync2 — CancellationToken overloads
    public Task<Guid> CreateAsync(Entity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Create(entity));
    }

    public Task<Entity> CreateAndReturnAsync(Entity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var id = Create(entity);
        return Task.FromResult(Retrieve(entity.LogicalName, id, new ColumnSet(true)));
    }

    public Task<Entity> RetrieveAsync(string entityName, Guid id, ColumnSet columnSet, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Retrieve(entityName, id, columnSet));
    }

    public Task UpdateAsync(Entity entity, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(string entityName, Guid id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Delete(entityName, id);
        return Task.CompletedTask;
    }

    public Task<OrganizationResponse> ExecuteAsync(OrganizationRequest request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Execute(request));
    }

    public Task AssociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Associate(entityName, entityId, relationship, relatedEntities);
        return Task.CompletedTask;
    }

    public Task DisassociateAsync(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Disassociate(entityName, entityId, relationship, relatedEntities);
        return Task.CompletedTask;
    }

    public Task<EntityCollection> RetrieveMultipleAsync(QueryBase query, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(RetrieveMultiple(query));
    }
}
