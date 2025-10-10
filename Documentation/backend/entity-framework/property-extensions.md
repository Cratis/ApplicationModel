# Property Extensions

The Cratis Application Model provides extension methods for Entity Framework Core's `PropertyBuilder` to help configure properties with cross-database compatibility in mind.

## AsGuid()

The `AsGuid()` extension method configures a GUID property for optimal storage based on the database provider, ensuring compatibility across different database providers while using native types when available.

> **Note**: You should use this in conjunction with the migration-based column type configuration, see [Common Column Types](./common-column-types.md) which provides the `GuidColumn()` method for creating GUID columns in migrations.

### Why AsGuid()?

Different database providers handle GUIDs differently:

- **PostgreSQL**: Native UUID type with optimal performance
- **SQL Server**: UNIQUEIDENTIFIER type with optimal performance  
- **SQLite**: No native GUID support, requires string conversion

The `AsGuid()` method uses the provided database information to apply the appropriate configuration:

- For **SQLite**: Converts GUIDs to strings using the "D" format (e.g., `550e8400-e29b-41d4-a716-446655440000`)
- For **PostgreSQL and SQL Server**: Uses native GUID types for optimal performance

This provides the best of both worlds: optimal performance on databases with native GUID support and compatibility for SQLite.

### Usage

#### Recommended Usage

```csharp
public class MyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id)
                .AsGuid(Database); // Pass the database for proper provider detection
        });
    }
}
```

> **Important**: The `database` parameter is required to ensure optimal configuration based on the actual database provider being used.

### What it does

The `AsGuid()` method:

1. **Uses the provided database parameter** to determine the database provider (PostgreSQL, SQL Server, or SQLite)
2. **For SQLite**: Converts `Guid` values to string using the `ToString("D")` format for storage and parses them back when reading
3. **For PostgreSQL and SQL Server**: Uses native GUID types without conversion for optimal performance
4. **Ensures optimal behavior** for each supported database provider

### When to use

Use `AsGuid()` when:

- You need consistent GUID behavior across multiple database providers
- Your application might be deployed with different database backends
- You want optimal performance on databases with native GUID support
- You want automatic SQLite compatibility with proper provider detection

### Why the Database parameter is required

The `database` parameter is required because:

- **Ensures optimal performance**: Native GUID types are used for PostgreSQL and SQL Server, string conversion only for SQLite
- **Explicit provider detection**: Eliminates guesswork and ensures the correct configuration is applied
- **Better debugging**: Makes it clear which database provider configuration is being used
- **Prevents misconfiguration**: Avoids scenarios where incorrect assumptions about the database provider could lead to suboptimal storage
