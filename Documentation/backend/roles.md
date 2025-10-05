# Roles

The `RolesAttribute` is a convenient wrapper around ASP.NET Core's `AuthorizeAttribute` that simplifies working with role-based authorization. It provides an intuitive way to specify multiple roles for commands, queries, and controllers.

## Overview

The `RolesAttribute` eliminates the need to manually format role strings when using multiple roles with the standard `AuthorizeAttribute`. Instead of writing `[Authorize(Roles = "Admin,Manager")]`, you can use the more readable `[Roles("Admin", "Manager")]`.

## Basic Usage

### Commands

```csharp
using Cratis.Applications.Authorization;

[Roles("Admin")]
public record DeleteUser(string UserId);
```

### Queries

```csharp
using Cratis.Applications.Authorization;

[Roles("Admin", "Manager")]
public record GetUserDetails(string UserId);
```

Both examples are equivalent to using the standard ASP.NET Core authorization:

```csharp
[Authorize(Roles = "Admin")]
public record DeleteUser(string UserId);

[Authorize(Roles = "Admin,Manager")]
public record GetUserDetails(string UserId);
```

## Multiple Roles

The `RolesAttribute` makes it easy to specify multiple roles. A user needs to have **at least one** of the specified roles to be authorized:

### For Commands

```csharp
[Roles("Admin", "Manager", "TeamLead")]
public record ApproveRequest(int RequestId);
```

### For Queries

```csharp
[Roles("Admin", "Manager", "TeamLead", "Viewer")]
public record GetSensitiveReport(int ReportId);
```

This allows users with any of the specified roles to execute the command or query.

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
    
    [HttpGet("details/{id}")]
    [Roles("Viewer", "Editor", "Admin")]
    public async Task<IActionResult> GetProductDetails(string id)
    {
        // Requires "Viewer", "Editor" or "Admin" role for queries
    }
    
    [HttpPost]
    [Roles("Editor", "Admin")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
    {
        // Requires "Editor" or "Admin" role for commands
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
    
    [HttpGet("sensitive-data")]
    [Roles("Admin")] // Overrides controller-level authorization
    public async Task<IActionResult> GetSensitiveData()
    {
        // Requires "Admin" role only, not "Manager"
    }
    
    [HttpDelete("critical-data")]
    [Roles("Admin")] // Overrides controller-level authorization
    public async Task<IActionResult> DeleteCriticalData()
    {
        // Requires "Admin" role only, not "Manager"
    }
}
```

## Model-Bound Commands and Queries

The `RolesAttribute` works seamlessly with both model-bound commands and queries:

### Model-Bound Commands

```csharp
[Command]
[Roles("System", "Admin")]
public record CreateUser(
    string Name,
    string Email,
    int Age);
```

### Model-Bound Queries

```csharp
[Query]
[Roles("Manager", "Admin", "Auditor")]
public record GetUserAuditLog(
    string UserId,
    DateTime FromDate,
    DateTime ToDate);
```

## Integration with Command and Query Results

When using the `RolesAttribute`, authorization status is automatically included in both command and query results. You can check the authorization status:

### Command Results

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

### Query Results

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

- [Authorization](authorization.md) - Complete authorization documentation
- [Command Filters](commands/command-filters.md) - Including the AuthorizationFilter
- [Commands](commands/index.md) - Command documentation
- [Queries](queries/index.md) - Query documentation
- [Identity](identity.md) - Identity and authentication setup
