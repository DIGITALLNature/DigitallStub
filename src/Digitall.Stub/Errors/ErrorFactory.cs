// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Errors;

public static class ErrorFactory
{

    public static void ThrowFault(ErrorCodes errorCode, string message)
    {
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)errorCode, Message = message }, message);
    }
}
