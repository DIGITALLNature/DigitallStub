// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Stub;

public interface IStubClock
{
    DateTime Today { get; }
    DateTime Now { get; }
}
