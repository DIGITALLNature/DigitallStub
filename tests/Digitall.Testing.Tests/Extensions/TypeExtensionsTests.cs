// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using AwesomeAssertions;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Tests.Extensions;

[TestClass]
public class TypeExtensionsTests
{
    [TestMethod]
    public void IsOptionSet_Should_ReturnTrue_For_OptionSetValue()
    {
        typeof(OptionSetValue).IsOptionSet().Should().BeTrue();
    }

    [TestMethod]
    public void IsOptionSet_Should_ReturnTrue_For_Enum()
    {
        typeof(DayOfWeek).IsOptionSet().Should().BeTrue();
    }

    [TestMethod]
    public void IsOptionSet_Should_ReturnTrue_For_NullableEnum()
    {
        typeof(DayOfWeek?).IsOptionSet().Should().BeTrue();
    }

    [TestMethod]
    public void IsOptionSet_Should_ReturnFalse_For_OtherTypes()
    {
        typeof(string).IsOptionSet().Should().BeFalse();
        typeof(int).IsOptionSet().Should().BeFalse();
    }

    [TestMethod]
    public void IsOptionSetValueCollection_Should_ReturnTrue_For_OptionSetValueCollection()
    {
        typeof(OptionSetValueCollection).IsOptionSetValueCollection().Should().BeTrue();
    }

    [TestMethod]
    public void IsDateTime_Should_ReturnTrue_For_DateTime()
    {
        typeof(DateTime).IsDateTime().Should().BeTrue();
    }

    [TestMethod]
    public void IsDateTime_Should_ReturnTrue_For_NullableDateTime()
    {
        typeof(DateTime?).IsDateTime().Should().BeTrue();
    }

    [TestMethod]
    public void IsNullableEnum_Should_ReturnTrue_For_NullableEnum()
    {
        typeof(DayOfWeek?).IsNullableEnum().Should().BeTrue();
    }

    [TestMethod]
    public void IsNullableEnum_Should_ReturnFalse_For_NonNullableEnum()
    {
        typeof(DayOfWeek).IsNullableEnum().Should().BeFalse();
    }
}
