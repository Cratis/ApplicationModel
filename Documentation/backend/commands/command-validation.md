# Command Validation

The Application Model provides built-in support for validating commands without executing them. This enables pre-flight validation to provide early feedback to users before performing potentially expensive or state-changing operations.

## Overview

Command validation allows you to check authorization and validation rules without executing the command handler. This is useful for:

- **Early User Feedback**: Show validation errors before the user submits a form
- **UX Improvements**: Enable/disable submit buttons based on validation state
- **Authorization Checks**: Verify user permissions without side effects
- **Progressive Validation**: Validate fields as users interact with forms

## Backend Support

### ICommandPipeline.Validate

The `ICommandPipeline` interface provides a `Validate` method that runs only authorization and validation filters:

```csharp
public interface ICommandPipeline
{
    /// <summary>
    /// Validates the given command without executing it.
    /// </summary>
    Task<CommandResult> Validate(object command);
}
```

**Key Characteristics:**

- Runs all command filters (authorization, validation)
- Does **not** invoke the command handler
- Returns a `CommandResult` with validation and authorization status
- No side effects on the system

### Example Usage

```csharp
public class OrderService
{
    private readonly ICommandPipeline _commandPipeline;

    public OrderService(ICommandPipeline commandPipeline)
    {
        _commandPipeline = commandPipeline;
    }

    public async Task<bool> CanCreateOrder(CreateOrder command)
    {
        var result = await _commandPipeline.Validate(command);
        return result.IsSuccess;
    }

    public async Task CreateOrder(CreateOrder command)
    {
        // Optionally validate first
        var validationResult = await _commandPipeline.Validate(command);
        if (!validationResult.IsSuccess)
        {
            // Handle validation errors
            return;
        }

        // Execute the command
        var result = await _commandPipeline.Execute(command);
        // Process result...
    }
}
```

### Model-Bound Commands

For model-bound commands, validation endpoints are automatically created alongside execute endpoints:

**Execute Endpoint**: `POST /api/orders/create-order`
**Validate Endpoint**: `POST /api/orders/create-order/validate`

The validation endpoint accepts the same payload as the execute endpoint but only runs filters.

### Controller-Based Commands

For controller-based commands, validation endpoints are **automatically discovered and created** at application startup. The system scans all controller actions that:

- Are POST methods
- Have a single `[FromBody]` parameter (the command)

For each matching controller action, a corresponding `/validate` endpoint is automatically registered.

**Example Controller:**

```csharp
[Route("api/carts")]
public class Carts : ControllerBase
{
    [HttpPost("add")]
    public Task AddItemToCart([FromBody] AddItemToCart command)
    {
        // Execute the command
    }
}
```

**Automatically Created Endpoints:**

- **Execute**: `POST /api/carts/add`
- **Validate**: `POST /api/carts/add/validate` _(automatically created)_

**Key Points:**

- Validation endpoints are automatically created for all controller command actions
- No attributes or special configuration required
- The system detects commands by looking for POST actions with `[FromBody]` parameters
- The route pattern for validation is: `{controller-action-route}/validate`
- Only validation and authorization filters run; the action method is not executed

**How It Works:**

During application startup, the `ControllerCommandEndpointMapper` service:

1. Discovers all controller actions using `IActionDescriptorCollectionProvider`
2. Identifies command actions (POST methods with `[FromBody]` parameter)
3. Creates corresponding `/validate` endpoints using Minimal APIs
4. Routes validation requests through the command pipeline's `Validate` method

This approach provides the same validation functionality as model-bound commands without requiring developers to manually create validation actions.

## Frontend Support

### Command.validate() Method

All generated TypeScript command proxies include a `validate()` method:

```typescript
interface ICommand<TCommandContent, TCommandResponse> {
    /**
     * Validate the command without executing it.
     * Returns validation and authorization status.
     */
    validate(): Promise<CommandResult<TCommandResponse>>;
    
    /**
     * Execute the command.
     */
    execute(): Promise<CommandResult<TCommandResponse>>;
}
```

### React Usage

```typescript
import { CreateOrder } from './generated/commands';

function OrderForm() {
    const [command, setValues] = CreateOrder.use();
    const [validationErrors, setValidationErrors] = useState<string[]>([]);

    const handleFieldBlur = async () => {
        // Validate on field blur for early feedback
        const result = await command.validate();
        
        if (!result.isValid) {
            setValidationErrors(result.validationResults.map(v => v.message));
        } else {
            setValidationErrors([]);
        }
    };

    const handleSubmit = async () => {
        // Execute the command
        const result = await command.execute();
        
        if (result.isSuccess) {
            // Handle success
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input 
                value={command.orderNumber}
                onChange={e => command.orderNumber = e.target.value}
                onBlur={handleFieldBlur}
            />
            {validationErrors.map(error => (
                <div className="error">{error}</div>
            ))}
            <button type="submit">Create Order</button>
        </form>
    );
}
```

### Progressive Validation Example

```typescript
function ProductOrderForm() {
    const [command, setValues] = CreateOrder.use();
    const [canSubmit, setCanSubmit] = useState(false);

    useEffect(() => {
        // Validate whenever command properties change
        const validateCommand = async () => {
            const result = await command.validate();
            setCanSubmit(result.isSuccess);
        };

        validateCommand();
    }, [command.hasChanges]);

    return (
        <form>
            <input 
                value={command.productId}
                onChange={e => command.productId = e.target.value}
            />
            <input 
                value={command.quantity}
                onChange={e => command.quantity = parseInt(e.target.value)}
            />
            <button 
                type="submit" 
                disabled={!canSubmit}
                onClick={() => command.execute()}
            >
                Create Order
            </button>
        </form>
    );
}
```

## Validation Filters

The `validate()` method runs all registered command filters:

### Built-in Filters

1. **AuthorizationFilter**: Checks user permissions
2. **DataAnnotationValidationFilter**: Validates data annotations
3. **FluentValidationFilter**: Runs FluentValidation validators

For more information, see [Command Filters](./command-filters.md).

## CommandResult Structure

Both `execute()` and `validate()` return the same `CommandResult` structure:

```typescript
interface CommandResult<TResponse> {
    correlationId: string;
    isSuccess: boolean;        // Overall success (authorized + valid + no exceptions)
    isAuthorized: boolean;     // Authorization status
    isValid: boolean;          // Validation status
    hasExceptions: boolean;    // Whether exceptions occurred
    validationResults: ValidationResult[];
    exceptionMessages: string[];
    exceptionStackTrace: string;
    response?: TResponse;      // Only populated on execute()
}
```

**Note**: The `response` property will be `null` or `undefined` when using `validate()` since the handler is not executed.

## Best Practices

### When to Use Validate

✅ **Good Use Cases:**

- Form validation as users type or blur fields
- Enabling/disabling submit buttons based on validation state
- Showing validation messages before submission
- Checking authorization before showing UI elements

❌ **Avoid:**

- Calling validate() immediately before execute() (execute already validates)
- Over-validating (don't validate on every keystroke for performance)
- Using validate() as a substitute for client-side validation

### Performance Considerations

- Validation makes a server round-trip, so use judiciously
- Consider debouncing validation calls for real-time feedback
- Client-side validation is still important for immediate feedback
- Server validation ensures security and data integrity

### Example: Debounced Validation

```typescript
import { useMemo } from 'react';
import { debounce } from 'lodash';

function OrderForm() {
    const [command] = CreateOrder.use();

    const debouncedValidate = useMemo(
        () => debounce(async () => {
            const result = await command.validate();
            // Update UI with validation results
        }, 500),
        []
    );

    useEffect(() => {
        if (command.hasChanges) {
            debouncedValidate();
        }
    }, [command.orderNumber, command.quantity]);

    return (/* form UI */);
}
```

## Security Considerations

- Validation endpoints run the same authorization filters as execute endpoints
- Unauthorized users receive 401/403 responses from validation endpoints
- Validation does not expose sensitive data since handlers aren't executed
- Validation results may reveal authorization policies (by design)

## Troubleshooting

### Validation endpoint returns 404

**Cause**: The validation endpoint is only created for model-bound commands.

**Solution**: For controller-based commands, call the execute endpoint and handle validation results, or implement a custom validation endpoint.

### Validation passes but execute fails

**Cause**: State may have changed between validate and execute calls, or the handler encountered an error.

**Solution**: This is expected behavior. Always check the result of `execute()` for the authoritative status.

### Validation is slow

**Cause**: Complex validation logic or database queries in validators.

**Solution**:

- Debounce validation calls
- Optimize validator implementations
- Consider client-side validation for immediate feedback
