# Class Mapping

Class mapping in Cratis Applications provides a powerful way to customize how your .NET types are serialized to and from MongoDB BSON documents. The framework includes automatic discovery and registration of custom mappings.

## Overview

Class mapping allows you to:

- **Customize field names**: Override property names in the database
- **Configure serialization**: Specify custom serializers for properties
- **Set up inheritance**: Configure polymorphic type hierarchies
- **Control indexes**: Define which properties should be indexed
- **Ignore properties**: Exclude certain properties from serialization

## IBsonClassMapFor Interface

To create a custom class map, implement the `IBsonClassMapFor<T>` interface:

```csharp
public interface IBsonClassMapFor<T>
{
    void Configure(BsonClassMap<T> classMap);
}
```

## Basic Class Map Example

```csharp
public class User
{
    public UserId Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public string PasswordHash { get; set; }  // We don't want to serialize this
}

public class UserClassMap : IBsonClassMapFor<User>
{
    public void Configure(BsonClassMap<User> classMap)
    {
        classMap.AutoMap();
        
        // Set the ID field
        classMap.SetIdMember(classMap.GetMemberMap(u => u.Id));
        
        // Custom field name
        classMap.GetMemberMap(u => u.UserName)
               .SetElementName("username");
        
        // Ignore sensitive data
        classMap.UnmapMember(u => u.PasswordHash);
        
        // Custom serializer for dates
        classMap.GetMemberMap(u => u.CreatedAt)
               .SetSerializer(new DateTimeOffsetSupportingBsonDateTimeSerializer());
    }
}
```

## Automatic Discovery

The framework automatically discovers and registers all implementations of `IBsonClassMapFor<T>`:

```csharp
// This happens automatically during UseCratisMongoDB() setup
var types = Types.Instance;
var classMaps = types.FindMultiple(typeof(IBsonClassMapFor<>));

foreach (var classMapType in classMaps)
{
    // Automatic registration
    RegisterClassMap(classMapType);
}
```

## Registration Process

Class maps are registered during MongoDB initialization:

1. **Discovery**: All `IBsonClassMapFor<T>` implementations are found
2. **Instantiation**: Each class map provider is created
3. **Configuration**: The `Configure` method is called with a `BsonClassMap<T>`
4. **Convention Application**: Configured conventions are applied to the class map
5. **Registration**: The class map is registered with MongoDB's `BsonClassMap.RegisterClassMap<T>()`

## Advanced Mapping Examples

### Inheritance Mapping

```csharp
public abstract class Document
{
    public DocumentId Id { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TextDocument : Document
{
    public string Content { get; set; }
}

public class ImageDocument : Document
{
    public byte[] ImageData { get; set; }
    public string MimeType { get; set; }
}

public class DocumentClassMap : IBsonClassMapFor<Document>
{
    public void Configure(BsonClassMap<Document> classMap)
    {
        classMap.AutoMap();
        classMap.SetIdMember(classMap.GetMemberMap(d => d.Id));
        
        // Configure inheritance
        classMap.SetIsRootClass(true);
        classMap.AddKnownType(typeof(TextDocument));
        classMap.AddKnownType(typeof(ImageDocument));
    }
}

public class TextDocumentClassMap : IBsonClassMapFor<TextDocument>
{
    public void Configure(BsonClassMap<TextDocument> classMap)
    {
        classMap.AutoMap();
        classMap.SetDiscriminator("text");
    }
}

public class ImageDocumentClassMap : IBsonClassMapFor<ImageDocument>
{
    public void Configure(BsonClassMap<ImageDocument> classMap)
    {
        classMap.AutoMap();
        classMap.SetDiscriminator("image");
        
        // Custom handling for binary data
        classMap.GetMemberMap(i => i.ImageData)
               .SetSerializer(new ByteArraySerializer());
    }
}
```

### Complex Property Mapping

```csharp
public class OrderItem
{
    public ProductId ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;  // Computed property
}

public class Order
{
    public OrderId Id { get; set; }
    public CustomerId CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = [];
    public Address ShippingAddress { get; set; }
    public decimal Total => Items.Sum(i => i.Total);  // Computed property
}

public class OrderClassMap : IBsonClassMapFor<Order>
{
    public void Configure(BsonClassMap<Order> classMap)
    {
        classMap.AutoMap();
        classMap.SetIdMember(classMap.GetMemberMap(o => o.Id));
        
        // Don't serialize computed properties
        classMap.UnmapMember(o => o.Total);
        
        // Custom collection handling
        classMap.GetMemberMap(o => o.Items)
               .SetElementName("orderItems")
               .SetIgnoreIfNull(true);
    }
}

public class OrderItemClassMap : IBsonClassMapFor<OrderItem>
{
    public void Configure(BsonClassMap<OrderItem> classMap)
    {
        classMap.AutoMap();
        
        // Don't serialize computed properties
        classMap.UnmapMember(i => i.Total);
        
        // Custom decimal handling
        classMap.GetMemberMap(i => i.UnitPrice)
               .SetSerializer(new DecimalSerializer(BsonType.Decimal128));
    }
}
```

### Custom Serializers in Class Maps

```csharp
public class EventRecord
{
    public EventId Id { get; set; }
    public EventType Type { get; set; }
    public object Data { get; set; }  // Polymorphic data
    public Dictionary<string, object> Metadata { get; set; }
    public DateTimeOffset Timestamp { get; set; }
}

public class EventRecordClassMap : IBsonClassMapFor<EventRecord>
{
    public void Configure(BsonClassMap<EventRecord> classMap)
    {
        classMap.AutoMap();
        classMap.SetIdMember(classMap.GetMemberMap(e => e.Id));
        
        // Custom polymorphic serialization
        classMap.GetMemberMap(e => e.Data)
               .SetSerializer(new ObjectSerializer(
                   type => type == typeof(object),
                   ObjectSerializationOptions.Default));
        
        // Custom dictionary serialization
        classMap.GetMemberMap(e => e.Metadata)
               .SetSerializer(new DictionaryInterfaceImplementerSerializer<Dictionary<string, object>>());
        
        // Use custom DateTime serializer
        classMap.GetMemberMap(e => e.Timestamp)
               .SetSerializer(new DateTimeOffsetSupportingBsonDateTimeSerializer());
    }
}
```

## Convention Integration

Class maps work seamlessly with the convention system:

### Applying Conventions

After configuration, conventions are automatically applied:

```csharp
public void Configure(BsonClassMap<User> classMap)
{
    classMap.AutoMap();
    // Your custom configuration...
}

// Conventions are applied automatically after Configure() returns
// This includes naming policies, ignore conventions, etc.
```

### Extension Method

The framework provides an extension method to manually apply conventions:

```csharp
public void Configure(BsonClassMap<User> classMap)
{
    classMap.AutoMap();
    
    // Manual configuration
    classMap.GetMemberMap(u => u.UserName).SetElementName("custom_name");
    
    // Apply conventions manually if needed
    classMap.ApplyConventions();
}
```

## Error Handling

### Duplicate Registration Prevention

Class maps are automatically protected against duplicate registration:

```csharp
// This check happens automatically
if (BsonClassMap.IsClassMapRegistered(typeof(T)))
{
    return; // Skip if already registered
}
```

### Validation

The framework validates class map configurations:

```csharp
public class InvalidClassMap : IBsonClassMapFor<User>
{
    public void Configure(BsonClassMap<User> classMap)
    {
        // This would throw an exception during registration
        classMap.SetIdMember(null);
    }
}
```

## Best Practices

### Use AutoMap First

Always call `AutoMap()` first, then customize:

```csharp
public void Configure(BsonClassMap<User> classMap)
{
    classMap.AutoMap();  // Set up defaults first
    
    // Then customize as needed
    classMap.SetIdMember(classMap.GetMemberMap(u => u.Id));
    classMap.UnmapMember(u => u.PasswordHash);
}
```

### Organize by Domain

Group related class maps together:

```csharp
namespace MyApp.MongoDB.UserMappings
{
    public class UserClassMap : IBsonClassMapFor<User> { }
    public class UserProfileClassMap : IBsonClassMapFor<UserProfile> { }
    public class UserSettingsClassMap : IBsonClassMapFor<UserSettings> { }
}
```

### Handle Computed Properties

Don't serialize computed properties:

```csharp
public void Configure(BsonClassMap<Order> classMap)
{
    classMap.AutoMap();
    
    // These are computed at runtime
    classMap.UnmapMember(o => o.Total);
    classMap.UnmapMember(o => o.ItemCount);
    classMap.UnmapMember(o => o.IsComplete);
}
```

### Use Appropriate Serializers

Choose serializers that match your data requirements:

```csharp
// For high-precision decimals
classMap.GetMemberMap(p => p.Price)
       .SetSerializer(new DecimalSerializer(BsonType.Decimal128));

// For large integers
classMap.GetMemberMap(p => p.LargeNumber)
       .SetSerializer(new Int64Serializer(BsonType.Int64));

// For enum values
classMap.GetMemberMap(p => p.Status)
       .SetSerializer(new EnumSerializer<OrderStatus>(BsonType.String));
```

## Testing Class Maps

You can test your class maps to ensure they work correctly:

```csharp
[Test]
public void should_serialize_user_correctly()
{
    // Arrange
    var user = new User
    {
        Id = UserId.New(),
        UserName = "testuser",
        Email = "test@example.com"
    };
    
    // Act
    var document = user.ToBsonDocument();
    
    // Assert
    document.Contains("_id").ShouldBeTrue();
    document.Contains("username").ShouldBeTrue();  // Custom element name
    document.Contains("PasswordHash").ShouldBeFalse();  // Should be ignored
}
```

## Next Steps

- Learn about [Convention Packs](convention-packs.md) for system-wide customizations
- Explore [Concepts](concepts.md) for domain-driven design patterns
- Understand [Naming Policies](naming-policies.md) for consistent field naming
