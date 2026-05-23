// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class CreateFakeDeepInsertTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService
        {
            Options = { UserId = Guid.NewGuid() }
        };

        // Register OneToMany relationship: account -> contacts
        _sut.State.Relationships["contact_customer_accounts"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "contact_customer_accounts",
            ReferencedEntity = "account",
            ReferencedAttribute = "accountid",
            ReferencingEntity = "contact",
            ReferencingAttribute = "parentcustomerid"
        };

        // Register ManyToMany relationship: account <-> role
        _sut.State.Relationships["systemuserroles_association"] = new ManyToManyRelationshipMetadata
        {
            SchemaName = "systemuserroles_association",
            Entity1LogicalName = "systemuser",
            Entity2LogicalName = "role",
            Entity1IntersectAttribute = "systemuserid",
            Entity2IntersectAttribute = "roleid",
            IntersectEntityName = "systemuserroles"
        };

        await Task.CompletedTask;
    }

    [Test]
    public async Task Create_WithOneToManyRelatedEntities_CreatesChildEntities()
    {
        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        var contact1 = new Entity("contact") { ["lastname"] = "Smith" };
        var contact2 = new Entity("contact") { ["lastname"] = "Jones" };

        account.RelatedEntities[new Relationship("contact_customer_accounts")] =
            new EntityCollection([contact1, contact2]);

        var response = (CreateResponse)_sut.Execute(new CreateRequest { Target = account });

        var contacts = _sut.CreateQuery("contact").ToList();
        await Assert.That(contacts).Count().IsEqualTo(2);
    }

    [Test]
    public async Task Create_WithOneToManyRelatedEntities_SetsForeignKeyOnChildren()
    {
        var accountId = Guid.NewGuid();
        var account = new Entity("account") { Id = accountId, ["name"] = "Contoso" };
        var contact = new Entity("contact") { ["lastname"] = "Smith" };

        account.RelatedEntities[new Relationship("contact_customer_accounts")] =
            new EntityCollection([contact]);

        _sut.Execute(new CreateRequest { Target = account });

        var createdContact = _sut.CreateQuery("contact").Single();
        var parentRef = createdContact.GetAttributeValue<EntityReference>("parentcustomerid");

        await Assert.That(parentRef).IsNotNull();
        await Assert.That(parentRef!.Id).IsEqualTo(accountId);
        await Assert.That(parentRef.LogicalName).IsEqualTo("account");
    }

    [Test]
    public async Task Create_WithManyToManyRelatedEntities_CreatesChildAndIntersection()
    {
        var userId = Guid.NewGuid();
        var user = new Entity("systemuser") { Id = userId, ["fullname"] = "Admin" };
        var role = new Entity("role") { ["name"] = "System Administrator" };

        user.RelatedEntities[new Relationship("systemuserroles_association")] =
            new EntityCollection([role]);

        _sut.Execute(new CreateRequest { Target = user });

        // Child entity was created
        var roles = _sut.CreateQuery("role").ToList();
        await Assert.That(roles).Count().IsEqualTo(1);

        // Intersection record was created
        var intersections = _sut.CreateQuery("systemuserroles").ToList();
        await Assert.That(intersections).Count().IsEqualTo(1);

        var intersection = intersections.Single();
        await Assert.That(intersection.GetAttributeValue<Guid>("systemuserid")).IsEqualTo(userId);
        await Assert.That(intersection.GetAttributeValue<Guid>("roleid")).IsEqualTo(roles.Single().Id);
    }

    [Test]
    public async Task Create_WithUnregisteredRelationship_ThrowsFault()
    {
        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        var task = new Entity("task") { ["subject"] = "Follow up" };

        account.RelatedEntities[new Relationship("unknown_relationship")] =
            new EntityCollection([task]);

        var act = () => _sut.Execute(new CreateRequest { Target = account });

        await Assert.That(act).Throws<FaultException>();
    }

    [Test]
    public async Task Create_WithNestedRelatedEntities_CreatesRecursively()
    {
        // Register a second OneToMany: contact -> tasks
        _sut.State.Relationships["contact_tasks"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "contact_tasks",
            ReferencedEntity = "contact",
            ReferencedAttribute = "contactid",
            ReferencingEntity = "task",
            ReferencingAttribute = "regardingobjectid"
        };

        var accountId = Guid.NewGuid();
        var account = new Entity("account") { Id = accountId, ["name"] = "Contoso" };

        var contact = new Entity("contact") { ["lastname"] = "Smith" };
        var task = new Entity("task") { ["subject"] = "Call back" };

        // Nested: contact has related tasks
        contact.RelatedEntities[new Relationship("contact_tasks")] =
            new EntityCollection([task]);

        account.RelatedEntities[new Relationship("contact_customer_accounts")] =
            new EntityCollection([contact]);

        _sut.Execute(new CreateRequest { Target = account });

        // All three levels were created
        var accounts = _sut.CreateQuery("account").ToList();
        var contacts = _sut.CreateQuery("contact").ToList();
        var tasks = _sut.CreateQuery("task").ToList();

        await Assert.That(accounts).Count().IsEqualTo(1);
        await Assert.That(contacts).Count().IsEqualTo(1);
        await Assert.That(tasks).Count().IsEqualTo(1);

        // Verify FK chain
        var createdContact = contacts.Single();
        var createdTask = tasks.Single();

        var contactParent = createdContact.GetAttributeValue<EntityReference>("parentcustomerid");
        await Assert.That(contactParent!.Id).IsEqualTo(accountId);

        var taskRegarding = createdTask.GetAttributeValue<EntityReference>("regardingobjectid");
        await Assert.That(taskRegarding!.Id).IsEqualTo(createdContact.Id);
    }

    [Test]
    public async Task Create_WithEmptyRelatedEntities_DoesNotThrow()
    {
        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        // RelatedEntities is empty by default

        var response = (CreateResponse)_sut.Execute(new CreateRequest { Target = account });

        await Assert.That(response.id).IsNotEqualTo(Guid.Empty);
    }
}
