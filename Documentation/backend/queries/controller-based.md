# Controller Based Queries

You can represent queries as regular ASP.NET Core Controller actions with HTTP GET methods.

```csharp
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance);

[Route("api/accounts")]
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet]
    public IEnumerable<DebitAccount> AllAccounts() => _collection.Find(_ => true).ToList();
}
```

> Note: This particular model represents its values as concepts - a value type encapsulation that
> makes us not use primitives - thus creating clearer APIs and models.

> **Note**: If you're using the Cratis ApplicationModel [proxy generator](../proxy-generation.md), the method name
> will become the query name for the generated TypeScript file and class.

## Async Support

For asynchronous operations, you can return `Task<T>`:

```csharp
[HttpGet]
public async Task<IEnumerable<DebitAccount>> AllAccountsAsync()
{
    var result = await _collection.FindAsync(_ => true);
    return result.ToList();
}
```

## Different Return Types

Queries can return various data types:

### Single Object

```csharp
[HttpGet("{id}")]
public DebitAccount GetAccount(AccountId id)
{
    return _collection.Find(a => a.Id == id).FirstOrDefault();
}
```

### Collections

```csharp
[HttpGet]
public IEnumerable<DebitAccount> GetAccounts()
{
    return _collection.Find(_ => true).ToList();
}

// Or List<T>
[HttpGet]
public List<DebitAccount> GetAccountsList()
{
    return _collection.Find(_ => true).ToList();
}

// Or arrays
[HttpGet]
public DebitAccount[] GetAccountsArray()
{
    return _collection.Find(_ => true).ToArray();
}
```

### Query Results

For more control over the response metadata, you can return `QueryResult<T>`:

```csharp
[HttpGet]
public QueryResult<IEnumerable<DebitAccount>> GetAccountsWithMetadata()
{
    var accounts = _collection.Find(_ => true).ToList();
    return new QueryResult<IEnumerable<DebitAccount>>
    {
        Data = accounts,
        // Additional metadata will be populated automatically
    };
}
```

## Dependency Injection

Controllers support dependency injection in their constructors:

```csharp
public class Accounts : Controller
{
    readonly IAccountService _accountService;
    readonly ILogger<Accounts> _logger;

    public Accounts(IAccountService accountService, ILogger<Accounts> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<DebitAccount> AllAccounts()
    {
        _logger.LogInformation("Retrieving all accounts");
        return _accountService.GetAllAccounts();
    }
}
```

## Route Templates

You can use standard ASP.NET Core routing:

```csharp
[Route("api/accounts")]
public class Accounts : Controller
{
    [HttpGet]
    public IEnumerable<DebitAccount> GetAll() { ... }

    [HttpGet("{id}")]
    public DebitAccount GetById(string id) { ... }

    [HttpGet("by-owner/{ownerId}")]
    public IEnumerable<DebitAccount> GetByOwner(string ownerId) { ... }

    [HttpGet("search")]
    public IEnumerable<DebitAccount> Search([FromQuery] string term) { ... }
}
```

## Query Arguments

Queries can accept arguments to filter, customize, or parameterize the data they return.
Arguments can come from route parameters, query strings, or request bodies.

> **💡 Proxy Generation**: The [proxy generator](../proxy-generation.md) automatically analyzes your query arguments and creates strongly-typed TypeScript interfaces, ensuring type safety between your backend and frontend.

### Route Parameters

Route parameters are embedded in the URL path and are typically used for primary identifiers:

```csharp
[Route("api/accounts")]
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet("{id}")]
    public DebitAccount GetAccountById(AccountId id)
    {
        return _collection.Find(a => a.Id == id).FirstOrDefault();
    }

    [HttpGet("owner/{ownerId}")]
    public IEnumerable<DebitAccount> GetAccountsByOwner(CustomerId ownerId)
    {
        return _collection.Find(a => a.Owner == ownerId).ToList();
    }

    [HttpGet("{id}/balance")]
    public decimal GetAccountBalance(AccountId id)
    {
        var account = _collection.Find(a => a.Id == id).FirstOrDefault();
        return account?.Balance ?? 0;
    }
}
```

### Query String Parameters

Query string parameters are appended to the URL after a `?` and are typically used for optional filters or configuration:

```csharp
[HttpGet]
public IEnumerable<DebitAccount> GetAccounts([FromQuery] string? nameFilter = null)
{
    var filter = Builders<DebitAccount>.Filter.Empty;
    
    if (!string.IsNullOrEmpty(nameFilter))
    {
        filter = Builders<DebitAccount>.Filter.Regex(
            account => account.Name, 
            new BsonRegularExpression(nameFilter, "i"));
    }
    
    return _collection.Find(filter).ToList();
}

[HttpGet("search")]
public async Task<IEnumerable<DebitAccount>> SearchAccounts(
    [FromQuery] string? name = null,
    [FromQuery] decimal? minBalance = null,
    [FromQuery] decimal? maxBalance = null,
    [FromQuery] bool includeInactive = false)
{
    var filterBuilder = Builders<DebitAccount>.Filter;
    var filters = new List<FilterDefinition<DebitAccount>>();

    if (!string.IsNullOrEmpty(name))
    {
        filters.Add(filterBuilder.Regex(a => a.Name, new BsonRegularExpression(name, "i")));
    }

    if (minBalance.HasValue)
    {
        filters.Add(filterBuilder.Gte(a => a.Balance, minBalance.Value));
    }

    if (maxBalance.HasValue)
    {
        filters.Add(filterBuilder.Lte(a => a.Balance, maxBalance.Value));
    }

    if (!includeInactive)
    {
        filters.Add(filterBuilder.Gt(a => a.Balance, 0));
    }

    var combinedFilter = filters.Any() 
        ? filterBuilder.And(filters) 
        : filterBuilder.Empty;

    var result = await _collection.FindAsync(combinedFilter);
    return result.ToList();
}
```

### Complex Query Objects

For complex queries with multiple parameters, you can create dedicated query objects:

```csharp
public record AccountSearchQuery(
    string? Name,
    decimal? MinBalance,
    decimal? MaxBalance,
    bool IncludeInactive,
    string? OwnerName);

[HttpGet("advanced-search")]
public IEnumerable<DebitAccount> SearchAccountsAdvanced([FromQuery] AccountSearchQuery query)
{
    var filterBuilder = Builders<DebitAccount>.Filter;
    var filters = new List<FilterDefinition<DebitAccount>>();

    if (!string.IsNullOrEmpty(query.Name))
    {
        filters.Add(filterBuilder.Regex(a => a.Name, new BsonRegularExpression(query.Name, "i")));
    }

    if (query.MinBalance.HasValue)
    {
        filters.Add(filterBuilder.Gte(a => a.Balance, query.MinBalance.Value));
    }

    if (query.MaxBalance.HasValue)
    {
        filters.Add(filterBuilder.Lte(a => a.Balance, query.MaxBalance.Value));
    }

    if (!query.IncludeInactive)
    {
        filters.Add(filterBuilder.Gt(a => a.Balance, 0));
    }

    // Additional complex filtering logic...
    
    var combinedFilter = filters.Any() 
        ? filterBuilder.And(filters) 
        : filterBuilder.Empty;

    return _collection.Find(combinedFilter).ToList();
}
```

### Observable Query Arguments

Observable queries can also accept arguments:

```csharp
[HttpGet("owner/{ownerId}/observable")]
public ISubject<IEnumerable<DebitAccount>> GetAccountsByOwnerObservable(CustomerId ownerId)
{
    return _collection.Observe(account => account.Owner == ownerId);
}

[HttpGet("filtered-observable")]
public ISubject<IEnumerable<DebitAccount>> GetFilteredAccountsObservable(
    [FromQuery] decimal? minBalance = null)
{
    if (minBalance.HasValue)
    {
        return _collection.Observe(account => account.Balance >= minBalance.Value);
    }
    
    return _collection.Observe();
}
```

### Argument Types

The application model supports various argument types:

#### Primitive Types

```csharp
[HttpGet("by-balance")]
public IEnumerable<DebitAccount> GetAccountsByBalance(
    [FromQuery] decimal balance,
    [FromQuery] bool exactMatch = false)
{
    return exactMatch 
        ? _collection.Find(a => a.Balance == balance).ToList()
        : _collection.Find(a => a.Balance >= balance).ToList();
}
```

#### Concept Types

Using concept types (value objects) for stronger typing:

```csharp
[HttpGet("by-owner-concept/{ownerId}")]
public IEnumerable<DebitAccount> GetAccountsByOwnerConcept(CustomerId ownerId)
{
    return _collection.Find(a => a.Owner == ownerId).ToList();
}
```

#### Collection Arguments

```csharp
[HttpGet("by-ids")]
public IEnumerable<DebitAccount> GetAccountsByIds([FromQuery] AccountId[] ids)
{
    return _collection.Find(a => ids.Contains(a.Id)).ToList();
}

[HttpGet("by-owners")]
public IEnumerable<DebitAccount> GetAccountsByOwners([FromQuery] List<CustomerId> ownerIds)
{
    return _collection.Find(a => ownerIds.Contains(a.Owner)).ToList();
}
```

#### Enums

```csharp
public enum AccountStatus { Active, Inactive, Suspended }

[HttpGet("by-status")]
public IEnumerable<DebitAccount> GetAccountsByStatus([FromQuery] AccountStatus status)
{
    // Implement status filtering logic
    return status switch
    {
        AccountStatus.Active => _collection.Find(a => a.Balance > 0).ToList(),
        AccountStatus.Inactive => _collection.Find(a => a.Balance == 0).ToList(),
        AccountStatus.Suspended => _collection.Find(a => a.Balance < 0).ToList(),
        _ => _collection.Find(_ => false).ToList()
    };
}
```

### Nullable Arguments

Optional arguments should be nullable:

```csharp
[HttpGet("flexible-search")]
public IEnumerable<DebitAccount> FlexibleSearch(
    [FromQuery] string? name = null,
    [FromQuery] CustomerId? ownerId = null,
    [FromQuery] decimal? minBalance = null,
    [FromQuery] decimal? maxBalance = null)
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

    return _collection.Find(combinedFilter).ToList();
}
```

### Default Values

Provide sensible default values for optional parameters:

```csharp
[HttpGet("paged")]
public IEnumerable<DebitAccount> GetPagedAccounts(
    [FromQuery] int page = 0,
    [FromQuery] int pageSize = 50,
    [FromQuery] string sortBy = "name",
    [FromQuery] bool ascending = true)
{
    var query = _collection.Find(_ => true);
    
    // Apply sorting
    query = ascending 
        ? query.SortBy(sortBy) 
        : query.SortByDescending(sortBy);
    
    // Apply paging
    return query.Skip(page * pageSize).Limit(pageSize).ToList();
}
```

> **Note**: The [proxy generator](../proxy-generation.md) automatically creates TypeScript types for your query arguments,
> making them strongly typed on the frontend as well.
