// Copyright (c) DIGITALL Nature. All rights reserved
// DIGITALL Nature licenses this file to you under the Microsoft Public License.

namespace Digitall.Testing;

public interface IFakeDataverseBuilder<out TOrganizationService>
    where TOrganizationService : FakeOrganizationService
{
    TOrganizationService GetOrganizationService();
}
