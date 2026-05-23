// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Digitall.Dataverse.Testing.Errors;
using Microsoft.Xrm.Sdk;

namespace Digitall.Dataverse.Testing.Tests.Errors;

public class ErrorFactoryTests
{
    [Test]
    public async Task ThrowFault_WithErrorCode_ThrowsFaultExceptionWithCorrectCode()
    {
        var exception = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => ErrorFactory.ThrowFault(ErrorCodes.ObjectDoesNotExist, "not found"));

        await Assert.That(exception.Detail.ErrorCode).IsEqualTo((int)ErrorCodes.ObjectDoesNotExist);
    }

    [Test]
    public async Task ThrowFault_WithErrorCode_ThrowsFaultExceptionWithCorrectMessage()
    {
        var exception = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => ErrorFactory.ThrowFault(ErrorCodes.InvalidArgument, "invalid value"));

        await Assert.That(exception.Detail.Message).IsEqualTo("invalid value");
    }

    [Test]
    public async Task ThrowFault_WithMessage_ThrowsFaultExceptionWithMessage()
    {
        var exception = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => ErrorFactory.ThrowFault("something went wrong"));

        await Assert.That(exception.Message).IsEqualTo("something went wrong");
    }

    [Test]
    public async Task ThrowFault_WithMessage_FaultDetailContainsMessage()
    {
        var exception = Assert.Throws<FaultException<OrganizationServiceFault>>(
            () => ErrorFactory.ThrowFault("detail message"));

        await Assert.That(exception.Detail.Message).IsEqualTo("detail message");
    }
}
