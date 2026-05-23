// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Logic.Queries;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

public class ValidatorsTests
{
    [Test]
    public async Task ValidConditionEntityName_MatchesLinkAlias_Passes()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner)
        {
            EntityAlias = "c"
        };
        qe.LinkEntities.Add(link);
        link.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "c", AttributeName = "fullname", Operator = ConditionOperator.Equal, Values = { "Test" } });

        await Assert.That(() => Validators.ValidateLinkedAliases(qe, link)).ThrowsNothing();
    }

    [Test]
    public async Task ValidConditionEntityName_MatchesLinkEntity_Passes()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link);
        link.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "contact", AttributeName = "fullname", Operator = ConditionOperator.Equal, Values = { "Test" } });

        await Assert.That(() => Validators.ValidateLinkedAliases(qe, link)).ThrowsNothing();
    }

    [Test]
    public async Task ValidConditionEntityName_MatchesPrimaryEntity_Passes()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link);
        link.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "account", AttributeName = "name", Operator = ConditionOperator.Equal, Values = { "Test" } });

        await Assert.That(() => Validators.ValidateLinkedAliases(qe, link)).ThrowsNothing();
    }

    [Test]
    public async Task DuplicateAlias_ThrowsFaultException()
    {
        var qe = new QueryExpression("account");
        var link1 = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner)
        {
            EntityAlias = "dup"
        };
        var link2 = new LinkEntity("account", "task", "accountid", "regardingobjectid", JoinOperator.Inner)
        {
            EntityAlias = "dup"
        };
        qe.LinkEntities.Add(link1);
        qe.LinkEntities.Add(link2);
        link1.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "dup", AttributeName = "fullname", Operator = ConditionOperator.Equal, Values = { "Test" } });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => Validators.ValidateLinkedAliases(qe, link1));
        await Assert.That(ex.Message).Contains("not unique amongst all top-level table and join aliases");
    }

    [Test]
    public async Task DuplicateEntityName_NoAlias_ThrowsFaultException()
    {
        var qe = new QueryExpression("account");
        var link1 = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        var link2 = new LinkEntity("account", "contact", "accountid", "ownerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link1);
        qe.LinkEntities.Add(link2);
        link1.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "contact", AttributeName = "fullname", Operator = ConditionOperator.Equal, Values = { "Test" } });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => Validators.ValidateLinkedAliases(qe, link1));
        await Assert.That(ex.Message).Contains("more than one LinkEntity");
    }

    [Test]
    public async Task UnknownEntityName_ThrowsFaultException()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link);
        link.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "unknown", AttributeName = "name", Operator = ConditionOperator.Equal, Values = { "Test" } });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => Validators.ValidateLinkedAliases(qe, link));
        await Assert.That(ex.Message).Contains("not found");
    }

    [Test]
    public async Task SingleEntityMatch_AppendsOneToEntityName()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link);
        var condition = new ConditionExpression { EntityName = "contact", AttributeName = "fullname", Operator = ConditionOperator.Equal, Values = { "Test" } };
        link.LinkCriteria.AddCondition(condition);

        Validators.ValidateLinkedAliases(qe, link);

        await Assert.That(condition.EntityName).IsEqualTo("contact1");
    }

    [Test]
    public async Task NullFilter_DoesNotThrow()
    {
        var qe = new QueryExpression("account");

        await Assert.That(() => Validators.ValidateFilterExpressionAliases(qe, null)).ThrowsNothing();
    }

    [Test]
    public async Task NestedLinkEntities_ValidatesRecursively()
    {
        var qe = new QueryExpression("account");
        var outerLink = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        var innerLink = new LinkEntity("contact", "task", "contactid", "regardingobjectid", JoinOperator.Inner);
        outerLink.LinkEntities.Add(innerLink);
        qe.LinkEntities.Add(outerLink);
        innerLink.LinkCriteria.AddCondition(new ConditionExpression { EntityName = "unknown_nested", AttributeName = "subject", Operator = ConditionOperator.Equal, Values = { "Test" } });

        var ex = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => Validators.ValidateLinkedAliases(qe, outerLink));
        await Assert.That(ex.Message).Contains("not found");
    }

    [Test]
    public async Task ConditionWithoutEntityName_SkipsValidation()
    {
        var qe = new QueryExpression("account");
        var link = new LinkEntity("account", "contact", "accountid", "parentcustomerid", JoinOperator.Inner);
        qe.LinkEntities.Add(link);
        link.LinkCriteria.AddCondition(new ConditionExpression("fullname", ConditionOperator.Equal, "Test"));

        await Assert.That(() => Validators.ValidateLinkedAliases(qe, link)).ThrowsNothing();
    }
}
