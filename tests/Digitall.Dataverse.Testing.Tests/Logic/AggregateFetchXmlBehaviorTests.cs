// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using Digitall.Dataverse.Testing.Tests.Fixtures;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Digitall.Dataverse.Testing.Tests.Logic;

/// <summary>
/// Behavioral tests for FetchXml aggregate queries verifying correct results
/// for Count, Sum, Avg, Min, Max, GroupBy, and DateTimeGrouping.
/// </summary>
public class AggregateFetchXmlBehaviorTests
{
    [Test]
    public async Task Count_ReturnsCorrectCount()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="accountid" alias="total" aggregate="count" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));

        await Assert.That(result.Entities).Count().IsEqualTo(1);
        var count = result.Entities[0].GetAttributeValue<AliasedValue>("total");
        await Assert.That(count).IsNotNull();
        await Assert.That((int)count!.Value!).IsEqualTo(2); // 2 accounts
    }

    [Test]
    public async Task CountColumn_CountsNonNullValues()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // Only corpA has MarketingOnly set, corpB does not (null)
        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="telephone1" alias="phone_count" aggregate="countcolumn" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        var count = result.Entities[0].GetAttributeValue<AliasedValue>("phone_count");
        await Assert.That(count).IsNotNull();
        // Only corpA has telephone1="1"
        await Assert.That((int)count!.Value!).IsEqualTo(1);
    }

    [Test]
    public async Task Sum_ReturnsSumOfValues()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        // corpA.MarketCap = 123, corpB.MarketCap = 321
        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="marketcap" alias="total_cap" aggregate="sum" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        var sum = result.Entities[0].GetAttributeValue<AliasedValue>("total_cap");
        await Assert.That(sum).IsNotNull();
        await Assert.That(((Money)sum!.Value!).Value).IsEqualTo(444m); // 123 + 321
    }

    [Test]
    public async Task Avg_ReturnsAverageOfValues()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="marketcap" alias="avg_cap" aggregate="avg" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        var avg = result.Entities[0].GetAttributeValue<AliasedValue>("avg_cap");
        await Assert.That(avg).IsNotNull();
        await Assert.That(((Money)avg!.Value!).Value).IsEqualTo(222m); // (123 + 321) / 2
    }

    [Test]
    public async Task Min_ReturnsMinimumValue()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="marketcap" alias="min_cap" aggregate="min" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        var min = result.Entities[0].GetAttributeValue<AliasedValue>("min_cap");
        await Assert.That(min).IsNotNull();
        await Assert.That(((Money)min!.Value!).Value).IsEqualTo(123m);
    }

    [Test]
    public async Task Max_ReturnsMaximumValue()
    {
        var sut = new FakeOrganizationService();
        sut.AddRange(TestData.Default);

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="marketcap" alias="max_cap" aggregate="max" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        var max = result.Entities[0].GetAttributeValue<AliasedValue>("max_cap");
        await Assert.That(max).IsNotNull();
        await Assert.That(((Money)max!.Value!).Value).IsEqualTo(321m);
    }

    [Test]
    public async Task GroupBy_GroupsResultsByAttribute()
    {
        var sut = new FakeOrganizationService();
        sut.Add(new Account(Guid.NewGuid()) { Name = "GroupA", Revenue = new Money(100m) });
        sut.Add(new Account(Guid.NewGuid()) { Name = "GroupA", Revenue = new Money(200m) });
        sut.Add(new Account(Guid.NewGuid()) { Name = "GroupB", Revenue = new Money(50m) });

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="name" alias="name_group" groupby="true" />
                <attribute name="accountid" alias="account_count" aggregate="count" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        await Assert.That(result.Entities).Count().IsEqualTo(2); // 2 groups: GroupA, GroupB

        var groupA = result.Entities.FirstOrDefault(e =>
            (e.GetAttributeValue<AliasedValue>("name_group")?.Value as string) == "GroupA");
        await Assert.That(groupA).IsNotNull();
        await Assert.That((int)groupA!.GetAttributeValue<AliasedValue>("account_count").Value!).IsEqualTo(2);
    }

    [Test]
    public async Task GroupBy_DateGrouping_Year_GroupsByYear()
    {
        var sut = new FakeOrganizationService();
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2023, 3, 1) });
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2023, 7, 1) });
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2024, 1, 1) });

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="overriddencreatedon" alias="year_group" groupby="true" dategrouping="year" />
                <attribute name="accountid" alias="cnt" aggregate="count" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        await Assert.That(result.Entities).Count().IsEqualTo(2); // 2023, 2024

        var group2023 = result.Entities.FirstOrDefault(e =>
            (int)(e.GetAttributeValue<AliasedValue>("year_group")?.Value ?? 0) == 2023);
        await Assert.That(group2023).IsNotNull();
        await Assert.That((int)group2023!.GetAttributeValue<AliasedValue>("cnt").Value!).IsEqualTo(2);
    }

    [Test]
    public async Task GroupBy_DateGrouping_Month_GroupsByMonth()
    {
        var sut = new FakeOrganizationService();
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2024, 1, 5) });
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2024, 1, 20) });
        sut.Add(new Account(Guid.NewGuid()) { OverriddenCreatedOn = new DateTime(2024, 3, 1) });

        var fetchXml = """
            <fetch aggregate="true">
              <entity name="account">
                <attribute name="overriddencreatedon" alias="month_group" groupby="true" dategrouping="month" />
                <attribute name="accountid" alias="cnt" aggregate="count" />
              </entity>
            </fetch>
            """;

        var result = sut.RetrieveMultiple(new FetchExpression(fetchXml));
        await Assert.That(result.Entities).Count().IsEqualTo(2); // Jan, Mar

        var janGroup = result.Entities.FirstOrDefault(e =>
            (int)(e.GetAttributeValue<AliasedValue>("month_group")?.Value ?? 0) == 1);
        await Assert.That(janGroup).IsNotNull();
        await Assert.That((int)janGroup!.GetAttributeValue<AliasedValue>("cnt").Value!).IsEqualTo(2);
    }
}
