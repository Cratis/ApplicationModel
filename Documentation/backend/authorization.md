# Identity

The Application Model provides enhanced authorization capabilities that build upon ASP.NET Core's built-in authorization system. It offers role-based authorization through specialized attributes and integrates authorization state into command and query results.

## Setup

Ensure that authentication and authorization are enabled in your application pipeline:

```csharp
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
```

> Note: If you're interested in leveraging the Microsoft Identity way of working with identity,
> read more [here](./microsoft-identity.md)

## Role-Based Authorization

The Application Model provides a convenient `RolesAttribute` that simplifies role-based authorization for controllers and actions.
Note that this works without any of the Application Model support for identity, this is just a wrapper to make it more convenient to work with roles.

### Roles Attribute

The `RolesAttribute` is a specialized authorization attribute that allows you to specify one or more roles required to access a controller or action:

```csharp
using Cratis.Applications.Authorization;

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

### Usage Patterns

#### Controller-Level Authorization

Apply roles to an entire controller to protect all actions:

```csharp
[Roles("Admin")]
public class AdminController : ControllerBase
{
    // All actions in this controller require "Admin" role
}
```

#### Action-Level Authorization

Apply roles to specific actions for fine-grained control:

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

#### Multiple Roles

Users must have at least one of the specified roles to access the resource:

```csharp
[Roles("Manager", "TeamLead", "Admin")]
public async Task<IActionResult> ApproveRequest(ApproveRequestCommand command)
{
    // User needs any one of: Manager, TeamLead, or Admin roles
}
```

## Authorization Integration

The Application Model integrates authorization state into command and query results, allowing you to handle authorization failures gracefully.

### Command Authorization

Commands automatically include authorization status in their results:

```csharp
public async Task<IActionResult> ProcessCommand(SomeCommand command)
{
    var result = await _commandManager.Execute(command);
    
    if (!result.IsAuthorized)
    {
        return Forbid(); // HTTP 403
    }
    
    if (!result.IsSuccess)
    {
        return BadRequest(result);
    }
    
    return Ok(result);
}
```

### Query Authorization

Queries also include authorization information:

```csharp
public async Task<IActionResult> GetData(SomeQuery query)
{
    var result = await _queryManager.Execute(query);
    
    if (!result.IsAuthorized)
    {
        return Forbid();
    }
    
    return Ok(result);
}
```

## Claims-Based Authorization

For more complex authorization scenarios, you can use standard ASP.NET Core claims-based authorization alongside the Application Model:

```csharp
[Authorize(Policy = "RequireAdminOrOwner")]
public async Task<IActionResult> UpdateResource(UpdateResourceCommand command)
{
    // Custom policy can check multiple claims, roles, and requirements
}
```

## Custom Authorization

### Custom Authorization Policies

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

## Best Practices

### Role Naming

- Use descriptive role names that reflect business functions (e.g., "AccountManager", "ContentEditor")
- Avoid generic names like "User1", "Level2"
- Consider using a consistent naming convention across your application

### Granular Permissions

- Apply authorization at the appropriate level (controller vs. action)
- Use action-level authorization for fine-grained control
- Consider the principle of least privilege

### Error Handling

- Always check authorization status in your command/query results
- Provide meaningful error messages while avoiding information disclosure
- Log authorization failures for security monitoring

### Testing Authorization

- Write tests that verify authorization behavior
- Test both positive (authorized) and negative (unauthorized) scenarios
- Include edge cases like missing roles or malformed tokens

```csharp
[Fact]
public async Task should_deny_access_when_user_lacks_required_role()
{
    // Arrange
    var command = new RestrictedCommand();
    var context = CreateContextWithoutAdminRole();
    
    // Act
    var result = await _handler.Handle(command, context);
    
    // Assert
    result.IsAuthorized.ShouldBeFalse();
}
```

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
