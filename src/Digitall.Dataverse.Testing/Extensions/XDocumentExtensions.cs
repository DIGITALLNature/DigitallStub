// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Extensions;

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

            return int.TryParse(countAttr.Value, out var iCount) ? iCount : throw new Exception("Count attribute in fetch node must be an integer");
        }

        public XAttribute? GetAttribute(string sAttributeName) => el.Attributes().FirstOrDefault(a => a.Name.LocalName.Equals(sAttributeName));

        public bool IsAttributeTrue(string attributeName)
        {
            var val = el.GetAttribute(attributeName)?.Value;

            return "true".Equals(val, StringComparison.InvariantCultureIgnoreCase) || "1".Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }

        private int? ToPageNumber()
        {
            var pageAttr = el.GetAttribute("page");
            if (pageAttr == null)
            {
                return null;
            }

            return int.TryParse(pageAttr.Value, out var iPage) ? iPage : throw new Exception("Count attribute in fetch node must be an integer");
        }

        private bool ToReturnTotalRecordCount()
        {
            var returnTotalRecordCountAttr = el.GetAttribute("returntotalrecordcount");
            if (returnTotalRecordCountAttr == null)
            {
                return false;
            }

            return bool.TryParse(returnTotalRecordCountAttr.Value, out var bReturnCount) ? bReturnCount : throw new Exception("returntotalrecordcount attribute in fetch node must be an boolean");
        }

        private int? ToTopCount()
        {
            var countAttr = el.GetAttribute("top");
            if (countAttr == null)
            {
                return null;
            }

            return int.TryParse(countAttr.Value, out var iCount) ? iCount : throw new Exception("Top attribute in fetch node must be an integer");
        }

        public bool IsFetchXmlNodeValid()
        {
            switch (el.Name.LocalName)
            {
                case "filter":
                case "value":
                case "fetch":
                    return true;

                case "entity":
                    return el.GetAttribute("name") != null;

                case "all-attributes":
                    return true;

                case "attribute":
                    return el.GetAttribute("name") != null;

                case "link-entity":
                    return el.GetAttribute("name") != null && el.GetAttribute("from") != null && el.GetAttribute("to") != null;

                case "order":
                    if (el.Document?.IsAggregateFetchXml() == true)
                    {
                        return el.GetAttribute("alias") != null && el.GetAttribute("attribute") == null;
                    }

                    return el.GetAttribute("attribute") != null;

                case "condition":
                    return el.GetAttribute("attribute") != null && el.GetAttribute("operator") != null;

                default:
                    throw new Exception($"Node {el.Name.LocalName} is not a valid FetchXml node or it doesn't have the required attributes");
            }
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
                    new OrderExpression
                    {
                        AttributeName = el.GetAttribute("attribute")?.Value,
                        OrderType = el.IsAttributeTrue("descending") ? OrderType.Descending : OrderType.Ascending,
                        EntityName = el.GetAttribute("entityname")?.Value
                    }).ToList();

            return orderByElements;
        }

        public int? ToPageNumber() =>
            //Check if all-attributes exist
            xlDoc.Elements() //fetch
                .FirstOrDefault()?.ToPageNumber();

        public bool? ToReturnTotalRecordCount() =>
            xlDoc.Elements() //fetch
                .FirstOrDefault()?.ToReturnTotalRecordCount();

        public int? ToTopCount() =>
            //Check if all-attributes exist
            xlDoc.Elements() //fetch
                .FirstOrDefault()?.ToTopCount();
    }
}
