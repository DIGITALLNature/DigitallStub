// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Xml.Linq;
using Digitall.Dataverse.Testing.OrganizationRequests;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Tests for FetchProcessor FetchXML → QueryExpression conversion.
/// Verifies correct operator mapping, value conversion, and structural translation.
/// </summary>
public class FetchProcessorTests
{
    private FakeOrganizationService _sut = null!;

    [Before(Test)]
    public async Task Setup()
    {
        _sut = new FakeOrganizationService();
        _sut.AddRequest(new RetrieveMultipleFake());
        _sut.AddRequest(new FetchXmlToQueryExpressionFake());
        await Task.CompletedTask;
    }

    #region Basic FetchXml Conversion

    [Test]
    public async Task FetchXml_SimpleCondition_EquivalentToQueryExpression()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Match" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Other" });

        var fetchXml = @"<fetch><entity name='account'>
            <attribute name='name'/>
            <filter><condition attribute='name' operator='eq' value='Match'/></filter>
        </entity></fetch>";

        var qe = new QueryExpression("account") { ColumnSet = new ColumnSet("name") };
        qe.Criteria.AddCondition("name", ConditionOperator.Equal, "Match");

        var fetchResults = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        var qeResults = _sut.RetrieveMultiple(qe).Entities;

        await Assert.That(fetchResults).Count().IsEqualTo(1);
        await Assert.That(qeResults).Count().IsEqualTo(1);
        await Assert.That(fetchResults[0].GetAttributeValue<string>("name")).IsEqualTo("Match");
    }

    #endregion

    #region Operator Mapping

    [Test]
    public async Task FetchXml_NeOperator_MapsToNotEqual()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Excluded" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Kept" });

        var fetchXml = @"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='name' operator='ne' value='Excluded'/></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Kept");
    }

    [Test]
    public async Task FetchXml_NullOperator_MapsCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "HasName" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        var fetchXml = @"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='name' operator='null'/></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].Contains("name")).IsFalse();
    }

    [Test]
    public async Task FetchXml_NotNullOperator_MapsCorrectly()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "HasName" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid() });

        var fetchXml = @"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='name' operator='not-null'/></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("HasName");
    }

    [Test]
    public async Task FetchXml_InOperator_FiltersMultipleValues()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });

        var fetchXml = @"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='name' operator='in'>
                <value>A</value><value>C</value>
            </condition></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    [Test]
    public async Task FetchXml_LikeWithLeadingWildcard_MapsToEndsWith()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Contoso Ltd" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Fabrikam Inc" });

        var fetchXml = @"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='name' operator='like' value='%Ltd'/></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["name"]).IsEqualTo("Contoso Ltd");
    }

    #endregion

    #region Link Entity

    [Test]
    public async Task FetchXml_LinkEntity_InnerJoin()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Child" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Orphan" });

        var fetchXml = @"<fetch><entity name='contact'>
            <all-attributes/>
            <link-entity name='account' from='accountid' to='parentcustomerid' link-type='inner'>
                <filter><condition attribute='name' operator='eq' value='Parent'/></filter>
            </link-entity>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0]["fullname"]).IsEqualTo("Child");
    }

    [Test]
    public async Task FetchXml_LinkEntity_LeftOuterJoin()
    {
        var accountId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = accountId, ["name"] = "Parent" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = accountId, ["fullname"] = "Child" });
        _sut.Add(new Entity("contact") { Id = Guid.NewGuid(), ["parentcustomerid"] = Guid.NewGuid(), ["fullname"] = "Orphan" });

        var fetchXml = @"<fetch><entity name='contact'>
            <all-attributes/>
            <link-entity name='account' from='accountid' to='parentcustomerid' link-type='outer'>
            </link-entity>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    #endregion

    #region Ordering and Paging

    [Test]
    public async Task FetchXml_OrderBy_SortsResults()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });

        var fetchXml = @"<fetch><entity name='account'>
            <attribute name='name'/>
            <order attribute='name' descending='false'/>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(3);
        await Assert.That(results[0]["name"]).IsEqualTo("A");
        await Assert.That(results[1]["name"]).IsEqualTo("B");
        await Assert.That(results[2]["name"]).IsEqualTo("C");
    }

    [Test]
    public async Task FetchXml_TopCount_LimitsResults()
    {
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "A" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "B" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "C" });

        var fetchXml = @"<fetch top='2'><entity name='account'>
            <all-attributes/>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(2);
    }

    #endregion

    #region Value Conversion

    [Test]
    public async Task FetchXml_GuidValue_ParsedCorrectly()
    {
        var targetId = Guid.NewGuid();
        _sut.Add(new Entity("account") { Id = targetId, ["name"] = "Target" });
        _sut.Add(new Entity("account") { Id = Guid.NewGuid(), ["name"] = "Other" });

        var fetchXml = $@"<fetch><entity name='account'>
            <all-attributes/>
            <filter><condition attribute='accountid' operator='eq' value='{targetId}'/></filter>
        </entity></fetch>";

        var results = _sut.RetrieveMultiple(new FetchExpression(fetchXml)).Entities;
        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].Id).IsEqualTo(targetId);
    }

    #endregion
}
