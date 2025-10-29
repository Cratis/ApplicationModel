# Authorization

The Application Model provides enhanced authorization capabilities that build upon ASP.NET Core's built-in authorization system. It offers role-based authorization through specialized attributes and integrates authorization state into command and query results across controllers, model-bound commands, and queries.

## Setup

Ensure that authentication and authorization are enabled in your application pipeline:

```csharp
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```

> Note: If you're interested in leveraging the Microsoft Identity way of working with identity,
> read more about [Microsoft Identity integration](./microsoft-identity.md)

## Role-Based Authorization

The Application Model provides two convenient ways to implement role-based authorization:

1. **Standard ASP.NET Core `[Authorize]` attribute** - Works with all scenarios
2. **Convenient `[Roles]` attribute** - Simplifies multi-role scenarios with cleaner syntax

The `RolesAttribute` is a wrapper around ASP.NET Core's `AuthorizeAttribute` that eliminates the need to manually format role strings. Instead of writing `[Authorize(Roles = "Admin,Manager")]`, you can use the more readable `[Roles("Admin", "Manager")]`.

### Using the Authorize Attribute

Standard ASP.NET Core authorization works across all scenarios:

```csharp
using Microsoft.AspNetCore.Authorization;

// Single role
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase { }

// Multiple roles (user needs at least one)
[Authorize(Roles = "Admin,Manager")]
public record DeleteUser(string UserId);
```

### Using the Roles Attribute for Controllers

The `RolesAttribute` provides cleaner syntax for multiple roles:

```csharp
using Cratis.Applications.Authorization;

// Equivalent to [Authorize(Roles = "Admin,Manager")]
[Roles("Admin", "Manager")]
public class UserManagementController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateUser(CreateUserCommand command)
    {
        // Only users with "Admin" or "Manager" roles can access this endpoint
        // ...
    }
    
    [HttpDelete("{id}")]
    [Roles("Admin")] // Override controller-level roles for specific actions
    public async Task<IActionResult> DeleteUser(string id)
    {
        // Only users with "Admin" role can delete users
        // ...
    }
}
```

Users must have at least one of the specified roles to access the resource.

## Authorization in Controllers

### Controller-Level Authorization

Apply authorization to an entire controller to protect all actions:

```csharp
[Roles("Admin")]
public class AdminController : ControllerBase
{
    // All actions in this controller require "Admin" role
}
```

### Action-Level Authorization

Apply authorization to specific actions for fine-grained control:

```csharp
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        // No authorization required - public endpoint
    }
    
    [HttpPost]
    [Roles("Editor", "Admin")]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        // Requires "Editor" or "Admin" role
    }
    
    [HttpDelete("{id}")]
    [Roles("Admin")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        // Requires "Admin" role only
    }
}
```

### Overriding Controller-Level Authorization

Action-level authorization overrides controller-level settings:

```csharp
[Route("api/management")]
[Roles("Manager")]
public class ManagementController : ControllerBase
{
    [HttpGet("reports")]
    public async Task<IActionResult> GetReports()
    {
        // Requires "Manager" role (from controller)
    }
    
    [HttpGet("sensitive-data")]
    [Roles("Admin")] // Overrides controller-level authorization
    public async Task<IActionResult> GetSensitiveData()
    {
        // Requires "Admin" role only, not "Manager"
    }
}
```

## Authorization in Model-Bound Commands

Model-bound commands support authorization through both standard ASP.NET Core authorization attributes and the convenient `[Roles]` attribute.

### Using Standard Authorization

```csharp
[Command]
[Authorize]
public record DeleteUser(string UserId)
{
    public void Handle(IUserService userService)
    {
        userService.DeleteUser(UserId);
    }
}
```

For role-based authorization with the standard attribute:

```csharp
[Command]
[Authorize(Roles = "Admin,Manager")]
public record ApproveRequest(int RequestId)
{
    public void Handle(IRequestService requestService)
    {
        requestService.ApproveRequest(RequestId);
    }
}
```

### Using the Roles Attribute for Commands

The `[Roles]` attribute provides cleaner syntax for model-bound commands:

```csharp
[Command]
[Roles("Admin", "Manager")]
public record ApproveRequest(int RequestId)
{
    public void Handle(IRequestService requestService)
    {
        requestService.ApproveRequest(RequestId);
    }
}

[Command]
[Roles("System", "Admin")]
public record CreateUser(
    string Name,
    string Email,
    int Age)
{
    public void Handle(IUserService userService)
    {
        // Command implementation
    }
}
```

### Authorization Results for Commands

When authorization fails, the command pipeline automatically returns an unauthorized result. The command's `Handle()` method will not be executed:

```csharp
public class Users(ICommandPipeline commandPipeline)
{
    public async Task DeleteUser(string user)
    {
        var result = await commandPipeline.Execute(new DeleteUserCommand(user));

        if (!result.IsAuthorized)
        {
            // Handle unauthorized access - command was not executed
        }

        if (result.IsSuccess)
        {
            // Command executed successfully
        }
    }
}
```

## Authorization in Model-Bound Queries

Queries also support both authorization approaches for data protection:

### Using Standard Authorization for Queries

```csharp
[Query]
[Authorize(Roles = "Admin,Manager")]
public record GetUserAuditLog(
    string UserId,
    DateTime FromDate,
    DateTime ToDate);
```

### Using the Roles Attribute for Queries

```csharp
[Query]
[Roles("Manager", "Admin", "Auditor")]
public record GetUserAuditLog(
    string UserId,
    DateTime FromDate,
    DateTime ToDate);

[Query]
[Roles("Viewer", "Editor", "Admin")]
public record GetProductDetails(string ProductId);
```

### Authorization Results for Queries

Query results include authorization status that can be checked:

```csharp
var result = await mediator.Send(new GetUserAuditLogQuery("user123", DateTime.Now.AddDays(-30), DateTime.Now));

if (!result.IsAuthorized)
{
    // Handle unauthorized access
    return Unauthorized();
}

if (result.IsSuccess)
{
    // Query executed successfully, use result.Data
    var auditLog = result.Data;
}
```

## Authorization Integration

The Application Model integrates authorization state into command and query results, allowing you to handle authorization failures gracefully.

## Policy-Based Authorization

For more complex authorization scenarios, you can use standard ASP.NET Core policy-based authorization alongside the Application Model:

```csharp
[Command]
[Authorize(Policy = "RequireAdminOrOwner")]
public record UpdateResource(string ResourceId, ResourceData Data)
{
    public void Handle()
    {
        // Custom policy can check multiple claims, roles, and requirements
    }
}
```

You can define custom authorization policies in your service configuration:

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminOrOwner", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") ||
            context.User.HasClaim("resource", "owner")));
});
```

## Custom Authorization

### Authorization Filters

The Application Model includes authorization filters that integrate with the command and query pipeline:

```csharp
// Custom authorization logic can be implemented through command filters
public class CustomAuthorizationFilter : ICommandFilter
{
    public Task<CommandResult> OnExecution(CommandContext context)
    {
        // Custom authorization logic
        if (!IsAuthorized(context))
        {
            return Task.FromResult(CommandResult.Error(context.CorrelationId, "Unauthorized"));
        }
        
        return Task.FromResult(CommandResult.Success(context.CorrelationId));
    }
}
```

For queries, you can implement custom authorization through query filters:

```csharp
public class QueryAuthorizationFilter : IQueryFilter
{
    public Task<QueryResult> OnPerform(QueryContext context)
    {
        // Custom authorization logic for queries
        if (!IsAuthorized(context))
        {
            return Task.FromResult(QueryResult.Unauthorized(context.CorrelationId, "Access denied"));
        }
        
        return Task.FromResult(QueryResult.Success(context.CorrelationId));
    }
}
```

## Built-in Authorization Filter

The Application Model provides a built-in `AuthorizationFilter` that automatically handles both `[Authorize]` and `[Roles]` attributes for commands and queries:

- **Authentication**: Verifies user is authenticated
- **Role-based authorization**: Checks required roles if specified
- **Policy-based authorization**: Evaluates custom policies
- **Automatic result handling**: Returns appropriate unauthorized results

This filter is automatically registered and executes before command handlers and query renderers.

## Best Practices

### Role Naming

- Use descriptive role names that reflect business functions (e.g., "AccountManager", "ContentEditor")
- Avoid generic names like "User1", "Level2"  
- Consider using a consistent naming convention across your application

### Granular Permissions

- Apply authorization at the appropriate level (controller vs. action vs. command/query)
- Use action-level and command/query-level authorization for fine-grained control
- Consider the principle of least privilege

### Error Handling

- Always check authorization status in your command/query results
- Provide meaningful error messages while avoiding information disclosure
- Log authorization failures for security monitoring

### Authorization Architecture

- Use controller-level authorization for protecting entire API surfaces
- Use model-bound command/query authorization for business logic protection
- Combine both approaches when you need different authorization rules for different access patterns

## Integration with Identity

Authorization works seamlessly with the [Identity](./identity.md) system. User roles are automatically extracted from the identity token and made available for authorization decisions. The identity provider context includes role information that can be used for authorization:

```csharp
public class IdentityDetailsProvider : IProvideIdentityDetails
{
    public Task<IdentityDetails> Provide(IdentityProviderContext context)
    {
        var userRoles = context.Claims
            .Where(c => c.Key == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();
            
        var isAuthorized = userRoles.Contains("Admin") || userRoles.Contains("User");
        
        return Task.FromResult(new IdentityDetails(isAuthorized, new { Roles = userRoles }));
    }
}
```

## Frontend Integration

Authorization attributes work seamlessly with the [proxy generator](./proxy-generation.md), which automatically creates TypeScript proxies for your commands and queries. The generated proxies provide:

- Authorization status handling in command and query results
- Consistent error handling for unauthorized access
- Integration with frontend authentication systems
- Type-safe authorization checking

## See Also

- [Commands](commands/index.md) - Command documentation including authorization
- [Model-Bound Commands](commands/model-bound.md) - Model-bound command authorization
- [Queries](queries/index.md) - Query documentation
- [Command Filters](commands/command-filters.md) - Including the AuthorizationFilter
- [Identity](identity.md) - Identity and authentication setup
- [Microsoft Identity](microsoft-identity.md) - Microsoft Identity integration
