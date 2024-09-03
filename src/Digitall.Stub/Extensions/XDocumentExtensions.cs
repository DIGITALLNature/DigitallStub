// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub;

public static class XDocumentExtensions
{
    public static XAttribute GetAttribute(this XElement elem, string sAttributeName) => elem.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(sAttributeName));

    public static bool IsAggregateFetchXml(this XDocument xDocument) => xDocument.Root.IsAttributeTrue("aggregate");

    public static bool IsAttributeTrue(this XElement xElement, string attributeName)
    {
        var val = xElement.GetAttribute(attributeName)?.Value;

        return "true".Equals(val, StringComparison.InvariantCultureIgnoreCase)
               || "1".Equals(val, StringComparison.InvariantCultureIgnoreCase);
    }

    public static bool IsDistincFetchXml(this XDocument xDocument) => xDocument.Root.IsAttributeTrue("distinct");

    public static ColumnSet ToColumnSet(this XDocument xDocument)
    {
        Debug.Assert(xDocument != null, nameof(xDocument) + " != null");
        return xDocument.Elements() //fetch
            .Elements()
            .FirstOrDefault()
            .ToColumnSet();
    }

    public static ColumnSet ToColumnSet(this XElement el)
    {
        var allAttributes = el.Elements()
            .Where(e => e.Name.LocalName.Equals("all-attributes"))
            .FirstOrDefault();

        if (allAttributes != null)
        {
            return new ColumnSet(true);
        }

        var attributes = el.Elements()
            .Where(e => e.Name.LocalName.Equals("attribute"))
            .Select(e => e.GetAttribute("name").Value)
            .ToArray();


        return new ColumnSet(attributes);
    }

    public static int? ToCount(this XElement el)
    {
        var countAttr = el.GetAttribute("count");
        if (countAttr == null)
        {
            return null;
        }

        int iCount;
        if (!int.TryParse(countAttr.Value, out iCount))
        {
            throw new Exception("Count attribute in fetch node must be an integer");
        }

        return iCount;
    }

    public static int? ToCount(this XDocument xlDoc) =>
        //Check if all-attributes exist
        xlDoc.Elements() //fetch
            .FirstOrDefault()
            .ToCount();

    public static List<OrderExpression> ToOrderExpressionList(this XDocument xlDoc)
    {
        var orderByElements = xlDoc.Elements() //fetch
            .Elements() //entity
            .Elements() //child nodes of entity
            .Where(el => el.Name.LocalName.Equals("order"))
            .Select(el =>
                new OrderExpression
                {
                    AttributeName = el.GetAttribute("attribute").Value,
                    OrderType = el.IsAttributeTrue("descending") ? OrderType.Descending : OrderType.Ascending
                })
            .ToList();

        return orderByElements;
    }

    public static int? ToPageNumber(this XElement el)
    {
        var pageAttr = el.GetAttribute("page");
        if (pageAttr == null)
        {
            return null;
        }

        int iPage;
        if (!int.TryParse(pageAttr.Value, out iPage))
        {
            throw new Exception("Count attribute in fetch node must be an integer");
        }

        return iPage;
    }


    public static int? ToPageNumber(this XDocument xlDoc) =>
        //Check if all-attributes exist
        xlDoc.Elements() //fetch
            .FirstOrDefault()
            .ToPageNumber();


    public static bool ToReturnTotalRecordCount(this XElement el)
    {
        var returnTotalRecordCountAttr = el.GetAttribute("returntotalrecordcount");
        if (returnTotalRecordCountAttr == null)
        {
            return false;
        }

        bool bReturnCount;
        if (!bool.TryParse(returnTotalRecordCountAttr.Value, out bReturnCount))
        {
            throw new Exception("returntotalrecordcount attribute in fetch node must be an boolean");
        }

        return bReturnCount;
    }

    public static bool ToReturnTotalRecordCount(this XDocument xlDoc) =>
        xlDoc.Elements() //fetch
            .FirstOrDefault()
            .ToReturnTotalRecordCount();

    public static int? ToTopCount(this XElement el)
    {
        var countAttr = el.GetAttribute("top");
        if (countAttr == null)
        {
            return null;
        }

        int iCount;
        if (!int.TryParse(countAttr.Value, out iCount))
        {
            throw new Exception("Top attribute in fetch node must be an integer");
        }

        return iCount;
    }


    public static int? ToTopCount(this XDocument xlDoc) =>
        //Check if all-attributes exist
        xlDoc.Elements() //fetch
            .FirstOrDefault()
            .ToTopCount();

    public static bool IsFetchXmlNodeValid(this XElement elem)
    {
        switch (elem.Name.LocalName)
        {
            case "filter":
                return true;

            case "value":
            case "fetch":
                return true;

            case "entity":
                return elem.GetAttribute("name") != null;

            case "all-attributes":
                return true;

            case "attribute":
                return elem.GetAttribute("name") != null;

            case "link-entity":
                return elem.GetAttribute("name") != null
                       && elem.GetAttribute("from") != null
                       && elem.GetAttribute("to") != null;

            case "order":
                if (elem.Document.IsAggregateFetchXml())
                {
                    return elem.GetAttribute("alias") != null
                           && elem.GetAttribute("attribute") == null;
                }
                else
                {
                    return elem.GetAttribute("attribute") != null;
                }

            case "condition":
                return elem.GetAttribute("attribute") != null
                       && elem.GetAttribute("operator") != null;

            default:
                throw new Exception(string.Format("Node {0} is not a valid FetchXml node or it doesn't have the required attributes", elem.Name.LocalName));
        }
    }

}
