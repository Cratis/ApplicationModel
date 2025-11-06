# Automatic Database Hookup

Out of the box we support the following databases:

- Sqlite
- PostgreSQL
- Microsoft SQL Server

There are different extension methods for adding `DbContext` types and also for resolving the correct database provider based on connection string.

```csharp
services.AddDbContext<MyDbContext>(opt => opt.UseDatabaseFromConnectionString(".. your connection string.."));
```

> Note: From the connection string it will do the correct `.UseSqlite()`, `.UseNpgsql()` or `.UseSqlServer()` call on the builder.

There is also a short-hand version of this:

```csharp
services.AddDbContextWithConnectionString<MyDbContext>(".. your connection string..", opt => /* do whatever configuration you want */);
```

## Read Only DbContexts

For any **read-only** `DbContext` there is also an extension method:

```csharp
services.AddReadOnlyDbContextWithConnectionString<MyDbContext>(".. your connection string..", opt => /* do whatever configuration you want */);
```

## Automatic Registration from Assemblies

The framework provides methods to automatically discover and register all `ReadOnlyDbContext` types from specified assemblies:

```csharp
// Register all ReadOnlyDbContext types from assemblies with a common options action
services.AddReadModelDbContextsFromAssemblies(opt => opt.UseDatabaseFromConnectionString(connectionString), assembly1, assembly2);

// Register all ReadOnlyDbContext types from assemblies with a connection string
services.AddReadModelDbContextsWithConnectionStringFromAssemblies(connectionString, opt => /* additional options */, assembly1, assembly2);
```

### Registration Filtering Rules

When using automatic registration, the framework applies the following filtering rules:

1. **Public Classes Only**: Only `public` DbContext classes will be automatically registered. Internal, private, or protected classes are ignored.

2. **Assembly Membership**: Only DbContext classes that belong to the specified assemblies will be considered for registration.

3. **Attribute-Based Exclusion**: Classes marked with the `IgnoreAutoRegistrationAttribute` will be excluded from automatic registration.

### Excluding DbContexts from Automatic Registration

If you have a DbContext that should not be automatically registered (for example, if it requires special configuration or should be registered manually), you can exclude it using the `IgnoreAutoRegistrationAttribute`:

```csharp
using Cratis.Applications;

[IgnoreAutoRegistration]
public class SpecialDbContext : ReadOnlyDbContext
{
    // This DbContext will be ignored during automatic registration
    // and must be registered manually if needed
}
```

This is useful for scenarios where:

- The DbContext requires special configuration
- You want to register it with different lifetime scopes
- It's used only in specific conditions
- You want to register it manually with custom options
