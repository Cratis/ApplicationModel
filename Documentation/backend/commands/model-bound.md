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

## Frontend Integration

Model-bound commands work seamlessly with the [proxy generator](../proxy-generation.md), which automatically creates TypeScript proxies for your commands. The generated proxies provide:

- Strong typing for command properties
- Automatic validation integration
- React hooks for easy frontend integration
- Consistent error handling and response processing
