# Base Db Context

The `BaseDbContext` provides a pre-configured Entity Framework Core context that automatically applies common conventions and converters. This eliminates the need for manual configuration while ensuring consistent behavior across your application.

## Features

The `BaseDbContext` automatically configures the following features:

### JSON Conversion

Automatically applies [JSON conversion](./json.md) for properties marked with the `[Json]` attribute, allowing complex objects to be stored as JSON in the database with cross-provider compatibility.

### ConceptAs Conversion

Automatically applies [ConceptAs conversion](./concept-as-conversion.md) for all properties that implement `ConceptAs<T>`, ensuring domain concepts are properly stored and retrieved while maintaining type safety.

### GUID Conversion

Automatically applies [GUID conversion](./guid-conversion.md) for all `Guid` properties, optimizing storage format and performance for each database provider.

## Usage

All you need to do is inherit from `BaseDbContext` and register it as you'd do with any other `DbContext`.

Take the following `DbContext`:

```csharp
using Cratis.Applications.EntityFrameworkCore;

public class StoreDbContext : BaseDbContext
{
    public DbSet<Customer> Customers { get; set; }
}
```

Then you register it as you normally would:

```csharp
services.AddDbContext<StoreDbContext>(opt => ...);
```
