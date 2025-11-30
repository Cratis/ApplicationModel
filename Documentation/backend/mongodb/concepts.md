# Concepts

Cratis Applications provides seamless integration between [Cratis Concepts](../../general/index.md) and MongoDB through automatic serialization support. Concepts are domain-driven design primitives that wrap primitive types with business meaning.

## What are Concepts?

Concepts are types that inherit from `ConceptAs<T>` and provide type-safe wrappers around primitive values:

```csharp
public record UserId(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly UserId NotSet = new(Guid.Empty);
    public static implicit operator UserId(Guid value) => new(value);
    public static UserId New() => new(Guid.NewGuid());
}

public record ProductName(string Value) : ConceptAs<string>(Value)
{
    public static readonly ProductName NotSet = new(string.Empty);
    public static implicit operator ProductName(string value) => new(value);
}
```

## Automatic Serialization

When you call `UseCratisMongoDB()`, all types implementing `ConceptAs<T>` are automatically configured for MongoDB serialization through the `ConceptSerializationProvider`.

### How It Works

The `ConceptSerializer<T>` handles the serialization by:

1. **Detection**: Automatically detects types that implement `ConceptAs<T>`
2. **Unwrapping**: Serializes only the underlying value, not the wrapper
3. **Type Safety**: Ensures type validation during serialization/deserialization
4. **Performance**: Optimized to avoid unnecessary object creation

### Example Usage

```csharp
public class User
{
    public UserId Id { get; set; }
    public ProductName Name { get; set; }
    public EmailAddress Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

// Usage
var user = new User
{
    Id = UserId.New(),
    Name = "John Doe",  // Implicit conversion
    Email = "john@example.com",
    CreatedAt = DateTimeOffset.Now
};

// In MongoDB, this will be stored as:
// {
//   "_id": "550e8400-e29b-41d4-a716-446655440000",
//   "name": "John Doe",
//   "email": "john@example.com", 
//   "createdAt": ISODate("2024-01-15T10:30:00Z")
// }
```

## Supported Underlying Types

The `ConceptSerializer<T>` supports all primitive types that MongoDB can natively handle:

### Numeric Types

- `int`, `uint`, `long`, `ulong`
- `float`, `double`, `decimal`
- `byte`, `sbyte`, `short`, `ushort`

### String and Character Types

- `string`
- `char`

### Date and Time Types

- `DateTime`
- `DateTimeOffset` (uses the custom serializer)
- `DateOnly` (uses the custom serializer)
- `TimeOnly` (uses the custom serializer)

### Other Types

- `bool`
- `Guid`
- Any type that has a registered MongoDB serializer

## Error Handling

The concept serializer includes robust error handling:

### Type Validation

```csharp
// This will throw TypeIsNotAConcept exception
var serializer = new ConceptSerializer<string>(); // Invalid - string is not a concept
```

### Null Safety

```csharp
public record OptionalId(Guid? Value) : ConceptAs<Guid?>(Value)
{
    public static readonly OptionalId NotSet = new(null);
}

// Properly handles null values during serialization
```

## Performance Benefits

### Storage Efficiency

Concepts are serialized as their underlying values, meaning:

- **No wrapper overhead**: Only the business value is stored
- **Native MongoDB types**: Uses optimal BSON types for each primitive
- **Index compatibility**: Underlying values can be indexed normally

### Memory Efficiency

- **Lazy initialization**: Concept instances are created only when needed
- **Value semantics**: Record-based concepts minimize allocation overhead
- **Implicit conversions**: Reduce explicit casting requirements

## Best Practices

### Use Static Members for Common Values

```csharp
public record Status(string Value) : ConceptAs<string>(Value)
{
    public static readonly Status Active = new("Active");
    public static readonly Status Inactive = new("Inactive");
    public static readonly Status Pending = new("Pending");
    
    public static implicit operator Status(string value) => new(value);
}
```

### Implement Validation

```csharp
public record EmailAddress(string Value) : ConceptAs<string>(Value)
{
    public EmailAddress(string value) : this(Validate(value)) { }
    
    static string Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");
            
        if (!email.Contains('@'))
            throw new ArgumentException("Invalid email format");
            
        return email;
    }
    
    public static implicit operator EmailAddress(string value) => new(value);
}
```

### Event Source Integration

For concepts used as Event Source IDs:

```csharp
public record CustomerId(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly CustomerId NotSet = new(Guid.Empty);
    public static implicit operator CustomerId(Guid value) => new(value);
    public static implicit operator EventSourceId(CustomerId id) => new(id.Value.ToString());
    public static CustomerId New() => new(Guid.NewGuid());
}
```

## Collection Examples

Concepts work seamlessly in collections:

```csharp
public class Order
{
    public OrderId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public IEnumerable<ProductId> ProductIds { get; set; }
    public Dictionary<ProductId, Quantity> Products { get; set; }
}

// All concept types in collections are automatically handled
```

## Query Support

Concepts can be used directly in MongoDB queries:

```csharp
var customerId = CustomerId.New();
var orders = await collection
    .Find(o => o.CustomerId == customerId)
    .ToListAsync();
    
// The concept is automatically converted to its underlying value for the query
```

## Next Steps

- Learn about [Class Mapping](class-mapping.md) for complex type configurations
- Explore [Convention Packs](convention-packs.md) for customizing serialization behavior
- Read about [Naming Policies](naming-policies.md) for consistent field naming
