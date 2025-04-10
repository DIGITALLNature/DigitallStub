// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.ServiceModel;
using Microsoft.Xrm.Sdk;

namespace Digitall.Stub.Errors;

public static class ErrorFactory
{

    public static void ThrowFault(ErrorCodes errorCode, string message)
    {
#if NETFRAMEWORK
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)errorCode, Message = message }, message);
#else
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { ErrorCode = (int)errorCode, Message = message }, new FaultReason(message));
#endif
    }

    public static void ThrowFault(string message)
    {
#if NETFRAMEWORK
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { Message = message }, message);
#else
        throw new FaultException<OrganizationServiceFault>(new OrganizationServiceFault { Message = message }, new FaultReason(message));
#endif
    }
}
