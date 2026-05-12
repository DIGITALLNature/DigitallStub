// Copyright (c) DIGITALL Nature.All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Linq;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.OrganizationRequests;

public class WhoAmIFake: OrganizationRequestFake<WhoAmIRequest,WhoAmIResponse>
{
    public override WhoAmIResponse Execute(WhoAmIRequest organizationRequest, FakeOrganizationService state)
    {
        var userId = Guid.Parse(Environment.GetEnvironmentVariable("UserId") ?? Guid.Empty.ToString());

        var results = new ParameterCollection {
            { "UserId", userId }
        };

        var user = state
            .CreateQuery("systemuser")
            .SingleOrDefault(u => u.Id == userId);

        if(user != null) {
            var buId = GetBusinessUnitId(user);
            results.Add("BusinessUnitId", buId);

            var orgId = GetOrganizationId(state, user, buId);
            results.Add("OrganizationId", orgId);
        }

        var response = new WhoAmIResponse
        {
            Results = results
        };
        return response;
    }

    private static Guid GetBusinessUnitId(Entity user) {
        var buRef = user.GetAttributeValue<EntityReference>("businessunitid");
        var buId = buRef?.Id ?? Guid.Parse(Environment.GetEnvironmentVariable("BusinessUnitId") ?? Guid.Empty.ToString());
        return buId;
    }

    private static Guid GetOrganizationId(FakeOrganizationService state, Entity user, Guid buId) {
        var orgId = user.GetAttributeValue<Guid?>("organizationid") ?? Guid.Empty;
        if(orgId == Guid.Empty) {
            var bu = state
                .CreateQuery("businessunit")
                .SingleOrDefault(b => b.Id == buId);
            var orgRef = bu.GetAttributeValue<EntityReference>("organizationid");
            orgId = orgRef?.Id ?? Guid.Empty;
        }

        return orgId;
    }
}
