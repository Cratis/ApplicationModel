# Json

Having data in Json can be very convenient. Cratis Application Model comes with a cross database
approach for the entity model and for migrations.

Its based on metadata on the model in the form of a `[Json]` attribute that you apply and
it will then configure the model from this.

The converters configured will serialize the content to Json and deserialize when fetched from the
database.

## Model

Your model would specify what properties should be treated as Json in the database by
applying the `[Json]` attribute like below:

```csharp
using Cratis.Applications.EntityFrameworkCore.Json;

public class Customer
{
    public Guid Id { get; set; }

    [Json]
    public IEnumerable<Address> Addresses { get; set; }
}
```

## Model Builder

To get it all hooked up, you need to call `.ApplyJsonConversion()` extension method on the
`ModelBuilder` during model creation of your `DbContext`:

```csharp
using Cratis.Applications.EntityFrameworkCore.Json;

public class StoreDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyJsonConversion(Database.ProviderName);
        base.OnModelCreating(modelBuilder);
    }
}
```

> Note: This is automatically hooked up for you when leveraging the [`BaseDbContext`](./base-db-context.md).
