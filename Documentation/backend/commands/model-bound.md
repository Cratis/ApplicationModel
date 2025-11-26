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

> **Note**: If you're using the Cratis Arc [proxy generator](../proxy-generation.md), the name of the type
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
using Cratis.Arc.Validation;
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
caller that invoked the command. By returning a tuple, the command pipeline will intelligently
process each value to determine which should be the response and which should be processed by
[response value handlers](./response-value-handlers.md).

### How Tuple Processing Works

The command pipeline processes tuples as follows:

1. **Checks each value** against available response value handlers using their `CanHandle` method
2. **Values with handlers** are processed by their respective response value handlers
3. **Values without handlers** are considered potential response values
4. **If exactly one value has no handler**, it becomes the response in the `CommandResult`
5. **If multiple values have no handlers**, a `MultipleUnhandledTupleValuesException` is thrown
6. **If all values have handlers**, no response value is set

### Simple Tuple (2 values)

```csharp
using Cratis.Arc.Validation;

[Command]
public record AddItemToCart(string Sku, int Quantity)
{
    public (Guid, ItemAddedToCart) Handle()
    {
        var cartLineIdentifier = Guid.NewGuid();

        // Logic for handling the actual adding...

        // Return the identifier and the consequence, in this case an event handled by Chronicle.
        return (cartLineIdentifier, new ItemAddedToCart(...));
    }
}
```

In this example, if `ItemAddedToCart` has a response value handler but `Guid` doesn't, then the `Guid` becomes the response.

### Multi-dimensional Tuples (3+ values)

The system supports tuples with any number of values:

```csharp
[Command]
public record ProcessOrder(string OrderId)
{
    public (Guid, OrderProcessed, ValidationResult, NotificationSent) Handle()
    {
        var confirmationId = Guid.NewGuid();
        
        // Processing logic...
        
        return (
            confirmationId,           // Response (if no handler exists for Guid)
            new OrderProcessed(...),  // Event (handled by event handler)
            validationResult,         // Validation (handled by validation handler)
            new NotificationSent(...) // Notification (handled by notification handler)
        );
    }
}
```

In this example:

- `OrderProcessed`, `ValidationResult`, and `NotificationSent` would be processed by their respective handlers
- `Guid` (confirmationId) would become the response value
- If multiple values lack handlers, an exception would be thrown

### Error Scenarios

If your tuple contains multiple values that don't have corresponding response value handlers, the system will throw a `MultipleUnhandledTupleValuesException` with details about which values couldn't be handled:

```csharp
// This would throw an exception if neither string nor int have handlers
public (string, int, SomeEvent) Handle() => ("response1", 42, new SomeEvent());
```

## Dependencies

Your command handler method can also take dependencies to any services configured in the
service collection. This is done by just specifying your dependencies on the methods signature:

```csharp
using Cratis.Arc.Validation;

[Command]
public record AddItemToCart(string Sku, int Quantity)
{
    public void Handle(ICartService carts)
    {
        carts.AddItemToCart(ski, quantity);
    }
}
```

## Authorization in Model-Bound Commands

Model-bound commands support comprehensive authorization mechanisms. For detailed information about securing your commands, see the [Authorization](../authorization.md) documentation.

## Programmatic Command Execution with ICommandPipeline

While model-bound commands are typically executed through HTTP endpoints, you can also execute them programmatically using the `ICommandPipeline` service. This is useful for scenarios such as:

- Background services or scheduled tasks
- Event handlers that need to execute commands
- Internal service-to-service communication
- Testing scenarios

### Basic Usage

Inject `ICommandPipeline` into your service and use it to execute commands:

```csharp
public class OrderProcessingService
{
    private readonly ICommandPipeline _commandPipeline;

    public OrderProcessingService(ICommandPipeline commandPipeline)
    {
        _commandPipeline = commandPipeline;
    }

    public async Task ProcessOrder(Order order)
    {
        var command = new ProcessOrderCommand(order.Id, order.Items);
        var result = await _commandPipeline.Execute(command);

        if (result.IsSuccess)
        {
            // Command executed successfully
            var orderId = result.Response; // If the command returns a value
        }
        else
        {
            // Handle validation errors or other failures
            foreach (var error in result.ValidationResults)
            {
                // Process validation errors
            }
        }
    }
}
```

### Command Results

The `ICommandPipeline.Execute()` method returns a `CommandResult` that provides information about the execution:

```csharp
var result = await _commandPipeline.Execute(command);

// Check if the command was authorized
if (!result.IsAuthorized)
{
    // Handle unauthorized access
}

// Check if the command executed successfully
if (result.IsSuccess)
{
    // Access the response value if the command returns one
    var responseValue = result.Response;
}
else
{
    // Handle validation errors
    foreach (var validationResult in result.ValidationResults)
    {
        // Process each validation error
    }
}
```

### Exception Handling

When using `ICommandPipeline` programmatically, exceptions in the command handler are caught and returned as part of the `CommandResult`:

```csharp
var result = await _commandPipeline.Execute(command);

if (result.HasExceptions)
{
    // An exception was thrown during command execution
    foreach (var message in result.ExceptionMessages)
    {
        // Handle message
    }
}
```

### Context and Authentication

When executing commands programmatically, the current execution context (including user identity and claims) is automatically used. If you need to execute commands under a different context, you'll need to manage the authentication context appropriately in your application.

## Frontend Integration

Model-bound commands work seamlessly with the [proxy generator](../proxy-generation.md), which automatically creates TypeScript proxies for your commands. The generated proxies provide:

- Strong typing for command properties
- Automatic validation integration
- React hooks for easy frontend integration
- Consistent error handling and response processing
- Authorization status handling in command results
