# Roles

The `RolesAttribute` is a convenient wrapper around ASP.NET Core's `AuthorizeAttribute` that simplifies working with role-based authorization. It provides an intuitive way to specify multiple roles for commands and controllers.

## Overview

The `RolesAttribute` eliminates the need to manually format role strings when using multiple roles with the standard `AuthorizeAttribute`. Instead of writing `[Authorize(Roles = "Admin,Manager")]`, you can use the more readable `[Roles("Admin", "Manager")]`.

## Basic Usage

```csharp
using Cratis.Applications.Authorization;

[Roles("Admin")]
public record DeleteUser(string UserId);
```

This is equivalent to:

```csharp
[Authorize(Roles = "Admin")]
public record DeleteUser(string UserId);
```

## Multiple Roles

The `RolesAttribute` makes it easy to specify multiple roles. A user needs to have **at least one** of the specified roles to be authorized:

```csharp
[Roles("Admin", "Manager", "TeamLead")]
public record ApproveRequest(int RequestId);
```

This allows users with any of the three roles (Admin, Manager, or TeamLead) to execute the command.

## Controller-Level Authorization

You can apply the `RolesAttribute` at the controller level to protect all actions:

```csharp
[Route("api/admin")]
[Roles("Admin")]
public class AdminController : ControllerBase
{
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command)
    {
        // Only users with "Admin" role can access this
    }
    
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        // Also requires "Admin" role
    }
}
```

## Action-Level Authorization

For fine-grained control, apply roles to specific actions:

```csharp
[Route("api/products")]
public class ProductController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        // No authorization required - public endpoint
    }
    
    [HttpPost]
    [Roles("Editor", "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
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

## Overriding Controller-Level Roles

Action-level `RolesAttribute` overrides controller-level authorization:

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
    
    [HttpDelete("critical-data")]
    [Roles("Admin")] // Overrides controller-level authorization
    public async Task<IActionResult> DeleteCriticalData()
    {
        // Requires "Admin" role only, not "Manager"
    }
}
```

## Model-Bound Commands

The `RolesAttribute` works seamlessly with model-bound commands:

```csharp
[Command]
[Roles("System", "Admin")]
public record CreateUser(
    string Name,
    string Email,
    int Age);
```

## Integration with Command Results

When using the `RolesAttribute`, authorization status is automatically included in command results. You can check the authorization status:

```csharp
var result = await mediator.Send(new CreateUserCommand("John", "john@example.com", 30));

if (!result.IsAuthorized)
{
    // Handle unauthorized access
    return Unauthorized();
}

if (result.IsSuccess)
{
    // Command executed successfully
}
```

## Best Practices

### Role Naming

- Use descriptive role names that reflect business functions (e.g., "AccountManager", "ContentEditor")
- Avoid generic names like "User1", "Level2"
- Maintain consistency across your application

### Granular Authorization

- Apply authorization at the appropriate level (controller vs. action)
- Use action-level authorization for fine-grained control
- Follow the principle of least privilege

### Error Handling

- Always check authorization status in command results
- Provide meaningful error messages while avoiding information disclosure
- Log authorization failures for security monitoring

## Relationship to ASP.NET Core Authorization

The `RolesAttribute` is built on top of ASP.NET Core's authorization system and is fully compatible with:

- Claims-based authorization
- Policy-based authorization
- Custom authorization handlers
- Identity providers

For more complex authorization scenarios, you can combine `RolesAttribute` with standard ASP.NET Core authorization features or use custom authorization policies.

## See Also

- [Authorization](../authorization.md) - Complete authorization documentation
- [Command Filters](command-filters.md) - Including the AuthorizationFilter
- [Identity](../identity.md) - Identity and authentication setup
