# Base Db Context

With the `BaseDbContext` you get something that pre-configures common conventions and converters, like the [JSON](./json.md)
support. It will then apply the common conventions and converters to the entities of the `DbContext`.

All you need to do is inherit from it and register it as you'd do with any other `DbContext`.

Take the following `DbContext`

```csharp
using Cratis.Applications.EntityFrameworkCore;

public class StoreDbContext : BaseDbContext
{
    public DbSet<Customer> Customers { get; set; }
}
```

Then you register it as you normally would:

```csharp
services.AddDbContext<StoreDbContext>(opt => ...);
```