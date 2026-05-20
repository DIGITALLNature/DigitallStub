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
        await Assert.That(sut.State.ModelAssemblies).IsNotNull();
        await Assert.That(sut.State.ModelAssemblies).IsNotEmpty();
        await Assert.That(sut.State.ModelAssemblies.Any(a => a.FullName == typeof(TestData).Assembly.FullName)).IsTrue();
    }

    [Test]
    public async Task ModelIsSeeded()
    {
        var sut = new FakeOrganizationService();

        sut.AddRange(TestData.Default);

        var accounts = sut.RetrieveMultiple(new QueryExpression(Account.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        var contacts = sut.RetrieveMultiple(new QueryExpression(Contact.EntityLogicalName) { ColumnSet = new ColumnSet(true) });
        await Assert.That(accounts.Entities).Count().IsEqualTo(2);
        await Assert.That(contacts.Entities).Count().IsEqualTo(3);
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
        await Assert.That(attributeInfo.PropertyType.FullName).IsEqualTo(typeof(EntityReference).FullName);
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
        void Action() => dataverse.ThrowIfNotKnownEntityType("unknownEntity");

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public async Task ThrowIfNotKnownEntityType_DoesNotThrow_WhenEntityTypeIsKnown()
    {
        var dataverse = new FakeOrganizationService();
        void Action() => dataverse.ThrowIfNotKnownEntityType(Account.EntityLogicalName);

        await Assert.That(Action).ThrowsNothing();
    }

    [Test]
    public async Task ThrowIfNotKnownAttribute_ThrowsFaultException_WhenAttributeIsNotKnown()
    {
        var sut = new FakeOrganizationService();
        void Action() => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, "non_existing");

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoAttribute);
    }

    [Test]
    public async Task ThrowIfNotKnownAttribute_DoesNotThrow_WhenAttributeIsKnown()
    {
        var sut = new FakeOrganizationService();
        void Action() => sut.ThrowIfNotKnownAttribute(Account.EntityLogicalName, Account.LogicalNames.TransactionCurrencyId);

        await Assert.That(Action).ThrowsNothing();
    }

    [Test]
    public async Task Create_ThrowsInvalidArgumentFault_WhenEntityIsNull()
    {
        var dataverse = new FakeOrganizationService();
        void Action() => dataverse.Create(null!);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account { Name = nameof(Create_ReturnsNonEmptyId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        await Assert.That(result).IsNotEqualTo(Guid.Empty);
        var retrieved = sut.Retrieve(Account.EntityLogicalName, result, new ColumnSet(true));
        await Assert.That(retrieved).IsNotNull();
    }

    [Test]
    public async Task Create_ClonesInput_MutatingOriginalDoesNotAffectStore()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account { Name = "Original" };

        var id = sut.Create(entity);
        entity.Name = "Mutated";

        var retrieved = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved.ToEntity<Account>().Name).IsEqualTo("Original");
    }

    [Test]
    public async Task Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ReturnsGivenId_WhenEntityIsAddedSuccessfully) };

        var result = sut.Create(entity);

        await Assert.That(result).IsEqualTo(id);
        var retrieved = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved).IsNotNull();
    }

    [Test]
    public async Task Create_ThrowsFaultException_WhenEntityIdIsDuplicate()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account (id) { Name = nameof(Create_ThrowsFaultException_WhenEntityIdIsDuplicate) };

        void ActionOne() => sut.Create(entity);
        void ActionTwo() => sut.Create(entity);
        await Assert.That(ActionOne).ThrowsNothing();
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(ActionTwo);
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

        void Action() => sut.Retrieve(Account.EntityLogicalName, Guid.Parse("10000000-0000-0000-0000-000000000000"), new ColumnSet(true));

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
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
        void Action() => sut.Update(null!);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Update_ThrowsObjectDoesNotExistFault_WhenEntityDoesNotExist()
    {
        var sut = new FakeOrganizationService();
        var entity = new Account(Guid.NewGuid());

        void Action() => sut.Update(entity);

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Update_UpdatesEntity()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        var entity = new Account(id) { Name = nameof(Update_UpdatesEntity) };
        sut.Add(entity);

        var updatedEntity = new Account(id) { Name = nameof(Update_UpdatesEntity), Description = "Changed"};
        sut.Update(updatedEntity);

        var retrieved = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved.ToEntity<Account>().Description).IsEquivalentTo("Changed");
    }

    [Test]
    public async Task Update_ClonesInput_MutatingOriginalDoesNotAffectStore()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = "Initial" });

        var updatedEntity = new Account(id) { Description = "Updated" };
        sut.Update(updatedEntity);
        updatedEntity.Description = "Tampered";

        var retrieved = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(retrieved.ToEntity<Account>().Description).IsEquivalentTo("Updated");
    }

    [Test]
    public async Task Retrieve_ReturnsClone_MutatingResultDoesNotAffectStore()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = "Immutable" });

        var r1 = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        r1["name"] = "Tampered";

        var r2 = sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        await Assert.That(r2.ToEntity<Account>().Name).IsEqualTo("Immutable");
        await Assert.That(r1).IsNotSameReferenceAs(r2);
    }

    [Test]
    public async Task Delete_WithValidEntityNameAndId_RemovesRecord()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithValidEntityNameAndId_RemovesRecord) });
        sut.Delete(Account.EntityLogicalName, id);

        void Action() => sut.Retrieve(Account.EntityLogicalName, id, new ColumnSet(true));
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task Delete_WithNullEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNullEntityName_ThrowsFault) });

        void Action() => sut.Delete(null!, id);
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.InvalidArgument);
    }

    [Test]
    public async Task Delete_WithUnknownEntityName_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithUnknownEntityName_ThrowsFault) });

        void Action() => sut.Delete("invalid", id);
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderNoEntity);
    }

    [Test]
    public async Task Delete_WithNonExistentId_ThrowsFault()
    {
        var sut = new FakeOrganizationService();
        var id = Guid.NewGuid();
        sut.Add(new Account(id) { Name = nameof(Delete_WithNonExistentId_ThrowsFault) });

        void Action() => sut.Delete(Account.EntityLogicalName, Guid.NewGuid());
        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task AddDefaultRequests()
    {
        var sut = new FakeOrganizationService();
        sut.AddDefaultRequests();

        var createRequest = new CreateRequest { Target = new Entity("account") { Id = Guid.NewGuid() } };
        void Action() => sut.Execute(createRequest);
        await Assert.That(Action).ThrowsNothing();
    }
}
