// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

namespace Digitall.Stub.Errors;

public enum ErrorCodes : int
{

    /// <summary>
    /// The specified attribute does not exist on this entity.
    /// </summary>
    QueryBuilderNoAttribute = -2147217149,

    /// <summary>
    /// The specified entity was not found.
    /// </summary>
    QueryBuilderNoEntity = -2147217150,

    /// <summary>
    /// The specified object was not found.
    /// </summary>
    ObjectDoesNotExist = -2147220969,

    /// <summary>
    /// Operation failed due to a SQL integrity violation.
    /// </summary>
    DuplicateRecord = 2147220937,

    /// <summary>
    /// The specified attribute value is not valid.
    /// </summary>
    InvalidArgument = -2147220989,

    /// <summary>
    /// Invalid alias for aggregate operation.
    /// </summary>
    QueryBuilderInvalid_Alias = -2147217143,

    /// <summary>
    /// The operator is not valid or it is not supported.
    /// </summary>
    InvalidOperatorCode = -2147187691,

}
