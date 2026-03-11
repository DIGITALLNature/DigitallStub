// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Testing.OrganizationRequests;

public class ExecuteTransactionFake : OrganizationRequestFake<ExecuteTransactionRequest, ExecuteTransactionResponse>
{
    public override ExecuteTransactionResponse Execute(ExecuteTransactionRequest organizationRequest, FakeOrganizationService state)
    {
        var response = new ExecuteTransactionResponse { ["Responses"] = new OrganizationResponseCollection() };

        foreach (var r in organizationRequest.Requests)
        {
            var result = state.Execute(r);

            if (organizationRequest.ReturnResponses.HasValue && organizationRequest.ReturnResponses.Value)
            {
                response.Responses.Add(result);
            }
        }
        return response;
    }
}
