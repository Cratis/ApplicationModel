# Entity Mapping

The Application Model provides a clean, organized way to configure your Entity Framework Core entities through the `IEntityMapFor<T>` interface. This approach separates entity configuration from your DbContext, making your code more maintainable and following the Single Responsibility Principle.

## Overview

Entity mapping in the Application Model allows you to define how your entities are configured for Entity Framework Core in dedicated classes. These entity maps are automatically discovered and applied when your DbContext is created, eliminating the need to override `OnModelCreating` in every DbContext.

## The IEntityMapFor&lt;T&gt; Interface

The `IEntityMapFor<T>` interface defines a contract for configuring a specific entity type:

```csharp
public interface IEntityMapFor<T>
    where T : class
{
    void Configure(EntityTypeBuilder<T> builder);
}
```

This interface provides access to the `EntityTypeBuilder<T>` which gives you the full power of Entity Framework Core's Fluent API for configuring your entities.

## Creating Entity Maps

To create an entity map, implement the `IEntityMapFor<T>` interface for your entity type:

```csharp
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cratis.Applications.EntityFrameworkCore;

public class UserMap : IEntityMapFor<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configure the primary key
        builder.HasKey(u => u.Id);
        
        // Configure properties
        builder.Property(u => u.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasMaxLength(255)
            .IsRequired();
            
        // Configure indexes
        builder.HasIndex(u => u.Email)
            .IsUnique();
            
        // Configure relationships
        builder.HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId);
    }
}
```

## Automatic Discovery and Registration

Entity maps are automatically discovered and registered when your application starts. The Application Model:

1. **Discovers all implementations** of `IEntityMapFor<T>` in your application
2. **Registers them with dependency injection** so they can have dependencies injected
3. **Applies them automatically** during `OnModelCreating` in your DbContext

This happens automatically when you inherit from `BaseDbContext`:

```csharp
public class MyDbContext : BaseDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    // No need to override OnModelCreating - entity maps are applied automatically!
}
```

## Dependency Injection Support

Entity maps support dependency injection, allowing you to inject services that might be needed for configuration:

```csharp
public class UserMap : IEntityMapFor<User>
{
    private readonly IConfiguration _configuration;
    
    public UserMap(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        // Use injected configuration
        var maxNameLength = _configuration.GetValue<int>("User:MaxNameLength", 100);
        builder.Property(u => u.Name)
            .HasMaxLength(maxNameLength)
            .IsRequired();
    }
}
```

## Advanced Configuration Examples

### Complex Property Configuration

```csharp
public class OrderMap : IEntityMapFor<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        // Configure value objects
        builder.OwnsOne(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Street).HasMaxLength(200);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.PostalCode).HasMaxLength(20);
        });
        
        // Configure JSON columns
        builder.Property(o => o.Metadata)
            .HasJsonConversion();
            
        // Configure computed columns
        builder.Property(o => o.TotalAmount)
            .HasComputedColumnSql("[Quantity] * [UnitPrice]");
    }
}
```

### Multiple Entity Configurations

```csharp
public class ProductMap : IEntityMapFor<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();
            
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
            
        // Table configuration
        builder.ToTable("Products", "Catalog");
        
        // Soft delete configuration
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

public class CategoryMap : IEntityMapFor<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        // Self-referencing relationship
        builder.HasMany(c => c.Children)
            .WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId);
    }
}
```

## Best Practices

### Organization

- **One entity map per entity**: Keep each entity map focused on a single entity type
- **Descriptive naming**: Use clear names like `UserMap`, `OrderMap`, etc.
- **Logical grouping**: Place entity maps in a dedicated folder (e.g., `Mapping/` or `Configuration/`)

### Configuration Guidelines

- **Configure all aspects**: Include keys, properties, relationships, and constraints
- **Be explicit**: Don't rely on conventions for critical configurations
- **Use meaningful names**: Configure table and column names explicitly when needed
- **Document complex logic**: Add comments for complex mapping logic

### Performance Considerations

- **Index configuration**: Configure indexes for frequently queried properties
- **Query filters**: Use global query filters for soft delete patterns
- **Relationship loading**: Configure loading behavior for relationships

## Integration with BaseDbContext

When you inherit from `BaseDbContext`, entity maps are automatically applied during model creation. The base class:

1. Discovers all `DbSet<T>` properties on your DbContext
2. Looks for corresponding `IEntityMapFor<T>` implementations
3. Applies the configurations during `OnModelCreating`

This means you don't need to manually register or apply entity maps - they work automatically once you implement the interface.

## Migration Support

Entity maps work seamlessly with Entity Framework Core migrations. When you add or modify entity maps:

1. Run `dotnet ef migrations add <MigrationName>` to create a migration
2. The migration will include all changes from your entity maps
3. Run `dotnet ef database update` to apply the changes

The automatic discovery and application of entity maps ensures that all your configurations are included in migrations without additional setup.
