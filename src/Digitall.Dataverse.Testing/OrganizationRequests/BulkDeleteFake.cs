// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Errors;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class BulkDeleteFake : OrganizationRequestFake<BulkDeleteRequest, BulkDeleteResponse>
{
    public override BulkDeleteResponse Execute(BulkDeleteRequest organizationRequest, FakeOrganizationService state)
    {
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
            Id = jobId,
            Attributes =
            {
                ["name"] = organizationRequest.JobName,
                ["ownerid"] = new EntityReference("systemuser", state.Options.UserId),
                ["operationtype"] = new OptionSetValue(13), // 13 = BulkDelete
            }
        };

        if (!string.IsNullOrEmpty(organizationRequest.RecurrencePattern))
        {
            asyncOpertation["recurrencepattern"] = organizationRequest.RecurrencePattern;
        }

        if (organizationRequest.StartDateTime != default)
        {
            asyncOpertation["recurrencestarttime"] = organizationRequest.StartDateTime;
        }

        state.Create(asyncOpertation);

        // delete all records from all queries
        foreach (QueryExpression queryExpression in organizationRequest.QuerySet ?? [])
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
