# Read Only DbContext

Typically in a CQRS model, your read models are not meant to be used for creating or updating state in the
database. They're meant to be read-only. In fact, you don't even want to take advantage of the EntityFramework change tracking.

There are 2 ways of doing read-only DbContexts; inheritance or using the registration methods that will configure without
having to use inheritance.

## ReadOnlyDbContext base class

The `ReadOnlyDbContext` base class gives you a base class that also inherits from the [`BaseDbContext`](./base-db-context.md) to
give you the common tools.

All you need to do for your `DbContext` is to inherit from it as shown below:

```csharp
using Cratis.Applications.EntityFrameworkCore;

public class StoreDbContext : ReadOnlyDbContext
{
    public DbSet<Customer> Customers { get; set; }
}
```

Then you register it as you normally would:

```csharp
services.AddDbContext<StoreDbContext>(opt => ...);
```

## Register

The other option is to let you inherit from whatever base `DbContext` you want and then instead leverage the extension methods:

```csharp
services.AddReadOnlyDbContext<StoreDbContext>(opt => ...);
```

### Discover and register all in assemblies

For convenience you can get all types inheriting from `DbContext` automatically discovered and registered in one call.

```csharp
services.AddReadModelDbContextsFromAssemblies(opt => 
{
    /* Configure any options */
},
[Assembly.GetExecutingAssembly()]);
```

Or if you want it to automatically configure it with the correct database:

```csharp
services.AddReadModelDbContextsWithConnectionStringFromAssemblies(opt => 
{
    /* Configure any options */
},
".. your connection string..",
[Assembly.GetExecutingAssembly()]);
```
