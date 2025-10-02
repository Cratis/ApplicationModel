# Controller Based Commands

You can represent commands as regular ASP.NET Core Controller actions.

```csharp
public record AddItemToCart(string sku, int quantity);

[Route("api/carts")]
public class Carts : ControllerBase
{
    [HttpPost("add")]
    public Task AddItemToCart([FromBody] AddItemToCart command)
    {
        // Logic for handling...
    }
}
```

> **Note**: If you're using the Cratis ApplicationModel [proxy generator](../proxy-generation.md), the method name
> will become the command name for the generated TypeScript file and class.

## Bypassing Command Result Wrappers

By default, controller-based commands return results wrapped in a `CommandResult` structure. If you need to return the raw result from your controller action without this wrapper, you can use the `[AspNetResult]` attribute. For more details, see [Without wrappers](../without-wrappers.md).
