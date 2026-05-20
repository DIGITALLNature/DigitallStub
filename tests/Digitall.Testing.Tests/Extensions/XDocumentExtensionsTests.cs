// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests.Extensions;

public class XDocumentExtensionsTests
{
    [Test]
    public async Task IsAggregateFetchXml_Should_ReturnTrue_When_AggregateAttributeIsTrue()
    {
        var xml = "<fetch aggregate='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.IsAggregateFetchXml()).IsTrue();
    }

    [Test]
    public async Task IsDistinctFetchXml_Should_ReturnTrue_When_DistinctAttributeIsTrue()
    {
        var xml = "<fetch distinct='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.IsDistinctFetchXml()).IsTrue();
    }

    [Test]
    public async Task ToColumnSet_Should_ReturnRequestedColumns()
    {
        var xml = "<fetch><entity name='account'><attribute name='name'/><attribute name='accountid'/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var columnSet = doc.ToColumnSet();

        await Assert.That(columnSet!.Columns.Contains("name")).IsTrue();
        await Assert.That(columnSet.Columns.Contains("accountid")).IsTrue();
        await Assert.That(columnSet.AllColumns).IsFalse();
    }

    [Test]
    public async Task ToColumnSet_Should_ReturnAllColumns_When_AllAttributesNodeExists()
    {
        var xml = "<fetch><entity name='account'><all-attributes/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var columnSet = doc.ToColumnSet();

        await Assert.That(columnSet!.AllColumns).IsTrue();
    }

    [Test]
    public async Task ToCount_Should_ReturnCountValue()
    {
        var xml = "<fetch count='50'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.ToCount()).IsEqualTo(50);
    }

    [Test]
    public async Task ToOrderExpressionList_Should_ReturnOrderExpressions()
    {
        var xml = "<fetch><entity name='account'><order attribute='name' descending='true'/><order attribute='createdon'/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var orders = doc.ToOrderExpressionList();

        await Assert.That(orders.Count).IsEqualTo(2);
        await Assert.That(orders[0].AttributeName).IsEqualTo("name");
        await Assert.That(orders[0].OrderType).IsEqualTo(OrderType.Descending);
        await Assert.That(orders[1].AttributeName).IsEqualTo("createdon");
        await Assert.That(orders[1].OrderType).IsEqualTo(OrderType.Ascending);
    }

    [Test]
    public async Task ToPageNumber_Should_ReturnPageValue()
    {
        var xml = "<fetch page='2'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.ToPageNumber()).IsEqualTo(2);
    }

    [Test]
    public async Task ToReturnTotalRecordCount_Should_ReturnTrue_When_AttributeIsTrue()
    {
        var xml = "<fetch returntotalrecordcount='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.ToReturnTotalRecordCount()).IsTrue();
    }

    [Test]
    public async Task ToTopCount_Should_ReturnTopValue()
    {
        var xml = "<fetch top='5'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        await Assert.That(doc.ToTopCount()).IsEqualTo(5);
    }

    [Test]
    public async Task IsFetchXmlNodeValid_Should_ReturnTrue_ForValidNodes()
    {
        await Assert.That(new XElement("filter").IsFetchXmlNodeValid()).IsTrue();
        await Assert.That(new XElement("entity", new XAttribute("name", "account")).IsFetchXmlNodeValid()).IsTrue();
        await Assert.That(new XElement("attribute", new XAttribute("name", "name")).IsFetchXmlNodeValid()).IsTrue();
        await Assert.That(new XElement("link-entity", new XAttribute("name", "contact"), new XAttribute("from", "contactid"), new XAttribute("to", "primarycontactid")).IsFetchXmlNodeValid()).IsTrue();
        await Assert.That(new XElement("condition", new XAttribute("attribute", "name"), new XAttribute("operator", "eq")).IsFetchXmlNodeValid()).IsTrue();
    }

    [Test]
    public async Task IsFetchXmlNodeValid_Should_Throw_ForInvalidNode()
    {
        var ex = Assert.Throws<Exception>(() => { new XElement("invalid").IsFetchXmlNodeValid(); });
        await Assert.That(ex).IsNotNull();
    }
}
