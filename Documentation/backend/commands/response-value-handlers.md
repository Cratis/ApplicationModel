# Response Value Handlers

For values returned from a command handler to have any impact, there needs to be a
value handler for it.

Out-of-the-box the Cratis ApplicationModel comes with the following

| Type | Description |
| ---- | ----------- |
| ValidationResultResponseValueHandler | Responds to Cratis ValidationResult object and adds it to the command result |

## Creating your own

You can easily create your own by simply adding a class to your solution that implements the interface
`ICommandResponseValueHandler`.

The Cratis ApplicationModel will automatically discover it and use it in the command pipeline automatically.
With this interface you get need to implement `CanHandle`, which is called to figure out
if the value in the context of a command can be handled the handler. If it returns true,
the `Handle` method is then called. This method can then perform operations as it see fit and
also return a result which will be merged with result of any other parts of the command pipeline
and used as the result.

## Response Object Availability

When implementing a command response value handler, it's important to understand that the `CommandContext.Response` property contains the response object returned by the command handler, **if any**. This property can be `null` in the following scenarios:

- The command handler didn't return anything (void method)
- The command handler returned `null`
- The command handler returned a single value (not a tuple), in which case that value is passed directly to the value handlers and no response is set

When a command handler returns a tuple, the first item becomes the response (available in `CommandContext.Response`) and the second item is passed as the `value` parameter to your handler methods.

### Example

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
            // This typically happens when the command returns a tuple: (response, value)
        }
        
        // Process the value that was returned by the command
        var myValue = (MyValueType)value;
        
        return Task.FromResult(CommandResult.Success(commandContext.CorrelationId));
    }
}
```
