# Proxy Generation

The Cratis Application Model includes a powerful proxy generation tool that automatically creates TypeScript proxies for your commands and queries during the build process. This eliminates the need to manually write frontend integration code or consult Swagger documentation, as the proxies provide compile-time type safety and intellisense support.

## Overview

The proxy generator runs as part of your build process using the C# Roslyn compiler's code generator extensibility. It analyzes your compiled assemblies and generates TypeScript representations for:

- **Commands**: HTTP POST operations on controllers
- **Queries**: HTTP GET operations on controllers that return data

## Package Dependency

To enable proxy generation, add a reference to the [Cratis.Applications.ProxyGenerator.Build](https://www.nuget.org/packages/Cratis.Applications.ProxyGenerator.Build/) NuGet package to your project:

```xml
<PackageReference Include="Cratis.Applications.ProxyGenerator.Build" Version="1.0.0" />
```

> **Important**: All projects that contain controllers (commands or queries) should reference this package, as the proxy generation runs as part of the compilation process.

## Configuration

Configure the proxy generator by adding MSBuild properties to your `.csproj` file:

### Required Configuration

```xml
<PropertyGroup>
    <CratisProxiesOutputPath>$(MSBuildThisFileDirectory)../Web</CratisProxiesOutputPath>
</PropertyGroup>
```

- `CratisProxiesOutputPath`: Specifies where the generated TypeScript files will be written. This should typically point to your frontend project directory.

### Optional Configuration

```xml
<PropertyGroup>
    <CratisProxiesSegmentsToSkip>1</CratisProxiesSegmentsToSkip>
    <CratisProxiesSkipOutputDeletion>false</CratisProxiesSkipOutputDeletion>
    <CratisProxiesSkipCommandNameInRoute>false</CratisProxiesSkipCommandNameInRoute>
    <CratisProxiesSkipQueryNameInRoute>false</CratisProxiesSkipQueryNameInRoute>
    <CratisProxiesApiPrefix>api</CratisProxiesApiPrefix>
</PropertyGroup>
```

#### Configuration Options

| Property | Default | Description |
|----------|---------|-------------|
| `CratisProxiesOutputPath` | *(Required)* | The output directory for generated TypeScript files |
| `CratisProxiesSegmentsToSkip` | `0` | Number of namespace segments to skip when creating the folder structure |
| `CratisProxiesSkipOutputDeletion` | `false` | When `true`, preserves existing files in the output directory |
| `CratisProxiesSkipCommandNameInRoute` | `false` | When `true`, excludes the command name from the generated route for command endpoints |
| `CratisProxiesSkipQueryNameInRoute` | `false` | When `true`, excludes the query name from the generated route for query endpoints |
| `CratisProxiesApiPrefix` | `api` | The API prefix used in generated routes |

### Namespace Segment Skipping

The `CratisProxiesSegmentsToSkip` property is particularly useful in multi-project solutions with consistent naming conventions.

For example, if you have a folder structure like:

```shell
<Your Root Folder>
|
├── Api
│   └── MyFeature
├── Domain
│   └── MyFeature
├── Events
│   └── MyFeature
└── Read
    └── MyFeature
```

And corresponding namespaces like `Api.MyFeature`, `Domain.MyFeature`, `Read.MyFeature`, you might want to skip the first segment (`Api`, `Domain`, `Read`) to create a unified structure in your frontend. Setting `CratisProxiesSegmentsToSkip` to `1` would generate:

```shell
MyFeature/
├── commands/
└── queries/
```

Instead of:

```shell
Domain/
└── MyFeature/
    └── commands/
Read/
└── MyFeature/
    └── queries/
```

## Generated Output Structure

The proxy generator maintains the folder structure based on your C# namespaces (after applying segment skipping). For each directory containing generated files, an `index.ts` file is automatically created that exports all the generated artifacts.

### Commands

Commands are generated from HTTP POST controller actions. The generator looks for:

- Methods marked with `[HttpPost]`
- Parameters marked with `[FromBody]`, `[FromRoute]`, or `[FromQuery]`

Generated command classes:

- Extend the `Command` base class from `@cratis/applications/commands`
- Include all properties from route parameters, query parameters, and body content
- Provide a static `use()` method for React hook integration

### Queries

Queries are generated from HTTP GET controller actions. The generator supports:

- Single model queries: Return a single object
- Enumerable queries: Return an array of objects
- Observable queries: Support real-time updates via WebSockets

Generated query classes provide:

- Type-safe parameter handling
- React hooks for integration (`useQuery`)
- Observable query support for real-time data

### Types and Enums

The generator also creates TypeScript representations for:

- Complex types used in command/query parameters or return values
- Enumerations used in your domain models
- Proper import statements and module resolution

### Route Name Configuration

The `CratisProxiesSkipCommandNameInRoute` and `CratisProxiesSkipQueryNameInRoute` properties allow you to control whether the command or query type names are included in the generated routes.

By default, both properties are `false`, meaning type names are included in routes:

- Command `CreateOrderCommand` → `/api/orders/create-order-command`
- Query `GetOrdersQuery` → `/api/orders/get-orders-query`

When set to `true`, the type names are excluded:

- Command `CreateOrderCommand` → `/api/orders`
- Query `GetOrdersQuery` → `/api/orders`

**Example Configuration:**

```xml
<PropertyGroup>
    <!-- Include command names but exclude query names from routes -->
    <CratisProxiesSkipCommandNameInRoute>false</CratisProxiesSkipCommandNameInRoute>
    <CratisProxiesSkipQueryNameInRoute>true</CratisProxiesSkipQueryNameInRoute>
</PropertyGroup>
```

This allows for more granular control over API route generation, which can be particularly useful when you want cleaner URLs for queries while keeping explicit command names.

## Frontend Prerequisites

The generated proxies depend on the [@cratis/applications](https://www.npmjs.com/package/@cratis/applications) NPM package. Install it in your frontend project:

```bash
npm install @cratis/applications
```

## Build Integration

The proxy generation runs automatically during the build process through the `CratisProxyGenerator` MSBuild target, which executes after the `AfterBuild` target. The generator:

1. Loads your compiled assembly
2. Discovers controllers and their actions
3. Analyzes command and query patterns
4. Generates TypeScript proxies with proper typing
5. Creates index files for easy importing
6. Maintains the folder structure based on namespaces

## Excluding controller based Commands and Queries

To exclude specific commands or queries from proxy generation, mark them with the `[AspNetResult]` attribute:

```csharp
[HttpPost]
[AspNetResult]
public IActionResult MyExcludedCommand([FromBody] MyCommand command)
{
    // This won't generate a proxy
    return Ok();
}
```

## Example Usage

### Backend Controller

```csharp
[Route("/api/accounts")]
public class AccountsController : Controller
{
    [HttpPost]
    public Task CreateAccount([FromBody] CreateAccount command)
    {
        // Command implementation
    }

    [HttpGet]
    public IEnumerable<Account> GetAccounts([FromQuery] string? filter = null)
    {
        // Query implementation
    }
}
```

### Generated Frontend Proxies

The above controller would generate:

```typescript
// commands/CreateAccount.ts
export class CreateAccount extends Command<ICreateAccount> {
    // Auto-generated properties and methods
    static use(initialValues?: ICreateAccount): [CreateAccount, SetCommandValues<ICreateAccount>] {
        return useCommand<CreateAccount, ICreateAccount>(CreateAccount, initialValues);
    }
}

// queries/GetAccounts.ts  
export class GetAccounts extends QueryFor<Account[]> {
    // Auto-generated query implementation
    static use(args?: IGetAccountsParameters): QueryResultWithState<Account[]> {
        return useQuery<Account[], IGetAccountsParameters>(GetAccounts, args);
    }
}
```

## Advanced Configuration

For more complex scenarios, you can combine multiple configuration options:

```xml
<PropertyGroup>
    <!-- Output to a specific frontend directory -->
    <CratisProxiesOutputPath>$(MSBuildThisFileDirectory)../../Frontend/src/api</CratisProxiesOutputPath>
    
    <!-- Skip Domain/Read prefixes in namespaces -->
    <CratisProxiesSegmentsToSkip>1</CratisProxiesSegmentsToSkip>
    
    <!-- Use a custom API prefix -->
    <CratisProxiesApiPrefix>v1</CratisProxiesApiPrefix>
    
    <!-- Don't delete existing files (useful for debugging) -->
    <CratisProxiesSkipOutputDeletion>true</CratisProxiesSkipOutputDeletion>
</PropertyGroup>
```

This configuration would:

- Generate proxies in the `Frontend/src/api` directory
- Skip the first namespace segment when creating folders
- Use `v1` instead of `api` in routes
- Preserve any existing files in the output directory

## Troubleshooting

### Common Issues

1. **No proxies generated**: Ensure the NuGet package is referenced and the output path is correctly configured
2. **Missing types**: Complex types need to be public and discoverable by the generator
3. **Wrong folder structure**: Adjust `CratisProxiesSegmentsToSkip` to match your desired output structure
4. **Build errors**: Check that the target framework matches between your project and the proxy generator

### Debugging

Enable detailed MSBuild logging to see the proxy generation process:

```bash
dotnet build -v detailed
```

Look for messages starting with "MSBuildProjectDirectory", "OutputPath", and "ProxyGenerator base directory" to verify the configuration is correct.
