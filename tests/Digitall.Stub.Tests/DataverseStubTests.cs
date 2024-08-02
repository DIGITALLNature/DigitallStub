using System.ServiceModel;
using Digitall.Stub.Errors;
using Digitall.Stub.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Tests;

[TestClass]
public class DataverseStubTests
{
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
        // Arrange
        var stub = new DataverseStub();

        stub.Invoking(s => s.ThrowIfNotKnownEntityType("unknownEntity")).Should().Throw<FaultException<OrganizationServiceFault>>()
            .And.Detail.ErrorCode.Should().Be((int)ErrorCodes.QueryBuilderNoEntity);

    }

    [TestMethod]
    public void ThrowIfNotKnownEntityType_DoesNotThrow_WhenEntityTypeIsKnown()
    {
        var stub = new DataverseStub();
        stub.Invoking(s => s.ThrowIfNotKnownEntityType(Account.EntityLogicalName)).Should().NotThrow();
    }
}
