# Commands

The Cratis Application Model provides comprehensive TypeScript/JavaScript support for commands, enabling seamless integration between your frontend and backend through type-safe, automatically generated proxies. Commands represent actions that modify system state and are executed as HTTP POST operations against your backend controllers.

## Overview

The frontend command system provides:

- **Type-safe interfaces** for commands
- **HTTP integration** using the Fetch API
- **Change tracking** for commands
- **Configuration flexibility** for different environments
- **Microservice support** for distributed architectures
- **Property change notifications** for reactive UI updates

## ICommand Interface

The core command interface provides the following capabilities:

```typescript
interface ICommand<TCommandContent = object, TCommandResponse = object> {
    readonly route: string;
    execute(): Promise<CommandResult<TCommandResponse>>;
    clear(): void;
    setInitialValues(values: TCommandContent): void;
    setInitialValuesFromCurrentValues(): void;
    revertChanges(): void;
    hasChanges: boolean;
    onPropertyChanged(callback: PropertyChanged, thisArg?: any): void;
}
```

## Key Features

### Change Tracking

Commands automatically track changes to their properties, allowing you to:

- Detect when data has been modified (`hasChanges` property)
- Revert changes to initial values (`revertChanges()`)
- Set baseline values for comparison (`setInitialValues()`)

### Property Change Notifications

Commands support property change callbacks, enabling reactive UI updates:

```typescript
command.onPropertyChanged((property: string) => {
    console.log(`Property ${property} changed`);
});
```

### Execution and Results

Commands return a `CommandResult<TCommandResponse>` that includes:

- Success/failure status
- Validation errors
- Response data
- Exception details

## Integration with Backend

The frontend command system is designed to work seamlessly with the backend through:

### Controller-Based Commands

Backend commands are implemented as controller actions that handle HTTP POST endpoints to modify state.

For detailed information about implementing backend commands, see [Backend Commands](../../backend/commands/index.md).

### Automatic Proxy Generation

The most powerful feature of this system is the automatic generation of TypeScript proxies from your backend controllers. This eliminates the need for:

- Manual HTTP client code
- Type definitions that can become out of sync
- Consulting API documentation for parameter requirements

**Key Benefits:**

- **Compile-time type safety**: Catch integration errors at build time
- **IntelliSense support**: Get autocomplete and parameter hints in your IDE
- **Automatic updates**: Proxies regenerate when backend changes
- **Zero maintenance**: No manual synchronization between frontend and backend

For comprehensive information about setting up and configuring proxy generation, see [Proxy Generation](../../backend/proxy-generation.md).

## Configuration

Commands support configuration for different deployment scenarios:

### Microservice Configuration

```typescript
command.setMicroservice('user-service');
```

### API Base Path Configuration

```typescript
command.setApiBasePath('/api/v1');
```

## Error Handling

The system provides comprehensive error handling for commands:

### Command Errors

- **Validation errors**: Server-side validation failures
- **Network errors**: Connection issues and timeouts
- **HTTP errors**: 404, 500, and other status codes
- **Custom exceptions**: Application-specific error responses

## Best Practices

### Command Usage

1. **Clear commands** after successful execution to reset state
2. **Set initial values** when loading existing data for editing
3. **Monitor hasChanges** to enable/disable save buttons
4. **Handle validation errors** gracefully in the UI

### Performance Considerations

1. **Use change tracking** to avoid unnecessary command executions
2. **Implement proper error boundaries** for network failures
3. **Clear commands** after successful execution to free memory

## Next Steps

- Explore [React integration](../react/commands.md) for React-specific command usage
- Learn about [Queries](./queries.md) for data retrieval
- Understand [MVVM patterns](../react.mvvm/index.md) for more sophisticated frontend architectures
- Set up [Proxy Generation](../../backend/proxy-generation.md) to automatically generate your command proxies
