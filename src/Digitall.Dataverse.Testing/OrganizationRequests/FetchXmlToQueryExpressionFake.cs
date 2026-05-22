// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Digitall.Dataverse.Testing.Logic.Queries;
using Microsoft.Crm.Sdk.Messages;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class FetchXmlToQueryExpressionFake : OrganizationRequestFake<FetchXmlToQueryExpressionRequest, FetchXmlToQueryExpressionResponse>
{
    public override FetchXmlToQueryExpressionResponse Execute(FetchXmlToQueryExpressionRequest organizationRequest, FakeOrganizationService state)
    {
        ArgumentNullException.ThrowIfNull(organizationRequest);

        if (string.IsNullOrWhiteSpace(organizationRequest.FetchXml))
        {
            throw new ArgumentException("FetchXml must not be null or empty.", nameof(organizationRequest));
        }

        XDocument xmlDocument;
        try
        {
            xmlDocument = XDocument.Parse(organizationRequest.FetchXml);
        }
        catch (Exception ex)
        {
            throw new ArgumentException("FetchXml is not valid XML.", nameof(organizationRequest), ex);
        }

        var processor = new QueryProcessor(state);
        var query = processor.ConvertXmlDocumentToQueryExpression(xmlDocument);

        return new FetchXmlToQueryExpressionResponse
        {
            Results =
            {
                ["Query"] = query
            }
        };
    }
}
