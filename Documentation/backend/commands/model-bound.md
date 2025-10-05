# Model Bound Commands

For a more lightweight approach; commands can be their own handlers.
This is achieved by adorning your command with the `[Command]` attribute
and implementing a `Handle()` method.

```csharp
[Command]  // The command attribute is needed
public record AddItemToCart(string Sku, int Quantity)
{
    public void Handle()
    {
        // Handle the command
    }
}
```

> **Note**: If you're using the Cratis ApplicationModel [proxy generator](../proxy-generation.md), the name of the type
> will become the name of the command for the generated TypeScript file and class.

If your handler has side-effects expressed in the return value, the
command pipeline has an [extensibility point for return values](./response-value-handlers.md).

You can then return anything you know there is a handler for.

## Discriminated Union

Your return type can leverage a discriminated union with [`OneOf<>`](https://github.com/mcintyre321/OneOf/) to
return different types of values depending on the situation, like for instance an explicit validation error.

As long as there are [response handlers](./response-value-handlers.md) for any of the types of the discriminated union, your value
will be handled.

```csharp
using Cratis.Applications.Validation;
using OneOf;

[Command]
public record AddItemToCart(string Sku, int Quantity)
{
    public OneOf<Guid, ValidationResult> Handle()
    {
        if( /* code that checks if product is carried */ )
        {
            var cartLineIdentifier = Guid.NewGuid();

            // Logic for handling the actual adding...

            return cartLineIdentifier;
        }

        return new ValidationResult(ValidationResultSeverity.Error, "Product is not carried anymore", [], null!);
    }
}
```

## Tuple

Sometimes you want to return a value that is part of the `CommandResult` and returned to the
caller that invoked the command. By returning a tuple, the command pipeline will use the
first value of the tuple as the value for the `CommandResult`, the second value is then considered
will then be the value, which can then be a discriminated union as shown earlier, or a direct
value that will then invoke any relevant [response handler](./response-value-handlers.md).

```csharp
using Cratis.Applications.Validation;

[Command]
public record AddItemToCart(string Sku, int Quantity)
{
    public (Guid, ItemAddedToCart) Handle()
    {
        var cartLineIdentifier = Guid.NewGuid();

        // Logic for handling the actual adding...

        // Return the identifier and the consequence, in this case an event handled by Chronicle.
        return (cartLineIdentifier, new ItemAddedToCart(...);
    }
}
```

## Dependencies

Your command handler method can also take dependencies to any services configured in the
service collection. This is done by just specifying your dependencies on the methods signature:

```csharp
using Cratis.Applications.Validation;

[Command]
public record AddItemToCart(string Sku, int Quantity)
{
    public void Handle(ICartService carts)
    {
        carts.AddItemToCart(ski, quantity);
    }
}
```

## Authorization

Model-bound commands support authorization through standard ASP.NET Core authorization attributes as well as the convenient `[Roles]` attribute provided by the Application Model.

### Using the Authorize Attribute

You can secure commands using the standard `[Authorize]` attribute:

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

### Using the Roles Attribute

The Application Model provides a more convenient `[Roles]` attribute that allows for cleaner syntax when specifying multiple roles:

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
```

The user needs to have **at least one** of the specified roles to execute the command.

### Authorization Results

When authorization fails, the command pipeline automatically returns an unauthorized result. The command's `Handle()` method will not be executed:

```csharp
var result = await commandManager.Execute(new DeleteUserCommand("user123"));

if (!result.IsAuthorized)
{
    // Handle unauthorized access - command was not executed
    return Forbid();
}

if (result.IsSuccess)
{
    // Command executed successfully
    return Ok(result);
}
```

### Policy-Based Authorization

For more complex authorization scenarios, you can use policy-based authorization:

```csharp
[Command]
[Authorize(Policy = "RequireAdminOrOwner")]
public record UpdateUserProfile(string UserId, UserProfileData Data)
{
    public void Handle(IUserService userService)
    {
        userService.UpdateProfile(UserId, Data);
    }
}
```

> **Note**: Authorization is evaluated before the command's `Handle()` method is called. If authorization fails, the command will not be executed and the result will indicate the authorization failure.

## Frontend Integration

Model-bound commands work seamlessly with the [proxy generator](../proxy-generation.md), which automatically creates TypeScript proxies for your commands. The generated proxies provide:

- Strong typing for command properties
- Automatic validation integration
- React hooks for easy frontend integration
- Consistent error handling and response processing
- Authorization status handling in command results
