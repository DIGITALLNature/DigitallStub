// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Testing.Extensions;

public static class XDocumentExtensions
{
    public static bool IsAggregateFetchXml(this XDocument xDocument) => xDocument.Root?.IsAttributeTrue("aggregate") == true;

    extension(XDocument xDocument)
    {
        public bool IsDistinctFetchXml() => xDocument.Root?.IsAttributeTrue("distinct") == true;

        public ColumnSet? ToColumnSet()
        {
            ArgumentNullException.ThrowIfNull(xDocument);
            return xDocument.Elements() //fetch
                .Elements().FirstOrDefault()?.ToColumnSet();
        }
    }

    extension(XElement el)
    {
        public ColumnSet ToColumnSet()
        {
            var allAttributes = el.Elements().FirstOrDefault(e => e.Name.LocalName.Equals("all-attributes"));

            if (allAttributes != null)
            {
                return new ColumnSet(true);
            }

            var attributes = el.Elements().Where(e => e.Name.LocalName.Equals("attribute")).Select(e => e.GetAttribute("name")?.Value).ToArray();


            return new ColumnSet(attributes);
        }

        private int? ToCount()
        {
            var countAttr = el.GetAttribute("count");
            if (countAttr == null)
            {
                return null;
            }

            if (!int.TryParse(countAttr.Value, out var iCount))
            {
                throw new Exception("Count attribute in fetch node must be an integer");
            }

            return iCount;
        }

        public XAttribute? GetAttribute(string sAttributeName) => el.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(sAttributeName));

        public bool IsAttributeTrue(string attributeName)
        {
            var val = el.GetAttribute(attributeName)?.Value;

            return "true".Equals(val, StringComparison.InvariantCultureIgnoreCase) || "1".Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    extension(XDocument xlDoc)
    {
        public int? ToCount() =>
            //Check if all-attributes exist
            xlDoc.Elements() //fetch
                .FirstOrDefault()?.ToCount();

        public List<OrderExpression> ToOrderExpressionList()
        {
            var orderByElements = xlDoc.Elements() //fetch
                .Elements() //entity
                .Elements() //child nodes of entity
                .Where(el => el.Name.LocalName.Equals("order")).Select(el =>
                    new OrderExpression { AttributeName = el.GetAttribute("attribute")?.Value, OrderType = el.IsAttributeTrue("descending") ? OrderType.Descending : OrderType.Ascending }).ToList();

            return orderByElements;
        }
    }

    private static int? ToPageNumber(this XElement el)
    {
        var pageAttr = el.GetAttribute("page");
        if (pageAttr == null)
        {
            return null;
        }

        if (!int.TryParse(pageAttr.Value, out var iPage))
        {
            throw new Exception("Count attribute in fetch node must be an integer");
        }

        return iPage;
    }


    public static int? ToPageNumber(this XDocument xlDoc) =>
        //Check if all-attributes exist
        xlDoc.Elements() //fetch
            .FirstOrDefault()?.ToPageNumber();


    private static bool ToReturnTotalRecordCount(this XElement el)
    {
        var returnTotalRecordCountAttr = el.GetAttribute("returntotalrecordcount");
        if (returnTotalRecordCountAttr == null)
        {
            return false;
        }

        if (!bool.TryParse(returnTotalRecordCountAttr.Value, out var bReturnCount))
        {
            throw new Exception("returntotalrecordcount attribute in fetch node must be an boolean");
        }

        return bReturnCount;
    }

    public static bool? ToReturnTotalRecordCount(this XDocument xlDoc) =>
        xlDoc.Elements() //fetch
            .FirstOrDefault()?.ToReturnTotalRecordCount();

    private static int? ToTopCount(this XElement el)
    {
        var countAttr = el.GetAttribute("top");
        if (countAttr == null)
        {
            return null;
        }

        if (!int.TryParse(countAttr.Value, out var iCount))
        {
            throw new Exception("Top attribute in fetch node must be an integer");
        }

        return iCount;
    }


    public static int? ToTopCount(this XDocument xlDoc) =>
        //Check if all-attributes exist
        xlDoc.Elements() //fetch
            .FirstOrDefault()?.ToTopCount();

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
                return elem.GetAttribute("name") != null && elem.GetAttribute("from") != null && elem.GetAttribute("to") != null;

            case "order":
                if (elem.Document?.IsAggregateFetchXml() == true)
                {
                    return elem.GetAttribute("alias") != null && elem.GetAttribute("attribute") == null;
                }

                return elem.GetAttribute("attribute") != null;

            case "condition":
                return elem.GetAttribute("attribute") != null && elem.GetAttribute("operator") != null;

            default:
                throw new Exception($"Node {elem.Name.LocalName} is not a valid FetchXml node or it doesn't have the required attributes");
        }
    }
}
