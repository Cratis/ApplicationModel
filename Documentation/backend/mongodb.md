# MongoDB

To use the MongoDB setup, you can easily hook it into your application by leveraging the configuration extension methods
provided for `WebApplicationBuilder` or `HostBuilder`.

With a builder:

```csharp
var builder = WebApplication.CreateBuilder(args)
    .UseCratisApplicationModel();
```

You can simply do the following:

```csharp
builder.UseCratisMongoDB();
```

This will configure your application with a set of MongoDB defaults and register services in the `ServiceCollection`.
The defaults include common serializers for types such as:

- `DateTimeOffset`
- `DateOnly`
- `TimeOnly`
- `System.Type`
- Cratis Concepts (Types implementing `ConceptAs<>`)

It also configured `Guid` to be as expected for a .NET developer.

## Naming policies

Collection names and its members are named based on a naming policy (`INamingPolicy`).
The default naming policy does not alter the input, giving you the names as defined in your types.
You can easily create your own naming policy or use one of the built in.

To configure one you already have, you can do the following:

```csharp
builder.UseCratisMongoDB(configureMongoDB: builder => builder.WithNamingPolicy<MyCustomNamingPolicy>());
```

If your naming policy requires parameters to be initialized, you can use the overload that takes an instance:

```csharp
builder.UseCratisMongoDB(configureMongoDB: builder => builder.WithNamingPolicy(new MyCustomNamingPolicy(...)));
```

### Camel Case

A common naming policy is to have everything camel cased, there is a built in naming policy for this and
configuration method for it:

```csharp
builder.UseCratisMongoDB(configureMongoDB: builder => builder.WithCamelCaseNamingPolicy());
```
