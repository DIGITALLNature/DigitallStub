// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Stub;

public class RealTimeClock : IStubClock
{
    public DateTime Today => DateTime.Today;
    public DateTime Now => DateTime.UtcNow;
}
