# Controller Based Commands

You can represent commands as regular ASP.NET Core Controller actions.

```csharp
public record AddItemToCart(string sku, int quantity);

[Route("api/carts)]
public class Carts : ControllerBase
{
    [HttpPost("add")]
    public Task AddItemToCart([FromBody] AddItemToCart command)
    {
        // Logic for handling...
    }
}
```

> Note: If you're using the Cratis ApplicationModel proxy generator, the method name
> will become the command name for the generated TypeScript file and class.
