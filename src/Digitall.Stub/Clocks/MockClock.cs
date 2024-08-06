// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;

namespace Digitall.Stub;

public class MockClock(DateTime utcNow) : IStubClock
{
    private readonly DateTime _now = utcNow;
    public DateTime Today => _now.Date;
    public DateTime Now => _now;
}
