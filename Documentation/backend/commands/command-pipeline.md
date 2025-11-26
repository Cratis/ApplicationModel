# Command Pipeline

The `ICommandPipeline` service provides a way to execute commands programmatically, bypassing the HTTP layer. This is useful for scenarios where you need to execute commands from within your application code rather than through HTTP requests.

## When to Use ICommandPipeline

The command pipeline is particularly useful for:

- **Background services or scheduled tasks** - Execute commands as part of scheduled jobs
- **Event handlers** - React to events by executing commands
- **Internal service-to-service communication** - Execute commands between services without HTTP overhead
- **Testing scenarios** - Execute commands directly in integration tests
- **Saga or workflow orchestration** - Coordinate multiple commands as part of a larger workflow

## Basic Usage

Inject `ICommandPipeline` into your service and use it to execute commands:

```csharp
public class OrderProcessingService
{
    readonly ICommandPipeline _commandPipeline;

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

## Command Results

The `ICommandPipeline.Execute()` method returns a `CommandResult` that provides comprehensive information about the execution:

```csharp
var result = await _commandPipeline.Execute(command);

// Check if the command was authorized
if (!result.IsAuthorized)
{
    // Handle unauthorized access
    // The command was not executed
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

### CommandResult Properties

| Property | Type | Description |
| -------- | ---- | ----------- |
| `IsSuccess` | `bool` | Whether the command executed successfully |
| `IsAuthorized` | `bool` | Whether the user was authorized to execute the command |
| `IsValid` | `bool` | Whether the command passed validation |
| `HasExceptions` | `bool` | Whether any exceptions occurred during execution |
| `Response` | `object?` | The response value returned by the command handler |
| `ValidationResults` | `IEnumerable<ValidationResult>` | Validation errors if the command failed validation |
| `ExceptionMessages` | `IEnumerable<string>` | Exception messages if exceptions occurred |
| `CorrelationId` | `CorrelationId` | The correlation ID for tracking the command |

## Exception Handling

When using `ICommandPipeline` programmatically, exceptions in the command handler are caught and returned as part of the `CommandResult`:

```csharp
var result = await _commandPipeline.Execute(command);

if (result.HasExceptions)
{
    // An exception was thrown during command execution
    foreach (var message in result.ExceptionMessages)
    {
        _logger.LogError("Command failed: {Message}", message);
    }
}
```

## Validation Without Execution

You can validate a command without actually executing it using the `Validate` method:

```csharp
var validationResult = await _commandPipeline.Validate(command);

if (validationResult.IsValid)
{
    // Command is valid, proceed with execution if needed
    var result = await _commandPipeline.Execute(command);
}
else
{
    // Handle validation errors
    foreach (var error in validationResult.ValidationResults)
    {
        // Process validation error
    }
}
```

This is useful for pre-flight validation before committing to command execution.

## Context and Authentication

When executing commands programmatically, the current execution context (including user identity and claims) is automatically used. The command pipeline respects:

- **Correlation ID** - Automatically tracked for request tracing
- **User context** - The current user's identity and claims are used for authorization
- **Tenant context** - Multi-tenancy context is preserved

If you need to execute commands under a different context, you'll need to manage the authentication context appropriately in your application.

## Background Service Example

Here's an example of using `ICommandPipeline` in a background service:

```csharp
public class OrderExpirationService : BackgroundService
{
    readonly IServiceProvider _serviceProvider;
    readonly ILogger<OrderExpirationService> _logger;

    public OrderExpirationService(
        IServiceProvider serviceProvider,
        ILogger<OrderExpirationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var commandPipeline = scope.ServiceProvider.GetRequiredService<ICommandPipeline>();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

            var expiredOrders = await orderRepository.GetExpiredOrders();
            
            foreach (var order in expiredOrders)
            {
                var command = new ExpireOrder(order.Id);
                var result = await commandPipeline.Execute(command);
                
                if (!result.IsSuccess)
                {
                    _logger.LogWarning(
                        "Failed to expire order {OrderId}: {Errors}",
                        order.Id,
                        string.Join(", ", result.ValidationResults.Select(v => v.Message)));
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

## Event Handler Example

Using `ICommandPipeline` in an event handler:

```csharp
public class OrderCreatedEventHandler
{
    readonly ICommandPipeline _commandPipeline;

    public OrderCreatedEventHandler(ICommandPipeline commandPipeline)
    {
        _commandPipeline = commandPipeline;
    }

    public async Task Handle(OrderCreated @event)
    {
        // Send confirmation email when an order is created
        var command = new SendOrderConfirmation(@event.OrderId, @event.CustomerEmail);
        var result = await _commandPipeline.Execute(command);
        
        if (!result.IsSuccess)
        {
            // Handle failure - maybe queue for retry
        }
    }
}
```

## Typed Command Results

When commands return typed values, you can access them through the `Response` property:

```csharp
[Command]
public record CreateOrder(IEnumerable<OrderItem> Items)
{
    public OrderId Handle(IOrderService orderService)
    {
        return orderService.CreateOrder(Items);
    }
}

// Usage
var result = await _commandPipeline.Execute(new CreateOrder(items));

if (result.IsSuccess && result.Response is OrderId orderId)
{
    // Use the order ID
    await NotifyCustomer(orderId);
}
```
