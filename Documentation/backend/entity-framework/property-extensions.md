# Property Extensions

The Cratis Application Model provides extension methods for Entity Framework Core's `PropertyBuilder` to help configure properties with cross-database compatibility in mind.

## AsGuid()

The `AsGuid()` extension method configures a GUID property to use string storage with a standardized format, ensuring compatibility across different database providers.

> **Note**: You should use this in conjunction with the migration-based column type configuration, see [Common Column Types](./common-column-types.md) which provides the `GuidColumn()` method for creating GUID columns in migrations.

### Why AsGuid()?

Different database providers handle GUIDs differently:

- **PostgreSQL**: Native UUID type
- **SQL Server**: UNIQUEIDENTIFIER type  
- **SQLite**: No native GUID support

The `AsGuid()` guarantees the storage of GUID by normalizing them to strings (e.g., `550e8400-e29b-41d4-a716-446655440000`), providing consistent behavior across all database providers.

### Usage

```csharp
public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id)
                .AsGuid(); // Configures the GUID to be stored as string
        });
    }
}
```

### What it does

The `AsGuid()` method:

1. Converts `Guid` values to string using the `ToString("D")` format for storage
2. Parses strings back to `Guid` values when reading from the database
3. Ensures consistent GUID representation across all supported database providers

### When to use

Use `AsGuid()` when:

- You need consistent GUID behavior across multiple database providers
- Your application might be deployed with different database backends
- You want to avoid database-specific GUID formatting issues
