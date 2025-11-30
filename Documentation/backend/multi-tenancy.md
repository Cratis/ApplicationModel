# Multi-Tenancy

Cratis Arc provides comprehensive multi-tenancy support that allows your application to automatically detect and work with tenant-specific data. The framework handles tenant identification through HTTP headers and provides easy access to the current tenant context throughout your application.

## Overview

Multi-tenancy in the Arc is built around three core components:

- **`TenantIdMiddleware`** - Automatically extracts and sets the tenant ID from HTTP headers
- **`ITenantIdAccessor`** - Provides access to the current tenant ID throughout your application
- **`ArcOptions`** - Configures the HTTP header used for tenant identification

The system automatically handles tenant context propagation across async operations using `AsyncLocal<T>`, ensuring that the tenant ID is available throughout the entire request lifecycle.

## Configuration

Multi-tenancy is configured through the `ArcOptions` class, specifically using the `Tenancy` property:

```csharp
builder.Services.Configure<ArcOptions>(options =>
{
    options.Tenancy.HttpHeader = "X-Tenant-ID"; // Custom header name
});
```

### Configuration Properties

#### Tenancy Options

- **HttpHeader** (string, default: `"x-cratis-tenant-id"`): The HTTP header name that contains the tenant identifier

The default header follows the convention `x-cratis-tenant-id`, but you can customize it to match your application's requirements.

## Automatic Tenant Detection

The `TenantIdMiddleware` is automatically registered and configured to run early in the ASP.NET Core pipeline. It performs the following operations:

1. **Header Extraction**: Reads the tenant ID from the configured HTTP header
2. **Context Storage**: Stores the tenant ID in the HTTP context items
3. **Async Local Setting**: Sets the tenant ID in an `AsyncLocal<TenantId>` for thread-safe access

The middleware is automatically added to your application pipeline when you configure the Arc, so no manual registration is required.

## Accessing the Current Tenant

### Using ITenantIdAccessor

The primary way to access the current tenant in your application is through dependency injection of `ITenantIdAccessor`:

```csharp
using Cratis.Arc.Tenancy;

public class MyService
{
    private readonly ITenantIdAccessor _tenantIdAccessor;

    public MyService(ITenantIdAccessor tenantIdAccessor)
    {
        _tenantIdAccessor = tenantIdAccessor;
    }

    public async Task ProcessDataAsync()
    {
        var tenantId = _tenantIdAccessor.Current;
        
        // Use the tenant ID for tenant-specific operations
        var data = await GetTenantDataAsync(tenantId);
        
        // Process the data...
    }
}
```

## The TenantId Type

The Arc provides a strongly-typed `TenantId` concept that wraps the string value:

```csharp
public record TenantId(string Value) : ConceptAs<string>(Value)
{
    public static implicit operator TenantId(string value) => new(value);
}
```

This provides type safety and prevents mixing up tenant IDs with other string values in your application.

## Best Practices

1. **Consistent Header Usage**: Ensure all clients (frontend applications, API gateways, etc.) send the tenant ID in the configured header
2. **Validation**: Consider adding middleware to validate that tenant IDs are valid and that the requesting user has access to the specified tenant
3. **Database Isolation**: Use the tenant ID to filter database queries and ensure data isolation between tenants
4. **Logging**: Include the tenant ID in your logging context for better observability across tenant-specific operations
5. **Caching**: When using caching, include the tenant ID as part of cache keys to prevent data leakage between tenants

## Security Considerations

- Always validate that the requesting user has permission to access the specified tenant
- Consider implementing tenant validation middleware that runs after tenant detection
- Ensure that tenant IDs cannot be easily guessed or enumerated
- Log tenant switches and access patterns for security monitoring
