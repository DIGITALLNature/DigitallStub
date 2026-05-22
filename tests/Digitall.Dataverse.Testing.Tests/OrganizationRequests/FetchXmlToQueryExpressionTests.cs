// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class FetchXmlToQueryExpressionTests
{
    [Test]
    public async Task Execute_Should_Convert_Simple_FetchXml_To_QueryExpression()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="name" />
                <attribute name="accountnumber" />
                <filter type="and">
                  <condition attribute="name" operator="eq" value="Contoso" />
                </filter>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        await Assert.That(response).IsNotNull();
        var query = response.Query;
        await Assert.That(query).IsNotNull();
        await Assert.That(query.EntityName).IsEqualTo("account");
        await Assert.That(query.ColumnSet.Columns).Contains("name");
        await Assert.That(query.ColumnSet.Columns).Contains("accountnumber");
        await Assert.That(query.Criteria.Conditions.Count).IsEqualTo(1);
        await Assert.That(query.Criteria.Conditions[0].AttributeName).IsEqualTo("name");
        await Assert.That(query.Criteria.Conditions[0].Operator).IsEqualTo(ConditionOperator.Equal);
        await Assert.That(query.Criteria.Conditions[0].Values[0]).IsEqualTo("Contoso");
    }

    [Test]
    public async Task Execute_Should_Convert_LinkEntity_And_Distinct()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        const string fetchXml = """
            <fetch distinct="true" top="50">
              <entity name="account">
                <attribute name="name" />
                <link-entity name="contact" from="contactid" to="primarycontactid" alias="pc" link-type="outer">
                  <attribute name="fullname" />
                </link-entity>
              </entity>
            </fetch>
            """;

        var response = (FetchXmlToQueryExpressionResponse)sut.Execute(new FetchXmlToQueryExpressionRequest { FetchXml = fetchXml });

        var query = response.Query;
        await Assert.That(query.Distinct).IsTrue();
        await Assert.That(query.TopCount).IsEqualTo(50);
        await Assert.That(query.LinkEntities.Count).IsEqualTo(1);

        var link = query.LinkEntities[0];
        await Assert.That(link.LinkToEntityName).IsEqualTo("contact");
        await Assert.That(link.LinkToAttributeName).IsEqualTo("contactid");
        await Assert.That(link.LinkFromAttributeName).IsEqualTo("primarycontactid");
        await Assert.That(link.EntityAlias).IsEqualTo("pc");
        await Assert.That(link.JoinOperator).IsEqualTo(JoinOperator.LeftOuter);
    }

    [Test]
    public void Execute_Should_Throw_For_Empty_FetchXml()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        Assert.Throws<ArgumentException>(() =>
            sut.Execute(new FetchXmlToQueryExpressionRequest { FetchXml = string.Empty }));
    }

    [Test]
    public void Execute_Should_Throw_For_Invalid_Xml()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new FetchXmlToQueryExpressionFake());

        Assert.Throws<ArgumentException>(() =>
            sut.Execute(new FetchXmlToQueryExpressionRequest { FetchXml = "not-xml<<>" }));
    }
}
