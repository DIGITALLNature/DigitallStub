// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

using System.Reflection;
using Digitall.Testing.OrganizationRequests;

namespace Digitall.Testing;

public class RequestFakeRegistry
{
    private readonly Dictionary<Type, IOrganizationRequestFake> _requestFakes = new();

    public void AddRequest(IOrganizationRequestFake fake)
    {
        _requestFakes.Add(fake.ForType, fake);
    }

    public void AddRequests(IEnumerable<IOrganizationRequestFake> requests)
    {
        foreach (var request in requests)
        {
            AddRequest(request);
        }
    }

    public void AddDefaultRequests()
    {
        Assembly a = typeof(IOrganizationRequestFake).Assembly;
        var requests = a.GetTypes().Where(type =>
            type.IsClass && type is { IsAbstract: false, Namespace: "Digitall.Testing.OrganizationRequests" } && typeof(IOrganizationRequestFake).IsAssignableFrom(type)).ToList();

        foreach (var request in requests)
        {
            AddRequestIfNecessary((Activator.CreateInstance(request) as IOrganizationRequestFake)!);
        }
    }

    public void AddRequestIfNecessary(IOrganizationRequestFake fake)
    {
        if (!_requestFakes.ContainsKey(fake.ForType))
        {
            AddRequest(fake);
        }
    }

    public bool TryGetFake(Type requestType, out IOrganizationRequestFake? fake)
    {
        return _requestFakes.TryGetValue(requestType, out fake);
    }
}
