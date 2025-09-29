# Model Bound Queries

For a more lightweight approach, queries can be their own performers.
This is achieved by adorning your read model record with the `[ReadModel]` attribute
and implementing static methods for query operations directly on the record type.

```csharp
[ReadModel]  // The ReadModel attribute is needed
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAllAccounts(IMongoCollection<DebitAccount> collection)
    {
        return collection.Find(_ => true).ToList();
    }
}
```

> **Note**: If you're using the Cratis ApplicationModel [proxy generator](../proxy-generation.md), the method name
> will become the query name for the generated TypeScript file and class.

## Static Method Requirements

The static methods on your read model record:

- Must be `public` and `static`
- Can have any name that describes the query operation
- Can take dependencies as parameters (injected via dependency injection)
- Can be async
- Should return the record itself, a generic enumerable/list/collection of the record type
- Can be observable by returning an `ISubject<>` of the record type (do not make it async by using Task<>)

## Dependencies

Your query method can take dependencies from the service collection as parameters.
The application model will automatically resolve and inject these dependencies:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetActiveAccounts(IMongoCollection<DebitAccount> collection) =>
        collection.Find(account => account.IsActive);
}
```

## Async Support

Your static methods can also be asynchronous:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetAllAccountsAsync(IMongoCollection<DebitAccount> collection)
    {
        var result = await collection.FindAsync(_ => true);
        return result.ToList();
    }
}
```

## Multiple Query Methods

A single read model record can have multiple query methods:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAllAccounts(IMongoCollection<DebitAccount> collection) =>
        collection.Find(_ => true).ToList();

    public static DebitAccount GetAccountById(AccountId id, IMongoCollection<DebitAccount> collection) =>
        collection.Find(a => a.Id == id).FirstOrDefault();

    public static IEnumerable<DebitAccount> GetAccountsByOwner(CustomerId ownerId, IMongoCollection<DebitAccount> collection) =>
        collection.Find(a => a.Owner == ownerId).ToList();
}
```

## Return Types

Model bound queries support various return types:

### Collections

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAccounts(IMongoCollection<DebitAccount> collection) 
        => collection.Find(_ => true);
}
```

### Single Objects

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static DebitAccount GetFirstAccount(IMongoCollection<DebitAccount> collection)
        => collection.Find(_ => true).FirstOrDefault();
}
```

### Complex Return Types

```csharp
public record AccountSummary(int TotalAccounts, decimal TotalBalance);

[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static AccountSummary GetSummary(IMongoCollection<DebitAccount> collection)
    {
        var accounts = collection.Find(_ => true).ToList();
        return new AccountSummary(accounts.Count, accounts.Sum(a => a.Balance));
    }
}
```

### Observable Queries (ISubject<>)

For real-time queries that can push updates to subscribers:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static ISubject<IEnumerable<DebitAccount>> GetAccountsObservable(
        IMongoCollection<DebitAccount> collection) => 
            collection.Observe(); // Leveraging MongoDB Extension methods
}
```

## Query Arguments

Model-bound queries can accept arguments as method parameters. Arguments can be route parameters, query string parameters, or complex objects.

### Method Parameters

Arguments are passed as method parameters and are automatically bound from the HTTP request:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static DebitAccount GetAccountById(
        AccountId id, 
        IMongoCollection<DebitAccount> collection)
    {
        return collection.Find(a => a.Id == id).FirstOrDefault();
    }

    public static IEnumerable<DebitAccount> SearchAccounts(
        string nameFilter,
        decimal? minBalance,
        IMongoCollection<DebitAccount> collection,
        ILogger<Accounts> logger)
    {
        logger.LogInformation("Searching accounts with filter: {Filter}", nameFilter);
        
        var filterBuilder = Builders<DebitAccount>.Filter;
        var filters = new List<FilterDefinition<DebitAccount>>();

        if (!string.IsNullOrEmpty(nameFilter))
        {
            filters.Add(filterBuilder.Regex(a => a.Name, new BsonRegularExpression(nameFilter, "i")));
        }

        if (minBalance.HasValue)
        {
            filters.Add(filterBuilder.Gte(a => a.Balance, minBalance.Value));
        }

        var combinedFilter = filters.Any() 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        return collection.Find(combinedFilter).ToList();
    }
}
```

### Argument Types

Model-bound queries support various argument types:

#### Primitive Types

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAccountsByBalance(
        decimal balance,
        bool exactMatch,
        IMongoCollection<DebitAccount> collection)
    {
        return exactMatch 
            ? collection.Find(a => a.Balance == balance).ToList()
            : collection.Find(a => a.Balance >= balance).ToList();
    }
}
```

#### Concept Types

Using concept types (value objects) for stronger typing:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAccountsByOwnerConcept(
        CustomerId ownerId,
        IMongoCollection<DebitAccount> collection)
    {
        return collection.Find(a => a.Owner == ownerId).ToList();
    }
}
```

#### Enums

```csharp
public enum AccountStatus { Active, Inactive, Suspended }

[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAccountsByStatus(
        AccountStatus status,
        IMongoCollection<DebitAccount> collection)
    {
        // Implement status filtering logic
        return status switch
        {
            AccountStatus.Active => collection.Find(a => a.Balance > 0).ToList(),
            AccountStatus.Inactive => collection.Find(a => a.Balance == 0).ToList(),
            AccountStatus.Suspended => collection.Find(a => a.Balance < 0).ToList(),
            _ => collection.Find(_ => false).ToList()
        };
    }
}
```

### Nullable Arguments

Optional arguments should be nullable:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> FlexibleSearch(
        string? name,
        CustomerId? ownerId,
        decimal? minBalance,
        decimal? maxBalance,
        IMongoCollection<DebitAccount> collection)
    {
        var filterBuilder = Builders<DebitAccount>.Filter;
        var filters = new List<FilterDefinition<DebitAccount>>();

        if (!string.IsNullOrEmpty(name))
            filters.Add(filterBuilder.Regex(a => a.Name, new BsonRegularExpression(name, "i")));

        if (ownerId.HasValue)
            filters.Add(filterBuilder.Eq(a => a.Owner, ownerId.Value));

        if (minBalance.HasValue)
            filters.Add(filterBuilder.Gte(a => a.Balance, minBalance.Value));

        if (maxBalance.HasValue)
            filters.Add(filterBuilder.Lte(a => a.Balance, maxBalance.Value));

        var combinedFilter = filters.Any() 
            ? filterBuilder.And(filters) 
            : filterBuilder.Empty;

        return collection.Find(combinedFilter).ToList();
    }
}
```

### Default Values

Provide sensible default values for optional parameters:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetPagedAccounts(
        int page = 0,
        int pageSize = 50,
        string sortBy = "name",
        bool ascending = true,
        IMongoCollection<DebitAccount> collection)
    {
        var query = collection.Find(_ => true);
        
        // Apply sorting
        query = ascending 
            ? query.SortBy(sortBy) 
            : query.SortByDescending(sortBy);
        
        // Apply paging
        return query.Skip(page * pageSize).Limit(pageSize).ToList();
    }
}
```

> **Note**: The [proxy generator](../proxy-generation.md) automatically creates TypeScript types for your query arguments,
> making them strongly typed on the frontend as well.
