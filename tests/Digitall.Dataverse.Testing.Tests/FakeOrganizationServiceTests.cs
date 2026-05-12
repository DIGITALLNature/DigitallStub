using System;
using System.ServiceModel;
using AwesomeAssertions;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;
using TUnit.Core;
using Account = Digitall.Dataverse.Testing.Tests.Fixtures.Account;

namespace Digitall.Dataverse.Testing.Tests;

public class FakeOrganizationServiceTests
{
    [Before(HookType.Class)]
    public static void MyClassInitialize()
    {
        DotNetEnv.Env.Load();
    }

    [Test]
    public void ModelIsDetected()
    {
        var sut = new FakeOrganizationService();
        sut.State.ModelAssemblies.Should().NotBeNullOrEmpty().And.Contain(a => a.FullName == typeof(TestData).Assembly.FullName);
    }

    [Test]
    public void ModelIsSeeded()
    {
        var sut = new FakeOrganizationService();
        sut.State.Entities.Should().NotBeNull().And.BeEmpty();

        sut.AddRange(TestData.Default);
        sut.State.Entities.Should().NotBeEmpty().And.ContainKeys(Account.EntityLogicalName, Contact.EntityLogicalName);
        sut.State.Entities[Account.EntityLogicalName].Should().HaveCount(2);
        sut.State.Entities[Contact.EntityLogicalName].Should().HaveCount(3);
    }

    [Test]
    public void EntityTypeIsKnown_ReturnsTrue_WhenEntityTypeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.EntityTypeIsKnown(Account.EntityLogicalName, out var knownEntityType);

        result.Should().BeTrue();
        knownEntityType.Should().NotBeNull().And.Be<Account>();
    }

    [Test]
    public void EntityTypeIsKnown_ReturnsFalse_WhenEntityTypeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.EntityTypeIsKnown("non_existing", out var knownEntityType);

        result.Should().BeFalse();
        knownEntityType.Should().BeNull();
    }

    [Test]
    public void IsKnownAttributeForType_ReturnsTrue_WhenAttributeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId, out var attributeInfo);


        result.Should().BeTrue();
        attributeInfo.Should().NotBeNull().And.Subject.PropertyType.FullName.Should().Be(typeof(EntityReference).FullName);
    }

    [Test]
    public void IsKnownAttributeForType_ReturnsFalse_WhenAttributeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, "non_existing", out var attributeInfo);

        result.Should().BeFalse();
        attributeInfo.Should().BeNull();
    }

    [Test]
    public void ThrowIfNotKnownEntityType_ThrowsArgumentException_WhenEntityTypeIsNotKnown()
    {
        var dataverse = new FakeOrganizationService();
        var action = () => dataverse.ThrowIfNotKnownEntityType("unknownEntity");

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public void ThrowIfNotKnownEntityType_DoesNotThrow_WhenEntityTypeIsKnown()
    {
        var dataverse = new FakeOrganizationService();
        var action = () => dataverse.ThrowIfNotKnownEntityType(Account.EntityLogicalName);

        action.Should().NotThrow();
    }

    [Test]
    public void ThrowIfNotKnownAttribute_ThrowsFaultException_WhenAttributeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, "non_existing");

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoAttribute);
    }

    [Test]
    public void ThrowIfNotKnownAttribute_DoesNotThrow_WhenAttributeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId);

        action.Should().NotThrow();
    }

    [Test]
    public void Create_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var dataverse = new FakeOrganizationService();
        var action = () => dataverse.Create(null);

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public void Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account { Name = nameof(Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        result.Should().NotBeEmpty();
        sut.State.Entities.Should().ContainKey(Account.EntityLogicalName).And.Subject[Account.EntityLogicalName].Should().ContainKey(result);

        sut.State.Entities[Account.EntityLogicalName][result].Should().NotBeSameAs(entity);
    }

    [Test]
    public void Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        result.Should().Be(id);
        sut.State.Entities.Should().ContainKey(Account.EntityLogicalName).And.Subject[Account.EntityLogicalName].Should().ContainKey(id);
    }

    [Test]
    public void Create_ThrowsFaultException_WhenEntityIdIsDuplicate()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Create_ThrowsFaultException_WhenEntityIdIsDuplicate) };

        var actionOne = () => sut.Create(entity);
        var actionTwo = () => sut.Create(entity);
        actionOne.Should().NotThrow();
        actionTwo.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.DuplicateRecord);
    }

    [Test]
    public void Retrieve_EntityExists_ReturnsRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.Retrieve(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001"), new ColumnSet(true));

        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var resultAcc = result.ToEntity<Account>();
        resultAcc.Should().NotBeNull();
        resultAcc.Name.Should().Be("A Corp");
        resultAcc.Telephone1.Should().Be("1");
        resultAcc.Telephone2.Should().Be("2");
        resultAcc.Telephone3.Should().BeNull();
    }

    [Test]
    public void Retrieve_EntityExists_withColumnSet_ReturnsRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.Retrieve(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001"), new ColumnSet(Account.LogicalNames.Name, Account.LogicalNames.Telephone1));

        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var resultAcc = result.ToEntity<Account>();
        resultAcc.Should().NotBeNull();
        resultAcc.Name.Should().Be("A Corp");
        resultAcc.Telephone1.Should().Be("1");
        resultAcc.Telephone2.Should().BeNull();
        resultAcc.Telephone3.Should().BeNull();
    }

    [Test]
    public void Retrieve_EntityDoesNotExist_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var action = () => sut.Retrieve(Account.EntityLogicalName, Guid.Parse("10000000-0000-0000-0000-000000000000"), new ColumnSet(true));

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public void Retrieve_EntityExists_ReturnsClonedRecord()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = "Inline Corp", Telephone1 = "1", Telephone2 = "2", Telephone3 = "3" };
        sut.Add(entity);

        var result = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));

        result.Should().NotBeNull();
        result.Id.Should().Be(id);

        result.Should().NotBeSameAs(entity);
    }

    [Test]
    public void Update_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.Update(null);

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public void Update_ThrowsObjectDoesNotExistFault_WhenEntityDoesNotExist()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account(Guid.NewGuid());

        var action = () => sut.Update(entity);

        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public void Update_UpdatesEntityInStateDictionary()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary) };
        sut.Add(entity);

        var updatedEntity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary), Description = "Changed" };
        sut.Update(updatedEntity);

        sut.State.Entities[Account.EntityLogicalName].Should().ContainKey(id);
        sut.State.Entities[Account.EntityLogicalName][id].ToEntity<Account>().Description.Should().BeEquivalentTo(updatedEntity.Description);
        sut.State.Entities[Account.EntityLogicalName][id].Should().NotBeSameAs(updatedEntity);
    }

    [Test]
    public void Delete_WithValidEntityNameAndId_RemovesRecord()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithValidEntityNameAndId_RemovesRecord) });
        sut.Delete(Account.EntityLogicalName, id);

        sut.State.Entities[Account.EntityLogicalName].Should().NotContainKey(id);
    }

    [Test]
    public void Delete_WithNullEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNullEntityName_ThrowsFault) });


        var action = () => sut.Delete(null, id);
        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public void Delete_WithUnknownEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithUnknownEntityName_ThrowsFault) });


        var action = () => sut.Delete("invalid", id);
        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public void Delete_WithNonExistentId_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNonExistentId_ThrowsFault) });


        var action = () => sut.Delete(Account.EntityLogicalName, Guid.NewGuid());
        action.Should().Throw<FaultException<OrganizationServiceFault>>().And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public void AddDefaultRequests()
    {
        var sut = new FakeOrganizationService();
        sut.AddDefaultRequests();

        // Verify that default requests were added by executing a CreateRequest
        var createRequest = new CreateRequest { Target = new Entity("account") { Id = Guid.NewGuid() } };
        var action = () => sut.Execute(createRequest);
        action.Should().NotThrow();
    }
}
