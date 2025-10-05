# Response Value Handlers

Command handlers can return values that need to be processed by the command pipeline. The Application Model provides a flexible system for handling these return values through Response Value Handlers.

## Automatic Response Handling

When a command handler returns a value, the command pipeline follows this logic:

1. **Check for Value Handlers**: The pipeline first checks if any registered response value handlers can handle the returned value using their `CanHandle` method
2. **Process with Handler**: If a value handler can handle the value, it processes the value and returns a `CommandResult`
3. **Automatic Response Creation**: If **no value handler** can handle the value, the pipeline automatically creates a `CommandResult<T>` with the returned value as the response

This means that **command handlers can return any type of value**, and it will either be processed by a specific handler or automatically become the command response.

## Built-in Value Handlers

Out-of-the-box the Cratis ApplicationModel comes with the following value handlers:

| Type | Description |
| ---- | ----------- |
| ValidationResultResponseValueHandler | Responds to Cratis ValidationResult object and adds it to the command result |

## Command Handler Return Patterns

### Single Value Return

```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task<object?> Handle(CommandContext context)
    {
        var userId = Guid.NewGuid();
        // This will automatically create CommandResult<Guid> with userId as response
        return Task.FromResult<object?>(userId);
    }
}
```

### OneOf Return

```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task<object?> Handle(CommandContext context)
    {
        var result = OneOf<UserId, ValidationResult>.FromT0(new UserId(Guid.NewGuid()));
        // If no handler can process UserId, it becomes CommandResult<UserId>
        // If ValidationResultResponseValueHandler processes ValidationResult, it affects the command result
        return Task.FromResult<object?>(result);
    }
}
```

### Tuple Return

```csharp
public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    public Task<object?> Handle(CommandContext context)
    {
        var userId = new UserId(Guid.NewGuid());
        var auditInfo = new AuditInfo(DateTime.UtcNow, "system");
        
        // Each value is checked against available handlers
        // If no handler processes userId, it becomes the response
        // If a handler processes auditInfo, it affects the command result
        return Task.FromResult<object?>((userId, auditInfo));
    }
}
```

## Creating Custom Value Handlers

You can create custom response value handlers by implementing the `ICommandResponseValueHandler` interface:

```csharp
public class AuditInfoResponseValueHandler : ICommandResponseValueHandler
{
    public bool CanHandle(CommandContext commandContext, object value)
    {
        return value is AuditInfo;
    }

    public Task<CommandResult> Handle(CommandContext commandContext, object value)
    {
        var auditInfo = (AuditInfo)value;
        
        // Perform audit logging
        LogAuditEvent(auditInfo);
        
        // Return success - this doesn't affect the command response
        return Task.FromResult(CommandResult.Success(commandContext.CorrelationId));
    }
}
```

The Application Model will automatically discover and register custom value handlers in the command pipeline.

## Response Object Availability

When implementing a command response value handler, the `CommandContext.Response` property contains the response object returned by the command handler, **if any**. This property can be `null` in the following scenarios:

- The command handler didn't return anything (void method)
- The command handler returned `null`
- The command handler returned a single value that has no corresponding value handler (in which case that value becomes the automatic response)

### Tuple Processing Behavior

When a command handler returns a tuple, the command pipeline intelligently processes each value:

1. **Each value is evaluated** against all available response value handlers using their `CanHandle` method
2. **Values with handlers** are processed by their respective response value handlers
3. **Values without handlers** are considered potential response values:
   - If exactly **one value** has no handler, it becomes the response (available in `CommandContext.Response`)
   - If **multiple values** have no handlers, a `MultipleUnhandledTupleValuesException` is thrown
   - If **all values** have handlers, no response is set (`CommandContext.Response` remains `null`)

### OneOf Processing Behavior

When a command handler returns a `OneOf<T1, T2, ...>` value:

1. **The inner value** is extracted from the OneOf wrapper
2. **Value handlers are checked** using the `CanHandle` method on the inner value
3. **If a handler can process it**, the handler processes the value
4. **If no handler can process it**, the inner value automatically becomes a `CommandResult<T>` response

## Key Benefits

- **Zero Configuration**: Values without handlers automatically become responses
- **Flexible Processing**: Custom handlers can perform side effects (logging, notifications, etc.)
- **Type Safety**: Automatic responses are properly typed as `CommandResult<T>`
- **Backward Compatibility**: Existing value handlers continue to work as before

### Example Implementation

```csharp
public class MyResponseValueHandler : ICommandResponseValueHandler
{
    public bool CanHandle(CommandContext commandContext, object value)
    {
        // The commandContext.Response can be null here
        return value is MyValueType;
    }

    public Task<CommandResult> Handle(CommandContext commandContext, object value)
    {
        // Access the response if available
        var response = commandContext.Response; // This can be null
        
        if (response is not null)
        {
            // Handle cases where the command returned a response
            // This typically happens when the command returns a tuple with unhandled values
        }
        
        // Process the value that was returned by the command
        var myValue = (MyValueType)value;
        
        // Perform side effects (logging, notifications, etc.)
        ProcessValue(myValue);
        
        return Task.FromResult(CommandResult.Success(commandContext.CorrelationId));
    }
}
```

## Migration Notes

### From Previous Versions

If you were previously relying on the requirement that **all returned values must have corresponding value handlers**, this is no longer necessary. Values without handlers will now automatically become command responses, which provides:

- **Simplified Development**: No need to create handlers for simple response values
- **Better Developer Experience**: Commands can return domain objects directly
- **Reduced Boilerplate**: Less code needed for basic response scenarios

### Backward Compatibility

All existing value handlers continue to work exactly as before. The new automatic response creation only applies to values that have no corresponding handlers, ensuring full backward compatibility.
