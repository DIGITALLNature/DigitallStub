using System;
using System.ServiceModel;
using Digitall.Stub.Errors;
using Digitall.Stub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.Tests;

[TestClass]
public class DataverseStubTests
{
    [ClassInitialize]
    public static void MyClassInitialize(TestContext testContext)
    {
        DotNetEnv.Env.Load();
    }

    [TestMethod]
    public void ModelIsDetected()
    {
        var sut = new DataverseStub();
        sut.ModelAssemblies.Should().NotBeNull().And.NotBeEmpty().And.Contain(a => a.FullName == typeof(TestData).Assembly.FullName);
    }

    [TestMethod]
    public void ModelIsSeeded()
    {
        var sut = new DataverseStub();
        sut.State.Should().NotBeNull().And.BeEmpty();

        sut.AddRange(TestData.Default);
        sut.State.Should().NotBeEmpty().And.ContainKeys(Account.EntityLogicalName, Contact.EntityLogicalName);
        sut.State[Account.EntityLogicalName].Should().HaveCount(2);
        sut.State[Contact.EntityLogicalName].Should().HaveCount(3);
    }

    [TestMethod]
    public void Stubs_Dispatch_Working()
    {
        var sut = new DataverseStub();
        var result = sut.Execute(new WhoAmIRequest());

        result.Should().BeAssignableTo<WhoAmIResponse>();
    }

    [TestMethod]
    public void EntityTypeIsKnown_ReturnsTrue_WhenEntityTypeIsKnown()
    {
        var sut = new DataverseStub();
        var result = sut.EntityTypeIsKnow(Account.EntityLogicalName, out var knownEntityType);

        result.Should().BeTrue();
        knownEntityType.Should().NotBeNull().And.Be<Account>();
    }

    [TestMethod]
    public void EntityTypeIsKnown_ReturnsFalse_WhenEntityTypeIsNotKnown()
    {
        var sut = new DataverseStub();
        var result = sut.EntityTypeIsKnow("non_existing", out var knownEntityType);

        result.Should().BeFalse();
        knownEntityType.Should().BeNull();
    }

    [TestMethod]
    public void IsKnownAttributeForType_ReturnsTrue_WhenAttributeIsKnown()
    {
        var sut = new DataverseStub();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId, out var attributeInfo);


        result.Should().BeTrue();
        attributeInfo.Should().NotBeNull()
            .And.Subject.PropertyType.FullName.Should().Be(typeof(EntityReference).FullName);

    }

    [TestMethod]
    public void IsKnownAttributeForType_ReturnsFalse_WhenAttributeIsNotKnown()
    {
        var sut = new DataverseStub();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, "non_existing", out var attributeInfo);

        result.Should().BeFalse();
        attributeInfo.Should().BeNull();
    }

    [TestMethod]
    public void ThrowIfNotKnownEntityType_ThrowsArgumentException_WhenEntityTypeIsNotKnown()
    {
        var stub = new DataverseStub();
        var action = () => stub.ThrowIfNotKnownEntityType("unknownEntity");

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoEntity);

    }

    [TestMethod]
    public void ThrowIfNotKnownEntityType_DoesNotThrow_WhenEntityTypeIsKnown()
    {
        var stub = new DataverseStub();
        var action = () => stub.ThrowIfNotKnownEntityType(Account.EntityLogicalName);

        action.Should().NotThrow();
    }

    [TestMethod]
    public void ThrowIfNotKnownAttribute_ThrowsFaultException_WhenAttributeIsNotKnown()
    {
        var sut = new DataverseStub();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, "non_existing");

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoAttribute);
    }

    [TestMethod]
    public void ThrowIfNotKnownAttribute_DoesNotThrow_WhenAttributeIsKnown()
    {
        var sut = new DataverseStub();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId);

        action.Should().NotThrow();
    }

    [TestMethod]
    public void Create_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var stub = new DataverseStub();
        var action = () => stub.Create(null);

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [TestMethod]
    public void Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new DataverseStub();
        var entity = new Account { Name = nameof(Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        result.Should().NotBeEmpty();
        sut.State.Should().ContainKey(Account.EntityLogicalName)
            .And.Subject[Account.EntityLogicalName].Should().ContainKey(result);

        sut.State[Account.EntityLogicalName][result].Should().NotBeSameAs(entity);
    }

    [TestMethod]
    public void Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        result.Should().Be(id);
        sut.State.Should().ContainKey(Account.EntityLogicalName)
            .And.Subject[Account.EntityLogicalName].Should().ContainKey(id);
    }

    [TestMethod]
    public void Create_ThrowsFaultException_WhenEntityIdIsDuplicate()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ThrowsFaultException_WhenEntityIdIsDuplicate) };

        var actionOne = () => sut.Create(entity);
        var actionTwo = () => sut.Create(entity);
        actionOne.Should().NotThrow();
        actionTwo.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.DuplicateRecord);
    }

    [TestMethod]
    public void Retrieve_EntityExists_ReturnsRecord()
    {
        var sut = new DataverseStub();
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

    [TestMethod]
    public void Retrieve_EntityExists_withColumnSet_ReturnsRecord()
    {
        var sut = new DataverseStub();
        sut.AddRange(TestData.Default);

        var result = sut.Retrieve(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001"), new ColumnSet(
            Account.LogicalNames.Name,
            Account.LogicalNames.Telephone1
            ));

        result.Should().NotBeNull();
        result.Id.Should().Be(Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var resultAcc = result.ToEntity<Account>();
        resultAcc.Should().NotBeNull();
        resultAcc.Name.Should().Be("A Corp");
        resultAcc.Telephone1.Should().Be("1");
        resultAcc.Telephone2.Should().BeNull();
        resultAcc.Telephone3.Should().BeNull();
    }

    [TestMethod]
    public void Retrieve_EntityDoesNotExist_ThrowsFault()
    {
        var sut = new DataverseStub();
        sut.AddRange(TestData.Default);

        var action = () =>  sut.Retrieve(Account.EntityLogicalName, Guid.Parse("10000000-0000-0000-0000-000000000000"), new ColumnSet(true));

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }

    [TestMethod]
    public void Retrieve_EntityExists_ReturnsClonedRecord()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = "Inline Corp", Telephone1 = "1", Telephone2 = "2", Telephone3 = "3" };
        sut.Add(entity);

        var result = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));

        result.Should().NotBeNull();
        result.Id.Should().Be(id);

        result.Should().NotBeSameAs(entity);
    }

    [TestMethod]
    public void Update_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var sut = new DataverseStub();
        var action = () => sut.Update(null);

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [TestMethod]
    public void Update_ThrowsObjectDoesNotExistFault_WhenEntityDoesNotExist()
    {
        var sut = new DataverseStub();
        var entity = new Account(Guid.NewGuid());

        var action = () => sut.Update(entity);

        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }

    [TestMethod]
    public void Update_UpdatesEntityInStateDictionary()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary) };
        sut.Add(entity);

        var updatedEntity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary), Description = "Changed"};
        sut.Update(updatedEntity);

        sut.State[Account.EntityLogicalName].Should().ContainKey(id);
        sut.State[Account.EntityLogicalName][id].ToEntity<Account>().Description.Should().BeEquivalentTo(updatedEntity.Description);
        sut.State[Account.EntityLogicalName][id].Should().NotBeSameAs(updatedEntity);
    }

    [TestMethod]
    public void Delete_WithValidEntityNameAndId_RemovesRecord()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithValidEntityNameAndId_RemovesRecord) });
        sut.Delete(Account.EntityLogicalName, id);

        sut.State[Account.EntityLogicalName].Should().NotContainKey(id);
    }

    [TestMethod]
    public void Delete_WithNullEntityName_ThrowsFault()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNullEntityName_ThrowsFault) });


        var action = () => sut.Delete(null, id);
        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.InvalidArgument);
    }

    [TestMethod]
    public void Delete_WithUnknownEntityName_ThrowsFault()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithUnknownEntityName_ThrowsFault) });


        var action = () => sut.Delete("invalid", id);
        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [TestMethod]
    public void Delete_WithNonExistentId_ThrowsFault()
    {
        var sut = new DataverseStub();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNonExistentId_ThrowsFault) });


        var action = () => sut.Delete(Account.EntityLogicalName, Guid.NewGuid());
        action.Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.ObjectDoesNotExist);
    }
}
