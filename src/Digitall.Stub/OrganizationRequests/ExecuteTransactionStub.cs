// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;

namespace Digitall.Stub.OrganizationRequests;

public class ExecuteTransactionStub : OrganizationRequestStub<ExecuteTransactionRequest, ExecuteTransactionResponse>
{
    public override ExecuteTransactionResponse Execute(ExecuteTransactionRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

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
