// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Diagnostics;
using Digitall.Stub.Errors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Stub.OrganizationRequests;

public class BulkDeleteStub : OrganizationRequestStub<BulkDeleteRequest, BulkDeleteResponse>
{
    public override BulkDeleteResponse Execute(BulkDeleteRequest organizationRequest, DataverseStub state)
    {
        Debug.Assert(state != null, nameof(state) + " != null");
        Debug.Assert(organizationRequest != null, nameof(organizationRequest) + " != null");

        if (string.IsNullOrEmpty(organizationRequest.JobName))
        {
            ErrorFactory.ThrowFault( "Can not Bulk delete without JobName");
        }
        if (organizationRequest.QuerySet == null)
        {
            ErrorFactory.ThrowFault( "Can not Bulk delete without QuerySet");
        }
        if (organizationRequest.CCRecipients == null)
        {
            ErrorFactory.ThrowFault( "Can not Bulk delete without CCRecipients");
        }
        if (organizationRequest.ToRecipients == null)
        {
            ErrorFactory.ThrowFault( "Can not Bulk delete without ToRecipients");
        }

        // generate JobId
        var jobId = Guid.NewGuid();

        // create related asyncOperation
        Entity asyncOpertation = new Entity("asyncoperation")
        {
            Id = jobId
        };

        state.Create(asyncOpertation);

        // delete all records from all queries
        foreach (QueryExpression queryExpression in organizationRequest.QuerySet)
        {
            EntityCollection recordsToDelete = state.RetrieveMultiple(queryExpression);
            foreach (Entity record in recordsToDelete.Entities)
            {
                state.Delete(record.LogicalName, record.Id);
            }
        }

        // set ayncoperation to completed
        asyncOpertation["statecode"] = new OptionSetValue(3);
        state.Update(asyncOpertation);

        // return result
        return new BulkDeleteResponse { ResponseName = "BulkDeleteResponse", ["JobId"] = jobId};
    }
}
