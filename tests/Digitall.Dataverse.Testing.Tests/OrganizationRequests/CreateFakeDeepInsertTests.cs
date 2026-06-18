// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
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

        _sut.Execute(new CreateRequest { Target = account });

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

        void Act() => _sut.Execute(new CreateRequest { Target = account });

        await Assert.That(Act).Throws<FaultException>();
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

    [Test]
    public async Task Create_WithMismatchedOneToManyRelationship_ThrowsFault()
    {
        // Register relationship where ReferencedEntity is "contact", not "account"
        _sut.State.Relationships["mismatched_onetomany"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "mismatched_onetomany",
            ReferencedEntity = "contact",
            ReferencedAttribute = "contactid",
            ReferencingEntity = "task",
            ReferencingAttribute = "regardingobjectid"
        };

        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        var task = new Entity("task") { ["subject"] = "Follow up" };

        account.RelatedEntities[new Relationship("mismatched_onetomany")] =
            new EntityCollection([task]);

        void Act() => _sut.Execute(new CreateRequest { Target = account });

        await Assert.That(Act).Throws<FaultException>();
    }

    [Test]
    public async Task Create_WithMismatchedManyToManyRelationship_ThrowsFault()
    {
        // Register relationship between "systemuser" and "role" but use it on "account"
        var account = new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso" };
        var role = new Entity("role") { ["name"] = "Admin" };

        account.RelatedEntities[new Relationship("systemuserroles_association")] =
            new EntityCollection([role]);

        void Act() => _sut.Execute(new CreateRequest { Target = account });

        await Assert.That(Act).Throws<FaultException>();
    }

    [Test]
    public async Task Create_WithReversedOneToMany_SetsForeignKeyOnParent()
    {
        // Scenario: calendarrule (Referencing) has a lookup to calendar (Referenced)
        // Deep insert from calendarrule side should create the calendar and set FK on calendarrule
        _sut.State.Relationships["calendarrule_innercalendar"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "calendarrule_innercalendar",
            ReferencedEntity = "calendar",
            ReferencedAttribute = "calendarid",
            ReferencingEntity = "calendarrule",
            ReferencingAttribute = "innercalendarid"
        };

        // Also register the parent relationship: calendar → calendarrule
        _sut.State.Relationships["calendar_calendar_rules"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "calendar_calendar_rules",
            ReferencedEntity = "calendar",
            ReferencedAttribute = "calendarid",
            ReferencingEntity = "calendarrule",
            ReferencingAttribute = "calendarid"
        };

        var calendar = new Entity("calendar") { Id = Guid.NewGuid(), ["name"] = "Business Hours" };
        var innerCalendar = new Entity("calendar") { ["name"] = "Inner Schedule" };
        var calendarRule = new Entity("calendarrule")
        {
            ["description"] = "Rule 1",
            RelatedEntities =
            {
                [new Relationship("calendarrule_innercalendar")] = new EntityCollection([innerCalendar])
            }
        };

        // Nested deep insert: calendar → calendarrule → innercalendar (reverse direction)

        calendar.RelatedEntities[new Relationship("calendar_calendar_rules")] =
            new EntityCollection([calendarRule]);

        _sut.Execute(new CreateRequest { Target = calendar });

        // Verify all entities were created
        var calendars = _sut.CreateQuery("calendar").ToList();
        var rules = _sut.CreateQuery("calendarrule").ToList();

        await Assert.That(calendars).Count().IsEqualTo(2); // parent + inner
        await Assert.That(rules).Count().IsEqualTo(1);

        // Verify the FK on calendarrule points to the inner calendar
        var createdRule = rules.Single();
        var innerCalRef = createdRule.GetAttributeValue<EntityReference>("innercalendarid");
        await Assert.That(innerCalRef).IsNotNull();
        await Assert.That(innerCalRef!.LogicalName).IsEqualTo("calendar");

        // The inner calendar should be the one named "Inner Schedule"
        var innerCal = _sut.Retrieve("calendar", innerCalRef.Id, new ColumnSet(true));
        await Assert.That(innerCal.GetAttributeValue<string>("name")).IsEqualTo("Inner Schedule");
    }

    [Test]
    public async Task Create_WithReversedOneToMany_MultipleChildren_ThrowsFault()
    {
        // N:1 direction: a lookup can only point to one record
        _sut.State.Relationships["calendarrule_innercalendar"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "calendarrule_innercalendar",
            ReferencedEntity = "calendar",
            ReferencedAttribute = "calendarid",
            ReferencingEntity = "calendarrule",
            ReferencingAttribute = "innercalendarid"
        };

        var rule = new Entity("calendarrule") { Id = Guid.NewGuid(), ["description"] = "Rule" };
        var cal1 = new Entity("calendar") { ["name"] = "Cal 1" };
        var cal2 = new Entity("calendar") { ["name"] = "Cal 2" };

        rule.RelatedEntities[new Relationship("calendarrule_innercalendar")] =
            new EntityCollection([cal1, cal2]); // Two children on N:1 → invalid

        void Act() => _sut.Execute(new CreateRequest { Target = rule });

        await Assert.That(Act).Throws<FaultException>();
    }

    [Test]
    public async Task Create_WithReversedOneToMany_WrongChildLogicalName_ThrowsFault()
    {
        _sut.State.Relationships["calendarrule_innercalendar"] = new OneToManyRelationshipMetadata
        {
            SchemaName = "calendarrule_innercalendar",
            ReferencedEntity = "calendar",
            ReferencedAttribute = "calendarid",
            ReferencingEntity = "calendarrule",
            ReferencingAttribute = "innercalendarid"
        };

        var rule = new Entity("calendarrule") { Id = Guid.NewGuid(), ["description"] = "Rule" };
        var wrongEntity = new Entity("account") { ["name"] = "Not a calendar" };

        rule.RelatedEntities[new Relationship("calendarrule_innercalendar")] =
            new EntityCollection([wrongEntity]);

        void Act() => _sut.Execute(new CreateRequest { Target = rule });

        await Assert.That(Act).Throws<FaultException>();
    }
}
