// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests proving that aggregate FetchXml validation guards are broken:
/// They check string literals ("alias"/"name") instead of the actual computed values,
/// so missing alias/name attributes slip through without error.
/// </summary>
public class AggregateFetchXmlValidationTests
{
    private static FakeOrganizationService BuildSut()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);
        return sut;
    }

    [Test]
    public async Task AggregateFetchXml_MissingAlias_ShouldThrowFault()
    {
        // An aggregate attribute without an alias should be rejected.
        // BUG: The guard checks string.IsNullOrEmpty("alias") instead of the variable,
        // so this does NOT throw — the query proceeds and produces a broken result.
        var sut = BuildSut();

        const string fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="name" aggregate="count" />
              </entity>
            </fetch>
            """;

        void Action() => sut.RetrieveMultiple(new FetchExpression(fetchXml));

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(Action);
        await Assert.That(ex.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.QueryBuilderInvalidAlias);
    }

    [Test]
    public async Task AggregateFetchXml_MissingName_ShouldThrowAtXmlValidation()
    {
        // An aggregate attribute without a name is caught by XML validation
        // (IsFetchXmlNodeValid requires "name" on <attribute> elements).
        // This test documents that the XML validator catches it before aggregate processing.
        var sut = BuildSut();

        const string fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute alias="cnt" aggregate="count" />
              </entity>
            </fetch>
            """;

        void Action() => sut.RetrieveMultiple(new FetchExpression(fetchXml));

        var ex = Assert.Throws<Exception>(Action);
        await Assert.That(ex.Message).IsEqualTo("At least some node is not valid");
    }

    [Test]
    public async Task AggregateFetchXml_OrderWithoutAlias_ShouldThrowAtXmlValidation()
    {
        // An order element without alias in aggregate queries is caught by XML validation
        // (IsFetchXmlNodeValid requires "alias" and no "attribute" on <order> in aggregate).
        // This test documents that the XML validator catches it before aggregate processing.
        var sut = BuildSut();

        const string fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="name" alias="cnt" aggregate="count" />
                <order attribute="name" />
              </entity>
            </fetch>
            """;

        void Action() => sut.RetrieveMultiple(new FetchExpression(fetchXml));

        var ex = Assert.Throws<Exception>(Action);
        await Assert.That(ex.Message).IsEqualTo("At least some node is not valid");
    }
}
