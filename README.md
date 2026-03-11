# DigitallStub

A .NET stub implementation for Microsoft Dataverse testing, providing a lightweight, in-memory `IOrganizationService`.

## Features

- **FakeOrganizationService**: In-memory implementation of `IOrganizationService`.
- **Query Support**: Supports `QueryExpression`, `QueryByAttribute`, and `FetchXml`.
- **Request Fakes**: Extensible architecture for mocking `OrganizationRequest` messages.
- **Early-Bound Support**: Automatically discovers early-bound entity types via `[assembly:ProxyTypesAssembly]`.
- **Plugin Testing**: Utilities for building and testing Dataverse plugins (e.g., `PluginExecutionContextBuilder`).

## Prerequisites

- **.NET SDK**: Supports `net462` and `net10.0`.
- **IDE**: Rider or Visual Studio.

## Setup & Build

1.  **Restore Dependencies**:
    ```bash
    dotnet restore
    ```
2.  **Build Solution**:
    ```bash
    dotnet build
    ```

## Usage

### Basic Example

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using AwesomeAssertions;
using Digitall.Testing;

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

### Early-Bound Entities

To use early-bound types, ensure your test assembly (or the assembly containing the types) has the `ProxyTypesAssembly` attribute:

```csharp
[assembly: Microsoft.Xrm.Sdk.Client.ProxyTypesAssembly]
```

## Testing

Tests are located in `tests/Digitall.Testing.Tests`.

- **Run all tests**:
  ```bash
  dotnet test
  ```
- **Run specific tests**:
  ```bash
  dotnet test --filter Name~YourTestName
  ```

## Configuration (Environment Variables)

The `FakeOrganizationService` and its internal components can be influenced by the following environment variables:

| Variable | Description | Default |
| :--- | :--- | :--- |
| `MaxRetrieveCount` | Maximum number of records returned by `RetrieveMultiple`. | `5000` |
| `UserId` | The ID of the current user (used in `WhoAmI` and `EqualUserId` filters). | `Guid.Empty` |
| `BusinessUnitId` | The ID of the current business unit. | `Guid.Empty` |
| `FiscalYearStart` | Start date for fiscal year calculations. | Current Year Start |

## Project Structure

- `src/Digitall.Testing`: Core library containing `FakeOrganizationService`.
  - `Extensions`: Utilities for Entities, QueryExpressions, etc.
  - `OrganizationRequests`: Custom fakes for specific `OrganizationRequest` types.
  - `Logic`: Internal logic for query processing and state management.
- `tests/Digitall.Testing.Tests`: Unit tests for the library.

## License

This project is licensed under the Microsoft Public License (MS-PL). See [Licence.md](Licence.md) for details.
