# Configuration

The Cratis Application Model can be configured both through `appsettings.json` and programmatically to customize its behavior. The main configuration is handled through the `ApplicationModelOptions` class.

## Default Configuration Section

By default, the Application Model looks for configuration under the `Cratis:ApplicationModel` section in your `appsettings.json` file.

## Configuration Options

### Complete Configuration Example

Here's a complete example showing all available configuration options in `appsettings.json`:

```json
{
  "Cratis": {
    "ApplicationModel": {
      "CorrelationId": {
        "HttpHeader": "X-Correlation-ID"
      },
      "Tenancy": {
        "HttpHeader": "x-cratis-tenant-id"
      },
      "GeneratedApis": {
        "RoutePrefix": "api",
        "SegmentsToSkipForRoute": 0,
        "IncludeCommandNameInRoute": true,
        "IncludeQueryNameInRoute": true
      }
    }
  }
}
```

### Configuration Properties

#### CorrelationId

Controls how correlation IDs are handled in HTTP requests.

- **HttpHeader** (string, default: `"X-Correlation-ID"`): The HTTP header name to use for correlation ID tracking.

#### Tenancy

Controls multi-tenancy support through HTTP headers.

- **HttpHeader** (string, default: `"x-cratis-tenant-id"`): The HTTP header name to use for tenant identification.

#### GeneratedApis

Controls how automatically generated API endpoints are configured for commands and queries.

- **RoutePrefix** (string, default: `"api"`): The base route prefix for all generated API endpoints.
- **SegmentsToSkipForRoute** (int, default: `0`): Number of namespace segments to skip when constructing routes from type namespaces.
- **IncludeCommandNameInRoute** (bool, default: `true`): Whether to include the command type name as the last segment of the route for command endpoints.
- **IncludeQueryNameInRoute** (bool, default: `true`): Whether to include the query type name as the last segment of the route for query endpoints.

#### IdentityDetailsProvider

- **IdentityDetailsProvider** (Type, default: `null`): Specifies a custom identity details provider type. If not specified, the system will use type discovery to find one automatically.

## Setup Methods

### Using Configuration File

The most common approach is to use the configuration file with the default section:

```csharp
var builder = Host.CreateDefaultBuilder(args);

// Use default configuration section (Cratis:ApplicationModel)
builder.UseCratisApplicationModel();

var app = builder.Build();
app.UseCratisApplicationModel();
```

### Using Custom Configuration Section

You can specify a custom configuration section path:

```csharp
var builder = Host.CreateDefaultBuilder(args);

// Use custom configuration section
builder.UseCratisApplicationModel("MyApp:CratisConfig");

var app = builder.Build();
app.UseCratisApplicationModel();
```

### Programmatic Configuration

You can configure the options entirely through code:

```csharp
var builder = Host.CreateDefaultBuilder(args);

builder.UseCratisApplicationModel(options =>
{
    // Configure correlation ID
    options.CorrelationId.HttpHeader = "X-My-Correlation-ID";
    
    // Configure tenancy
    options.Tenancy.HttpHeader = "X-Tenant-ID";
    
    // Configure generated APIs
    options.GeneratedApis.RoutePrefix = "myapi";
    options.GeneratedApis.SegmentsToSkipForRoute = 2;
    options.GeneratedApis.IncludeCommandNameInRoute = false;
    options.GeneratedApis.IncludeQueryNameInRoute = false;
    
    // Set custom identity details provider
    options.IdentityDetailsProvider = typeof(MyCustomIdentityDetailsProvider);
});

var app = builder.Build();
app.UseCratisApplicationModel();
```

### Hybrid Configuration

You can combine configuration file settings with programmatic overrides:

```csharp
var builder = Host.CreateDefaultBuilder(args);

// First bind from configuration, then override specific values
builder.UseCratisApplicationModel("Cratis:ApplicationModel")
       .ConfigureServices(services =>
       {
           services.Configure<ApplicationModelOptions>(options =>
           {
               // Override specific settings programmatically
               options.GeneratedApis.RoutePrefix = "v1/api";
           });
       });

var app = builder.Build();
app.UseCratisApplicationModel();
```

## Environment-Specific Configuration

You can use different configurations for different environments using the standard ASP.NET Core configuration pattern:

**appsettings.json** (base configuration):

```json
{
  "Cratis": {
    "ApplicationModel": {
      "GeneratedApis": {
        "RoutePrefix": "api"
      }
    }
  }
}
```

**appsettings.Development.json** (development overrides):

```json
{
  "Cratis": {
    "ApplicationModel": {
      "CorrelationId": {
        "HttpHeader": "X-Dev-Correlation-ID"
      }
    }
  }
}
```

**appsettings.Production.json** (production overrides):

```json
{
  "Cratis": {
    "ApplicationModel": {
      "GeneratedApis": {
        "RoutePrefix": "v1"
      }
    }
  }
}
```

## Route Generation Examples

The `GeneratedApis` configuration affects how routes are generated for your commands and queries. Here are some examples:

Given a command class `MyApp.Sales.Commands.CreateOrderCommand`:

### Default Configuration

```json
{
  "GeneratedApis": {
    "RoutePrefix": "api",
    "SegmentsToSkipForRoute": 0,
    "IncludeCommandNameInRoute": true,
    "IncludeQueryNameInRoute": true
  }
}
```

**Generated route**: `/api/MyApp/Sales/Commands/CreateOrderCommand`

### Skip Namespace Segments

```json
{
  "GeneratedApis": {
    "RoutePrefix": "api",
    "SegmentsToSkipForRoute": 2,
    "IncludeCommandNameInRoute": true,
    "IncludeQueryNameInRoute": true
  }
}
```

**Generated route**: `/api/Sales/Commands/CreateOrderCommand`

### Exclude Type Names

```json
{
  "GeneratedApis": {
    "RoutePrefix": "api",
    "SegmentsToSkipForRoute": 3,
    "IncludeCommandNameInRoute": false,
    "IncludeQueryNameInRoute": false
  }
}
```

**Generated route**: `/api/Commands` (for commands) or `/api/Queries` (for queries)

## Best Practices

1. **Use Configuration Files**: For most scenarios, use `appsettings.json` configuration as it allows easy environment-specific overrides without code changes.

2. **Environment-Specific Settings**: Leverage `appsettings.{Environment}.json` files for environment-specific configurations.

3. **Programmatic Configuration**: Use programmatic configuration when you need to:
   - Set configuration based on runtime conditions
   - Use custom identity providers
   - Override specific settings that can't be easily expressed in JSON

4. **Route Planning**: Consider your API route structure carefully when configuring `GeneratedApis` options, especially in public-facing APIs where route stability is important.

5. **Header Standardization**: Use standard HTTP header names for correlation IDs and tenant IDs that align with your organization's conventions and any API gateways or load balancers in use.
