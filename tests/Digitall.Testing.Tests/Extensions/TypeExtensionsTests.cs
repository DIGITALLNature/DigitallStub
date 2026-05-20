// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Threading.Tasks;
using Digitall.Testing.Extensions;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Tests.Extensions;

public class TypeExtensionsTests
{
    [Test]
    public async Task IsOptionSet_Should_ReturnTrue_For_OptionSetValue()
    {
        await Assert.That(typeof(OptionSetValue).IsOptionSet()).IsTrue();
    }

    [Test]
    public async Task IsOptionSet_Should_ReturnTrue_For_Enum()
    {
        await Assert.That(typeof(DayOfWeek).IsOptionSet()).IsTrue();
    }

    [Test]
    public async Task IsOptionSet_Should_ReturnTrue_For_NullableEnum()
    {
        await Assert.That(typeof(DayOfWeek?).IsOptionSet()).IsTrue();
    }

    [Test]
    public async Task IsOptionSet_Should_ReturnFalse_For_OtherTypes()
    {
        await Assert.That(typeof(string).IsOptionSet()).IsFalse();
        await Assert.That(typeof(int).IsOptionSet()).IsFalse();
    }

    [Test]
    public async Task IsOptionSetValueCollection_Should_ReturnTrue_For_OptionSetValueCollection()
    {
        await Assert.That(typeof(OptionSetValueCollection).IsOptionSetValueCollection()).IsTrue();
    }

    [Test]
    public async Task IsDateTime_Should_ReturnTrue_For_DateTime()
    {
        await Assert.That(typeof(DateTime).IsDateTime()).IsTrue();
    }

    [Test]
    public async Task IsDateTime_Should_ReturnTrue_For_NullableDateTime()
    {
        await Assert.That(typeof(DateTime?).IsDateTime()).IsTrue();
    }

    [Test]
    public async Task IsNullableEnum_Should_ReturnTrue_For_NullableEnum()
    {
        await Assert.That(typeof(DayOfWeek?).IsNullableEnum()).IsTrue();
    }

    [Test]
    public async Task IsNullableEnum_Should_ReturnFalse_For_NonNullableEnum()
    {
        await Assert.That(typeof(DayOfWeek).IsNullableEnum()).IsFalse();
    }
}
