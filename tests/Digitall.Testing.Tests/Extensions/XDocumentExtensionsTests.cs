// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using System.Xml.Linq;
using AwesomeAssertions;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Tests.Extensions;

[TestClass]
public class XDocumentExtensionsTests
{
    [TestMethod]
    public void IsAggregateFetchXml_Should_ReturnTrue_When_AggregateAttributeIsTrue()
    {
        var xml = "<fetch aggregate='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.IsAggregateFetchXml().Should().BeTrue();
    }

    [TestMethod]
    public void IsDistinctFetchXml_Should_ReturnTrue_When_DistinctAttributeIsTrue()
    {
        var xml = "<fetch distinct='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.IsDistinctFetchXml().Should().BeTrue();
    }

    [TestMethod]
    public void ToColumnSet_Should_ReturnRequestedColumns()
    {
        var xml = "<fetch><entity name='account'><attribute name='name'/><attribute name='accountid'/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var columnSet = doc.ToColumnSet();

        columnSet.Columns.Should().Contain("name", "accountid");
        columnSet.AllColumns.Should().BeFalse();
    }

    [TestMethod]
    public void ToColumnSet_Should_ReturnAllColumns_When_AllAttributesNodeExists()
    {
        var xml = "<fetch><entity name='account'><all-attributes/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var columnSet = doc.ToColumnSet();

        columnSet.AllColumns.Should().BeTrue();
    }

    [TestMethod]
    public void ToCount_Should_ReturnCountValue()
    {
        var xml = "<fetch count='50'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.ToCount().Should().Be(50);
    }

    [TestMethod]
    public void ToOrderExpressionList_Should_ReturnOrderExpressions()
    {
        var xml = "<fetch><entity name='account'><order attribute='name' descending='true'/><order attribute='createdon'/></entity></fetch>";
        var doc = XDocument.Parse(xml);

        var orders = doc.ToOrderExpressionList();

        orders.Count.Should().Be(2);
        orders[0].AttributeName.Should().Be("name");
        orders[0].OrderType.Should().Be(OrderType.Descending);
        orders[1].AttributeName.Should().Be("createdon");
        orders[1].OrderType.Should().Be(OrderType.Ascending);
    }

    [TestMethod]
    public void ToPageNumber_Should_ReturnPageValue()
    {
        var xml = "<fetch page='2'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.ToPageNumber().Should().Be(2);
    }

    [TestMethod]
    public void ToReturnTotalRecordCount_Should_ReturnTrue_When_AttributeIsTrue()
    {
        var xml = "<fetch returntotalrecordcount='true'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.ToReturnTotalRecordCount().Should().BeTrue();
    }

    [TestMethod]
    public void ToTopCount_Should_ReturnTopValue()
    {
        var xml = "<fetch top='5'><entity name='account'></entity></fetch>";
        var doc = XDocument.Parse(xml);

        doc.ToTopCount().Should().Be(5);
    }

    [TestMethod]
    public void IsFetchXmlNodeValid_Should_ReturnTrue_ForValidNodes()
    {
        new XElement("filter").IsFetchXmlNodeValid().Should().BeTrue();
        new XElement("entity", new XAttribute("name", "account")).IsFetchXmlNodeValid().Should().BeTrue();
        new XElement("attribute", new XAttribute("name", "name")).IsFetchXmlNodeValid().Should().BeTrue();
        new XElement("link-entity", new XAttribute("name", "contact"), new XAttribute("from", "contactid"), new XAttribute("to", "primarycontactid")).IsFetchXmlNodeValid().Should().BeTrue();
        new XElement("condition", new XAttribute("attribute", "name"), new XAttribute("operator", "eq")).IsFetchXmlNodeValid().Should().BeTrue();
    }

    [TestMethod]
    public void IsFetchXmlNodeValid_Should_Throw_ForInvalidNode()
    {
        var action = () => new XElement("invalid").IsFetchXmlNodeValid();
        action.Should().Throw<Exception>();
    }
}
