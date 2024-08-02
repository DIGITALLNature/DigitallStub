// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

[assembly:ProxyTypesAssembly]

namespace Digitall.Stub.Tests.Fixtures;

public static class TestData
{
    public static IEnumerable<Entity> Default
    {
        get
        {
            var CorpA = new Account(Guid.Parse("00000000-0000-0000-0001-000000000001"))
            {
                Name = "A Corp",
                Telephone1 = "1",
                Telephone2 = "2"
            };
            var CorpB = new Account(Guid.Parse("00000000-0000-0000-0001-000000000002"))
            {
                Name = "B Corp"
            };


            var ConA = new Contact(Guid.Parse("00000000-0000-0000-0002-000000000001"))
            {
                FirstName = "John A",
                LastName = " Doe A"
            };

            var ConB = new Contact(Guid.Parse("00000000-0000-0000-0002-000000000002"))
            {
                ParentCustomerId = CorpB.ToNamedEntityReference(),
                FirstName = "John B",
                LastName = " Doe B (Corp B)"
            };

            var ConC = new Contact(Guid.Parse("00000000-0000-0000-0002-000000000003"))
            {
                ParentCustomerId = CorpB.ToNamedEntityReference(),
                FirstName = "John C",
                LastName = " Doe C (Corp B)"
            };

            return new List<Entity>{ CorpA, CorpB, ConA, ConB, ConC };
        }
    }
}
