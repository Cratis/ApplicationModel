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