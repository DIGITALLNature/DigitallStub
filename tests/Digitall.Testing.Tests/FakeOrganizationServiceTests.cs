using System.ServiceModel;
using Digitall.Testing.Errors;
using Digitall.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests;

public class FakeOrganizationServiceTests
{
    [Before(Class)]
    public static async Task MyClassInitialize()
    {
        DotNetEnv.Env.Load();
        await Task.CompletedTask;
    }

    [Test]
    public async Task ModelIsDetected()
    {
        var sut = new FakeOrganizationService();
        await Assert.That(sut.ModelAssemblies).IsNotNull();
        await Assert.That(sut.ModelAssemblies).IsNotEmpty();
        await Assert.That(sut.ModelAssemblies.Any(a => a.FullName == typeof(TestData).Assembly.FullName)).IsTrue();
    }

    [Test]
    public async Task ModelIsSeeded()
    {
        var sut = new FakeOrganizationService();
        await Assert.That(sut.ServiceState).IsNotNull();
        await Assert.That(sut.ServiceState).IsEmpty();

        sut.AddRange(TestData.Default);
        await Assert.That(sut.ServiceState).IsNotEmpty();
        await Assert.That(sut.ServiceState.ContainsKey(Account.EntityLogicalName)).IsTrue();
        await Assert.That(sut.ServiceState.ContainsKey(Contact.EntityLogicalName)).IsTrue();
        await Assert.That(sut.ServiceState[Account.EntityLogicalName]).Count().IsEqualTo(2);
        await Assert.That(sut.ServiceState[Contact.EntityLogicalName]).Count().IsEqualTo(3);
    }

    [Test]
    public async Task EntityTypeIsKnown_ReturnsTrue_WhenEntityTypeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.EntityTypeIsKnown(Account.EntityLogicalName, out var knownEntityType);

        await Assert.That(result).IsTrue();
        await Assert.That(knownEntityType).IsNotNull();
        await Assert.That(knownEntityType).IsEqualTo(typeof(Account));
    }

    [Test]
    public async Task EntityTypeIsKnown_ReturnsFalse_WhenEntityTypeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.EntityTypeIsKnown("non_existing", out var knownEntityType);

        await Assert.That(result).IsFalse();
        await Assert.That(knownEntityType).IsNull();
    }

    [Test]
    public async Task IsKnownAttributeForType_ReturnsTrue_WhenAttributeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId, out var attributeInfo);

        await Assert.That(result).IsTrue();
        await Assert.That(attributeInfo).IsNotNull();
        await Assert.That(attributeInfo!.PropertyType.FullName).IsEqualTo(typeof(EntityReference).FullName);
    }

    [Test]
    public async Task IsKnownAttributeForType_ReturnsFalse_WhenAttributeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var result = sut.IsKnownAttributeForType(Account.EntityLogicalName, "non_existing", out var attributeInfo);

        await Assert.That(result).IsFalse();
        await Assert.That(attributeInfo).IsNull();
    }

    [Test]
    public async Task ThrowIfNotKnownEntityType_ThrowsArgumentException_WhenEntityTypeIsNotKnown()
    {
        var dataverse = new FakeOrganizationService();
        var action = () => dataverse.ThrowIfNotKnownEntityType("unknownEntity");

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public async Task ThrowIfNotKnownEntityType_DoesNotThrow_WhenEntityTypeIsKnown()
    {
        var dataverse = new FakeOrganizationService();
        var action = () => dataverse.ThrowIfNotKnownEntityType(Account.EntityLogicalName);

        await Assert.That(action).ThrowsNothing();
    }

    [Test]
    public async Task ThrowIfNotKnownAttribute_ThrowsFaultException_WhenAttributeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, "non_existing");

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoAttribute);
    }

    [Test]
    public async Task ThrowIfNotKnownAttribute_DoesNotThrow_WhenAttributeIsKnown()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId);

        await Assert.That(action).ThrowsNothing();
    }

    [Test]
    public async Task Create_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var dataverse = new FakeOrganizationService();
        Action action = () => dataverse.Create(null!);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account { Name = nameof(Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        await Assert.That(result).IsNotEqualTo(Guid.Empty);
        await Assert.That(sut.ServiceState.ContainsKey(Account.EntityLogicalName)).IsTrue();
        await Assert.That(sut.ServiceState[Account.EntityLogicalName].ContainsKey(result)).IsTrue();
        await Assert.That(sut.ServiceState[Account.EntityLogicalName][result]).IsNotSameReferenceAs(entity);
    }

    [Test]
    public async Task Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        await Assert.That(result).IsEqualTo(id);
        await Assert.That(sut.ServiceState.ContainsKey(Account.EntityLogicalName)).IsTrue();
        await Assert.That(sut.ServiceState[Account.EntityLogicalName].ContainsKey(id)).IsTrue();
    }

    [Test]
    public async Task Create_ThrowsFaultException_WhenEntityIdIsDuplicate()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ThrowsFaultException_WhenEntityIdIsDuplicate) };

        Action actionOne = () => sut.Create(entity);
        Action actionTwo = () => sut.Create(entity);
        await Assert.That(actionOne).ThrowsNothing();
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(actionTwo);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.DuplicateRecord);
    }

    [Test]
    public async Task Retrieve_EntityExists_ReturnsRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.Retrieve(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001"), new ColumnSet(true));

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsEqualTo(Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var resultAcc = result.ToEntity<Account>();
        await Assert.That(resultAcc).IsNotNull();
        await Assert.That(resultAcc.Name).IsEqualTo("A Corp");
        await Assert.That(resultAcc.Telephone1).IsEqualTo("1");
        await Assert.That(resultAcc.Telephone2).IsEqualTo("2");
        await Assert.That(resultAcc.Telephone3).IsNull();
    }

    [Test]
    public async Task Retrieve_EntityExists_withColumnSet_ReturnsRecord()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var result = sut.Retrieve(Account.EntityLogicalName, Guid.Parse("00000000-0000-0000-0001-000000000001"), new ColumnSet(
            Account.LogicalNames.Name,
            Account.LogicalNames.Telephone1
            ));

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsEqualTo(Guid.Parse("00000000-0000-0000-0001-000000000001"));

        var resultAcc = result.ToEntity<Account>();
        await Assert.That(resultAcc).IsNotNull();
        await Assert.That(resultAcc.Name).IsEqualTo("A Corp");
        await Assert.That(resultAcc.Telephone1).IsEqualTo("1");
        await Assert.That(resultAcc.Telephone2).IsNull();
        await Assert.That(resultAcc.Telephone3).IsNull();
    }

    [Test]
    public async Task Retrieve_EntityDoesNotExist_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        Action action = () =>  sut.Retrieve(Account.EntityLogicalName, Guid.Parse("10000000-0000-0000-0000-000000000000"), new ColumnSet(true));

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Retrieve_EntityExists_ReturnsClonedRecord()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = "Inline Corp", Telephone1 = "1", Telephone2 = "2", Telephone3 = "3" };
        sut.Add(entity);

        var result = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Id).IsEqualTo(id);
        await Assert.That(result).IsNotSameReferenceAs(entity);
    }

    [Test]
    public async Task Update_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var sut = new FakeOrganizationService();
        var action = () => sut.Update(null!);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Update_ThrowsObjectDoesNotExistFault_WhenEntityDoesNotExist()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account(Guid.NewGuid());

        var action = () => sut.Update(entity);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Update_UpdatesEntityInStateDictionary()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary) };
        sut.Add(entity);

        var updatedEntity = new Account(id) { Name = nameof(Update_UpdatesEntityInStateDictionary), Description = "Changed"};
        sut.Update(updatedEntity);

        await Assert.That(sut.ServiceState[Account.EntityLogicalName].ContainsKey(id)).IsTrue();
        await Assert.That(sut.ServiceState[Account.EntityLogicalName][id].ToEntity<Account>().Description).IsEquivalentTo(updatedEntity.Description);
        await Assert.That(sut.ServiceState[Account.EntityLogicalName][id]).IsNotSameReferenceAs(updatedEntity);
    }

    [Test]
    public async Task Delete_WithValidEntityNameAndId_RemovesRecord()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithValidEntityNameAndId_RemovesRecord) });
        sut.Delete(Account.EntityLogicalName, id);

        await Assert.That(sut.ServiceState[Account.EntityLogicalName].ContainsKey(id)).IsFalse();
    }

    [Test]
    public async Task Delete_WithNullEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNullEntityName_ThrowsFault) });

        var action = () => sut.Delete(null!, id);
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Delete_WithUnknownEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithUnknownEntityName_ThrowsFault) });

        var action = () => sut.Delete("invalid", id);
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public async Task Delete_WithNonExistentId_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNonExistentId_ThrowsFault) });

        var action = () => sut.Delete(Account.EntityLogicalName, Guid.NewGuid());
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task AddDefaultRequests()
    {
        var sut = new FakeOrganizationService();
        sut.AddDefaultRequests();

        var createRequest = new CreateRequest { Target = new Entity("account") { Id = Guid.NewGuid() } };
        var action = () => sut.Execute(createRequest);
        await Assert.That(action).ThrowsNothing();
    }
}
