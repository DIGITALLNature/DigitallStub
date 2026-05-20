// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Digitall.Testing.Errors;

public static class ErrorFactory
{

    [DoesNotReturn]
    public static void ThrowFault(ErrorCodes errorCode, string message)
    {
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)errorCode, Message = message }, message);
    }

    [DoesNotReturn]
    public static void ThrowFault(string message)
    {
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { Message = message }, message);
    }
}
