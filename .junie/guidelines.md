# Project Development Guidelines

This document provides essential information for developers working on the DigitallStub project, which provides a .NET stub for Dataverse testing.

## Build/Configuration Instructions

### Prerequisites
- .NET SDK (supports `net462` and `net10.0`)
- Standard IDE (Rider or Visual Studio)

### Build Steps
1. **Restore Dependencies**:
   ```bash
   dotnet restore
   ```
2. **Build Solution**:
   ```bash
   dotnet build
   ```

### Configuration
The project uses `global.json` and `Directory.Build.props` for consistent build settings across environments.
- **Environment Variables**: The `FakeOrganizationService` can be influenced by certain environment variables mentioned in `README.md`, such as `MaxRetrieveCount`, `CallerId`, `BusinessUnitId`, `FiscalYearStart`, and `FiscalPeriod`.

## Testing Information

### Running Tests
Tests are located in the `tests/Digitall.Testing.Tests` project.
- **Run all tests**:
  ```bash
  dotnet test
  ```
- **Run specific tests**:
  ```bash
  dotnet test --filter Name~YourTestName
  ```

### Adding New Tests
- Use **MSTest** as the testing framework.
- Use **AwesomeAssertions** for assertions.
- Place tests in the `tests/Digitall.Testing.Tests` directory, mirroring the structure of the source code.
- Ensure `[TestClass]` and `[TestMethod]` attributes are used.

### Test Example
Below is a demonstration of how to use `FakeOrganizationService` to test Dataverse operations.

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using AwesomeAssertions;
using Digitall.Testing;

namespace YourNamespace.Tests;

[TestClass]
public class DataverseStubTests
{
    [TestMethod]
    public void Should_CreateAndRetrieveEntity()
    {
        // Arrange: Initialize the fake service
        var service = new FakeOrganizationService();
        var account = new Entity("account")
        {
            ["name"] = "Test Account"
        };

        // Act: Perform operations
        var id = service.Create(account);
        var retrieved = service.Retrieve("account", id, new ColumnSet("name"));

        // Assert: Verify results
        retrieved.Should().NotBeNull();
        retrieved["name"].Should().Be("Test Account");
    }
}
```

## Additional Development Information

### Code Style
- Follow standard C# coding conventions.
- Use file-scoped namespaces (C# 10+).
- The project uses `Nullable` enable by default.
- Maintain consistency with existing code (e.g., copyright headers).

### Key Components
- **FakeOrganizationService**: The core stub implementation of `IOrganizationService`.
- **Extensions**: Located in `src/Digitall.Testing/Extensions`, these provide helpful utilities for working with Entities, QueryExpressions, etc.
- **OrganizationRequests**: Custom fakes for specific `OrganizationRequest` types can be added in `src/Digitall.Testing/OrganizationRequests`.

### Proxy Types
To use early-bound entities in tests, ensure your assembly is marked with `[assembly:ProxyTypesAssembly]` so that `FakeOrganizationService` can discover them.
