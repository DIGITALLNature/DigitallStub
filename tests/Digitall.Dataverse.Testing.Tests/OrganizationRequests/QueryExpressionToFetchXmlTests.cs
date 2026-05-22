// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

public class QueryExpressionToFetchXmlTests
{
    [Test]
    public async Task Execute_Should_Convert_Simple_QueryExpression_To_FetchXml()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new QueryExpressionToFetchXmlFake());

        var query = new QueryExpression("account")
        {
            ColumnSet = new ColumnSet("name", "accountnumber"),
            Criteria = new FilterExpression(LogicalOperator.And)
            {
                Conditions = { new ConditionExpression("name", ConditionOperator.Equal, "Contoso") }
            }
        };

        var response = (QueryExpressionToFetchXmlResponse)sut.Execute(new QueryExpressionToFetchXmlRequest { Query = query });

        await Assert.That(response).IsNotNull();
        var fetchXml = response.FetchXml;
        await Assert.That(string.IsNullOrWhiteSpace(fetchXml)).IsFalse();

        var doc = XDocument.Parse(fetchXml);
        await Assert.That(doc.Root!.Name.LocalName).IsEqualTo("fetch");

        var entity = doc.Root.Element("entity")!;
        await Assert.That(entity.Attribute("name")!.Value).IsEqualTo("account");

        var attributes = entity.Elements("attribute").Select(a => a.Attribute("name")!.Value).ToList();
        await Assert.That(attributes).Contains("name");
        await Assert.That(attributes).Contains("accountnumber");

        var condition = entity.Element("filter")!.Element("condition")!;
        await Assert.That(condition.Attribute("attribute")!.Value).IsEqualTo("name");
        await Assert.That(condition.Attribute("operator")!.Value).IsEqualTo("eq");
        await Assert.That(condition.Attribute("value")!.Value).IsEqualTo("Contoso");
    }

    [Test]
    public async Task Execute_Should_Render_AllAttributes_When_ColumnSet_AllColumns()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new QueryExpressionToFetchXmlFake());

        var query = new QueryExpression("contact") { ColumnSet = new ColumnSet(true) };

        var response = (QueryExpressionToFetchXmlResponse)sut.Execute(new QueryExpressionToFetchXmlRequest { Query = query });

        var doc = XDocument.Parse(response.FetchXml);
        await Assert.That(doc.Root!.Element("entity")!.Element("all-attributes")).IsNotNull();
    }

    [Test]
    public async Task Execute_Should_Render_Orders_Distinct_TopCount_And_LinkEntities()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new QueryExpressionToFetchXmlFake());

        var query = new QueryExpression("account")
        {
            ColumnSet = new ColumnSet("name"),
            Distinct = true,
            TopCount = 50,
            Orders = { new OrderExpression("name", OrderType.Descending) }
        };
        var link = new LinkEntity("account", "contact", "primarycontactid", "contactid", JoinOperator.LeftOuter)
        {
            EntityAlias = "pc",
            Columns = new ColumnSet("fullname")
        };
        query.LinkEntities.Add(link);

        var response = (QueryExpressionToFetchXmlResponse)sut.Execute(new QueryExpressionToFetchXmlRequest { Query = query });

        var doc = XDocument.Parse(response.FetchXml);
        await Assert.That(doc.Root!.Attribute("distinct")!.Value).IsEqualTo("true");
        await Assert.That(doc.Root.Attribute("top")!.Value).IsEqualTo("50");

        var entity = doc.Root.Element("entity")!;
        var order = entity.Element("order")!;
        await Assert.That(order.Attribute("attribute")!.Value).IsEqualTo("name");
        await Assert.That(order.Attribute("descending")!.Value).IsEqualTo("true");

        var linkEl = entity.Element("link-entity")!;
        await Assert.That(linkEl.Attribute("name")!.Value).IsEqualTo("contact");
        await Assert.That(linkEl.Attribute("from")!.Value).IsEqualTo("contactid");
        await Assert.That(linkEl.Attribute("to")!.Value).IsEqualTo("primarycontactid");
        await Assert.That(linkEl.Attribute("alias")!.Value).IsEqualTo("pc");
        await Assert.That(linkEl.Attribute("link-type")!.Value).IsEqualTo("outer");
    }

    [Test]
    public void Execute_Should_Throw_For_NonQueryExpression()
    {
        var sut = new FakeOrganizationService();
        sut.AddRequest(new QueryExpressionToFetchXmlFake());

        Assert.Throws<ArgumentException>(() =>
            sut.Execute(new QueryExpressionToFetchXmlRequest { Query = new FetchExpression("<fetch/>") }));
    }
}
