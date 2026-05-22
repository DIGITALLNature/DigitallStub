// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.OrganizationRequests;

/// <summary>
/// Regression tests for FetchXml bool conditions on attributes that are NOT present in the
/// early-bound model (so the FetchProcessor cannot infer the attribute type via reflection).
/// Without the bool fallback in <c>GetConditionExpressionValueCast</c>, the right-hand side would
/// remain a string ("true"/"false"), while the left-hand side (entity attribute) is cast to bool —
/// causing equality to always evaluate to false.
/// </summary>
public class RetrieveMultipleFetchXmlBoolTests
{
    private static FakeOrganizationService BuildSut()
    {
        var sut = new FakeOrganizationService();

        // Register a custom bool attribute on the (early-bound) account entity so that
        // ThrowIfNotKnownAttribute passes, but IsKnownAttributeForType returns false
        // (because it is not part of the early-bound Account class).
        var metadata = new EntityMetadata { LogicalName = Account.EntityLogicalName };
        var attribute = new BooleanAttributeMetadata { LogicalName = "new_custombool" };
        typeof(EntityMetadata)
            .GetProperty(nameof(EntityMetadata.Attributes))!
            .SetValue(metadata, new AttributeMetadata[] { attribute });
        sut.State.EntityMetadata[Account.EntityLogicalName] = metadata;

        return sut;
    }

    [Test]
    public async Task FetchXml_BoolCondition_WithoutEarlyBoundAttribute_Equals_True_Matches_Only_True()
    {
        var sut = BuildSut();

        var active = new Entity(Account.EntityLogicalName, Guid.NewGuid()) { ["new_custombool"] = true };
        var inactive = new Entity(Account.EntityLogicalName, Guid.NewGuid()) { ["new_custombool"] = false };
        sut.Add(active);
        sut.Add(inactive);

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="new_custombool" />
                <filter type="and">
                  <condition attribute="new_custombool" operator="eq" value="true" />
                </filter>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(active.Id);
    }

    [Test]
    public async Task FetchXml_BoolCondition_WithoutEarlyBoundAttribute_Equals_False_Matches_Only_False()
    {
        var sut = BuildSut();

        var active = new Entity(Account.EntityLogicalName, Guid.NewGuid()) { ["new_custombool"] = true };
        var inactive = new Entity(Account.EntityLogicalName, Guid.NewGuid()) { ["new_custombool"] = false };
        sut.Add(active);
        sut.Add(inactive);

        const string fetchXml = """
            <fetch>
              <entity name="account">
                <attribute name="new_custombool" />
                <filter type="and">
                  <condition attribute="new_custombool" operator="eq" value="false" />
                </filter>
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        await Assert.That(result.Entities[0].Id).IsEqualTo(inactive.Id);
    }
}
