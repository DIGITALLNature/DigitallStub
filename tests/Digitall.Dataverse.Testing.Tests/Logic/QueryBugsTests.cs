// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests proving the existence of critical bugs found in the query architecture review.
/// Each test demonstrates a specific bug that causes incorrect behavior or crashes.
/// </summary>
public class QueryBugsTests
{
    /// <summary>
    /// BUG #1: OrderQuery ThenByDescending missing ContainsKey guard
    /// Throws KeyNotFoundException when 2nd order attribute is missing on some entities
    /// FIXED: Now handles missing attributes correctly
    /// </summary>
    [Test]
    public async Task OrderQuery_MultipleOrdersDescending_HandlesAttributeMissing()
    {
        // Setup: accounts with different attributes present
        var account1 = new Account(Guid.NewGuid())
        {
            Name = "Acme",
            Revenue = new Money(100m) // has revenue
        };
        var account2 = new Account(Guid.NewGuid())
        {
            Name = "Globex"
            // MISSING revenue - this is valid in Dataverse
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([account1, account2]);

        var query = new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true),
            Orders =
            {
                new OrderExpression(Account.LogicalNames.Name, OrderType.Ascending),
                new OrderExpression(Account.LogicalNames.Revenue, OrderType.Descending) // FIXED: now has ContainsKey guard
            }
        };

        // After fix: should NOT throw — Dataverse treats missing attributes as null/lowest
        var result = sut.RetrieveMultiple(query);
        await Assert.That(result.Entities).Count().IsGreaterThan(0);
    }

    /// <summary>
    /// BUG #2: Values mutation - Last/Next/BetweenDates operators modify input QueryExpression
    /// FIXED: Now creates temporary condition instead of mutating input
    /// </summary>
    [Test]
    public async Task LastXDays_DoesNotMutateValuesArray_QueryIsReusable()
    {
        var sut = new FakeOrganizationService();
        var account = new Account(Guid.NewGuid())
        {
            OverriddenCreatedOn = DateTime.UtcNow.AddDays(-15)
        };
        sut.Add(account);

        // Create a LastXDays query with a specific parameter value
        var query = new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(true)
        };
        var daysCondition = new ConditionExpression(Account.LogicalNames.OverriddenCreatedOn, ConditionOperator.LastXDays, 30);
        query.Criteria.AddCondition(daysCondition);

        // Capture original Values before query
        var originalValues = new List<object>(daysCondition.Values);
        await Assert.That(originalValues).Count().IsEqualTo(1);
        await Assert.That(originalValues[0]).IsEqualTo(30);

        // First execution — should NOT mutate Values
        var result1 = sut.RetrieveMultiple(query);
        await Assert.That(result1.Entities).Count().IsGreaterThan(0);

        // FIXED: Values should NOT be mutated anymore
        // It should still contain the original value [30], not [beforeDate, currentDate]
        await Assert.That(daysCondition.Values).Count().IsEqualTo(1); // Still 1, not 2!
        await Assert.That(daysCondition.Values[0]).IsEqualTo(30); // Still 30, not a DateTime!

        // Query is reusable: second execution returns same results
        var result2 = sut.RetrieveMultiple(query);
        await Assert.That(result2.Entities).Count().IsEqualTo(result1.Entities.Count);
    }

    /// <summary>
    /// BUG #3: CompareColumnsHelper MethodInfo reflection not cached
    /// The MethodInfo lookup happens on every call instead of being cached in a static field.
    /// This test verifies CompareColumns works correctly across repeated queries.
    /// </summary>
    [Test]
    public async Task CompareColumns_RepeatedQueries_ReturnsCorrectResults()
    {
        var sut = new FakeOrganizationService();
        var matching = new Account(Guid.NewGuid()) { Name = "Test", [Account.LogicalNames.Description] = "Test" };
        var nonMatching = new Account(Guid.NewGuid()) { Name = "Other", [Account.LogicalNames.Description] = "Different" };
        sut.AddRange([matching, nonMatching]);

        // Execute multiple CompareColumns queries to exercise the reflection path repeatedly
        for (var i = 0; i < 100; i++)
        {
            var query = new QueryExpression(Account.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(true)
            };
            var condition = new ConditionExpression(Account.LogicalNames.Name, ConditionOperator.Equal, Account.LogicalNames.Description)
            {
                CompareColumns = true
            };
            query.Criteria.AddCondition(condition);

            var result = sut.RetrieveMultiple(query);
            await Assert.That(result.Entities).Count().IsEqualTo(1);
            await Assert.That(result.Entities[0].Id).IsEqualTo(matching.Id);
        }
    }

    /// <summary>
    /// BUG #4: MatchFirstRowUsingCrossApply uses non-deterministic .First()
    /// FIXED: Now orders by ID before .First() for deterministic results
    /// </summary>
    [Test]
    public async Task CrossApply_FirstIsDeterministic_ReturnsSameResultsRegardlessOfInsertionOrder()
    {
        // Create two scenarios with same data but different insertion order
        var contact1 = new Contact(Guid.Parse("00000000-0000-0000-0002-000000000001"))
        {
            FirstName = "Alice",
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000002"))
        };

        var contact2 = new Contact(Guid.Parse("00000000-0000-0000-0002-000000000002"))
        {
            FirstName = "Bob",
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000002"))
        };

        var corpB = new Account(Guid.Parse("00000000-0000-0000-0001-000000000002")) { Name = "Corp B" };

        // Scenario 1: Add Alice first
        var sut1 = new FakeOrganizationService();
        sut1.AddRange([corpB, contact1, contact2]);

        var result1 = sut1.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.MatchFirstRowUsingCrossApply)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // Scenario 2: Add Bob first
        var sut2 = new FakeOrganizationService();
        sut2.AddRange([corpB, contact2, contact1]); // reversed order

        var result2 = sut2.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName)
        {
            ColumnSet = new ColumnSet(Account.LogicalNames.Name),
            LinkEntities =
            {
                new LinkEntity(Account.EntityLogicalName, Contact.EntityLogicalName,
                    Account.LogicalNames.AccountId, Contact.LogicalNames.ParentCustomerId,
                    JoinOperator.MatchFirstRowUsingCrossApply)
                {
                    EntityAlias = "c",
                    Columns = new ColumnSet(Contact.LogicalNames.FirstName)
                }
            }
        });

        // FIXED: Both insertion orders now produce the same result (deterministic by ID)
        // Should return Alice (lower ID: 00000000-0000-0000-0002-000000000001)
        var firstName1 = result1.Entities[0].GetAttributeValue<AliasedValue>("c." + Contact.LogicalNames.FirstName)?.Value;
        var firstName2 = result2.Entities[0].GetAttributeValue<AliasedValue>("c." + Contact.LogicalNames.FirstName)?.Value;

        // After fix: Should be the same (both return Alice, the one with lower ID)
        await Assert.That(firstName1).IsEqualTo(firstName2);
        await Assert.That(firstName1).IsEqualTo("Alice"); // Verified: the lower ID entity is returned
    }

    /// <summary>
    /// BUG #5 (partial fix in beta.11): Root-level cross-entity OrderExpression reads EntityName
    /// instead of Alias. The 3-arg constructor sets only Alias; EntityName is null, so the sort
    /// key was never resolved and ordering had no effect.
    /// FIXED: CollectOrders now prefers Alias over EntityName (mirrors CollectLinkOrders).
    /// </summary>
    [Test]
    public async Task RootLevelCrossEntityOrder_ThreeArgConstructorDescending_ReturnsHighestLinkedValue()
    {
        // Arrange – three accounts each linked to a contact with a distinct NumberOfChildren value
        var accountLow = new Account(Guid.NewGuid()) { Name = "Low" };
        var accountMid = new Account(Guid.NewGuid()) { Name = "Mid" };
        var accountHigh = new Account(Guid.NewGuid()) { Name = "High" };

        var contactLow = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 1,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountLow.Id)
        };
        var contactMid = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 5,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountMid.Id)
        };
        var contactHigh = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 10,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountHigh.Id)
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([accountLow, accountMid, accountHigh, contactLow, contactMid, contactHigh]);

        var query = new QueryExpression(Account.EntityLogicalName) { TopCount = 1, ColumnSet = new ColumnSet(Account.LogicalNames.Name) };
        var link = query.AddLink(Contact.EntityLogicalName, Account.LogicalNames.AccountId,
            Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner);
        link.EntityAlias = "c";
        link.Columns = new ColumnSet(Contact.LogicalNames.NumberOfChildren);

        // 3-arg constructor: sets Alias="c", EntityName=null
        query.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Descending, "c"));

        // Act
        var result = sut.RetrieveMultiple(query);

        // Assert – should return the account linked to the contact with the highest value (10)
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].GetAttributeValue<string>(Account.LogicalNames.Name))
            .IsEqualTo("High");
    }

    [Test]
    public async Task RootLevelCrossEntityOrder_ThreeArgConstructorAscending_ReturnsLowestLinkedValue()
    {
        // Arrange
        var accountLow = new Account(Guid.NewGuid()) { Name = "Low" };
        var accountMid = new Account(Guid.NewGuid()) { Name = "Mid" };
        var accountHigh = new Account(Guid.NewGuid()) { Name = "High" };

        var contactLow = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 1,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountLow.Id)
        };
        var contactMid = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 5,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountMid.Id)
        };
        var contactHigh = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 10,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountHigh.Id)
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([accountLow, accountMid, accountHigh, contactLow, contactMid, contactHigh]);

        var query = new QueryExpression(Account.EntityLogicalName) { TopCount = 1, ColumnSet = new ColumnSet(Account.LogicalNames.Name) };
        var link = query.AddLink(Contact.EntityLogicalName, Account.LogicalNames.AccountId,
            Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner);
        link.EntityAlias = "c";
        link.Columns = new ColumnSet(Contact.LogicalNames.NumberOfChildren);

        // 3-arg ascending
        query.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Ascending, "c"));

        // Act
        var result = sut.RetrieveMultiple(query);

        // Assert – should return the account linked to the contact with the lowest value (1)
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].GetAttributeValue<string>(Account.LogicalNames.Name))
            .IsEqualTo("Low");
    }

    [Test]
    public async Task RootLevelCrossEntityOrder_FourArgConstructor_UsesAliasNotEntityName()
    {
        // Arrange – verifies that when both Alias and EntityName are set (4-arg constructor),
        // Alias is used as the key prefix, not EntityName.
        var accountLow = new Account(Guid.NewGuid()) { Name = "Low" };
        var accountHigh = new Account(Guid.NewGuid()) { Name = "High" };

        var contactLow = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 1,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountLow.Id)
        };
        var contactHigh = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 10,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountHigh.Id)
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([accountLow, accountHigh, contactLow, contactHigh]);

        var query = new QueryExpression(Account.EntityLogicalName) { TopCount = 1, ColumnSet = new ColumnSet(Account.LogicalNames.Name) };
        var link = query.AddLink(Contact.EntityLogicalName, Account.LogicalNames.AccountId,
            Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner);
        link.EntityAlias = "c";
        link.Columns = new ColumnSet(Contact.LogicalNames.NumberOfChildren);

        // 4-arg constructor: sets Alias="c", EntityName="contact"
        // Attributes are stored as "c.numberofchildren" (alias prefix), NOT "contact.numberofchildren"
        query.Orders.Add(new OrderExpression(Contact.LogicalNames.NumberOfChildren, OrderType.Descending,
            "c", Contact.EntityLogicalName));

        // Act
        var result = sut.RetrieveMultiple(query);

        // Assert – Alias "c" was used → correctly found "c.numberofchildren" → returns highest
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].GetAttributeValue<string>(Account.LogicalNames.Name))
            .IsEqualTo("High");
    }

    [Test]
    public async Task RootLevelCrossEntityOrder_EntityNameOnlyNoAlias_UsesEntityNameAsFallbackPrefix()
    {
        // Arrange – link entity has an EntityAlias that equals the entity logical name ("contact").
        // The root-level OrderExpression carries only EntityName="contact" (no Alias set).
        // CollectOrders must fall back to EntityName when Alias is empty/null so the key
        // "contact.numberofchildren" is resolved correctly.
        var accountLow = new Account(Guid.NewGuid()) { Name = "Low" };
        var accountHigh = new Account(Guid.NewGuid()) { Name = "High" };

        var contactLow = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 1,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountLow.Id)
        };
        var contactHigh = new Contact(Guid.NewGuid())
        {
            NumberOfChildren = 10,
            ParentCustomerId = new EntityReference(Account.EntityLogicalName, accountHigh.Id)
        };

        var sut = new FakeOrganizationService();
        sut.AddRange([accountLow, accountHigh, contactLow, contactHigh]);
        var query = new QueryExpression(Account.EntityLogicalName) { TopCount = 1, ColumnSet = new ColumnSet(Account.LogicalNames.Name) };
        var link = query.AddLink(Contact.EntityLogicalName, Account.LogicalNames.AccountId,
            Contact.LogicalNames.ParentCustomerId, JoinOperator.Inner);
        // EntityAlias explicitly set to the entity logical name so the key prefix is "contact"
        link.EntityAlias = Contact.EntityLogicalName;
        link.Columns = new ColumnSet(Contact.LogicalNames.NumberOfChildren);

        // OrderExpression with EntityName only (Alias not set) → fallback to EntityName="contact"
        query.Orders.Add(new OrderExpression
        {
            AttributeName = Contact.LogicalNames.NumberOfChildren,
            OrderType = OrderType.Descending,
            EntityName = Contact.EntityLogicalName
        });

        // Act
        var result = sut.RetrieveMultiple(query);

        // Assert – EntityName "contact" used as key prefix → correctly resolved → returns highest
        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].GetAttributeValue<string>(Account.LogicalNames.Name))
            .IsEqualTo("High");
    }

}
