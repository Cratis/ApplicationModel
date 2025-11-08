# Observable Queries

Observable queries provide real-time data streaming using WebSockets, enabling reactive user experiences where data changes are pushed to clients as they occur. You achieve this by returning `ISubject<T>` from your controller actions.

## Basic Observable Query

The key to an observable query is to leverage the `ClientObservable<T>` generic type or MongoDB extension methods:

```csharp
[HttpGet("observable")]
public ISubject<IEnumerable<DebitAccount>> AllAccountsObservable()
{
    return _collection.Observe(); // Simple MongoDB extension method
}
```

## Observable with Arguments

Observable queries can accept arguments just like regular queries:

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

## Single Object Observable

For observing changes to a single object:

```csharp
[HttpGet("{id}/observable")]
public ISubject<DebitAccount> GetAccountObservable(AccountId id)
{
    return _collection.Observe(account => account.Id == id);
}
```

## Custom Observable Logic

For more complex scenarios, you can implement custom observable logic using `ClientObservable<T>`:

```csharp
[HttpGet("summary")]
public ISubject<AccountSummary> GetAccountSummaryObservable()
{
    var observable = new ClientObservable<AccountSummary>();
    
    var calculateSummary = () =>
    {
        var accounts = _collection.Find(_ => true).ToList();
        return new AccountSummary(accounts.Count, accounts.Sum(a => a.Balance));
    };

    // Send initial summary
    observable.OnNext(calculateSummary());

    // Watch for any account changes
    var cursor = _collection.Watch();
    Task.Run(() =>
    {
        while (cursor.MoveNext())
        {
            if (cursor.Current.Any())
            {
                observable.OnNext(calculateSummary());
            }
        }
    });

    observable.ClientDisconnected = () => cursor.Dispose();
    return observable;
}
```

## Multiple Collection Observables

Observe changes across multiple collections:

```csharp
public record CombinedData(IEnumerable<DebitAccount> Accounts, IEnumerable<Customer> Customers);

[HttpGet("combined-observable")]
public ISubject<CombinedData> GetCombinedDataObservable()
{
    var observable = new ClientObservable<CombinedData>();
    
    var sendUpdate = () =>
    {
        var accounts = _accountCollection.Find(_ => true).ToList();
        var customers = _customerCollection.Find(_ => true).ToList();
        observable.OnNext(new CombinedData(accounts, customers));
    };

    // Send initial data
    sendUpdate();

    // Watch both collections
    var accountCursor = _accountCollection.Watch();
    var customerCursor = _customerCollection.Watch();
    
    Task.Run(() =>
    {
        while (accountCursor.MoveNext() || customerCursor.MoveNext())
        {
            if (accountCursor.Current.Any() || customerCursor.Current.Any())
            {
                sendUpdate();
            }
        }
    });

    observable.ClientDisconnected = () =>
    {
        accountCursor.Dispose();
        customerCursor.Dispose();
    };
    
    return observable;
}
```

## Observable with Computed Values

Create observables that compute derived values:

```csharp
[HttpGet("computed-metrics")]
public ISubject<AccountMetrics> GetAccountMetricsObservable()
{
    var observable = new ClientObservable<AccountMetrics>();
    
    var computeMetrics = () =>
    {
        var accounts = _collection.Find(_ => true).ToList();
        return new AccountMetrics(
            TotalAccounts: accounts.Count,
            TotalBalance: accounts.Sum(a => a.Balance),
            AverageBalance: accounts.Count > 0 ? accounts.Average(a => a.Balance) : 0,
            ActiveAccounts: accounts.Count(a => a.Balance > 0),
            HighValueAccounts: accounts.Count(a => a.Balance > 100000)
        );
    };

    // Send initial metrics
    observable.OnNext(computeMetrics());

    // Watch for changes and recompute
    var cursor = _collection.Watch();
    Task.Run(() =>
    {
        while (cursor.MoveNext())
        {
            if (cursor.Current.Any())
            {
                observable.OnNext(computeMetrics());
            }
        }
    });

    observable.ClientDisconnected = () => cursor.Dispose();
    return observable;
}
```

## Filtered Observables with Dynamic Criteria

Allow clients to specify filter criteria for observables:

```csharp
public record ObservableFilter(
    decimal? MinBalance,
    decimal? MaxBalance,
    string? NamePattern,
    CustomerId? OwnerId);

[HttpPost("filtered-observable")]
public ISubject<IEnumerable<DebitAccount>> GetFilteredObservable([FromBody] ObservableFilter filter)
{
    // Build the filter expression
    var filterBuilder = Builders<DebitAccount>.Filter;
    var filters = new List<FilterDefinition<DebitAccount>>();

    if (filter.MinBalance.HasValue)
        filters.Add(filterBuilder.Gte(a => a.Balance, filter.MinBalance.Value));

    if (filter.MaxBalance.HasValue)
        filters.Add(filterBuilder.Lte(a => a.Balance, filter.MaxBalance.Value));

    if (!string.IsNullOrEmpty(filter.NamePattern))
        filters.Add(filterBuilder.Regex(a => a.Name, new BsonRegularExpression(filter.NamePattern, "i")));

    if (filter.OwnerId.HasValue)
        filters.Add(filterBuilder.Eq(a => a.Owner, filter.OwnerId.Value));

    var combinedFilter = filters.Any() 
        ? filterBuilder.And(filters) 
        : filterBuilder.Empty;

    // Use the computed filter for observation
    return _collection.Observe(combinedFilter);
}
```

## Throttled Observables

For high-frequency changes, implement throttling to prevent overwhelming clients:

```csharp
[HttpGet("throttled-observable")]
public ISubject<IEnumerable<DebitAccount>> GetThrottledObservable(
    [FromQuery] int throttleMs = 1000)
{
    var observable = new ClientObservable<IEnumerable<DebitAccount>>();
    var throttleTimer = new Timer(throttleMs);
    var pendingUpdate = false;

    var sendUpdate = () =>
    {
        var accounts = _collection.Find(_ => true).ToList();
        observable.OnNext(accounts);
        pendingUpdate = false;
    };

    // Send initial data
    sendUpdate();

    // Watch for changes with throttling
    var cursor = _collection.Watch();
    Task.Run(() =>
    {
        while (cursor.MoveNext())
        {
            if (cursor.Current.Any() && !pendingUpdate)
            {
                pendingUpdate = true;
                throttleTimer.Elapsed += (s, e) => 
                {
                    sendUpdate();
                    throttleTimer.Stop();
                };
                throttleTimer.Start();
            }
        }
    });

    observable.ClientDisconnected = () =>
    {
        cursor.Dispose();
        throttleTimer.Dispose();
    };

    return observable;
}
```

## Error Handling in Observables

Implement proper error handling for robust observables:

```csharp
[HttpGet("robust-observable")]
public ISubject<IEnumerable<DebitAccount>> GetRobustObservable()
{
    var observable = new ClientObservable<IEnumerable<DebitAccount>>();
    
    try
    {
        var sendUpdate = () =>
        {
            try
            {
                var accounts = _collection.Find(_ => true).ToList();
                observable.OnNext(accounts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating observable accounts");
                observable.OnError(ex);
            }
        };

        // Send initial data
        sendUpdate();

        // Watch for changes with error handling
        var cursor = _collection.Watch();
        Task.Run(() =>
        {
            try
            {
                while (cursor.MoveNext())
                {
                    if (cursor.Current.Any())
                    {
                        sendUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in observable watch cursor");
                observable.OnError(ex);
            }
        });

        observable.ClientDisconnected = () => cursor?.Dispose();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error initializing observable");
        observable.OnError(ex);
    }

    return observable;
}
```

## Authentication and Authorization

Observable queries support the same authentication and authorization as regular queries:

```csharp
[Authorize]
[HttpGet("secure-observable")]
public ISubject<IEnumerable<DebitAccount>> GetSecureObservable()
{
    // Only authenticated users can subscribe
    return _collection.Observe();
}

[Authorize(Roles = "Admin")]
[HttpGet("admin-observable")]
public ISubject<IEnumerable<DebitAccount>> GetAdminObservable()
{
    // Only admin users can subscribe
    return _collection.Observe();
}
```

## Best Practices for Observable Queries

1. **Use the `.Observe()` extension method** for simple cases - it handles initial data load and change monitoring automatically
2. **Always handle client disconnection** with the `ClientDisconnected` callback when using `ClientObservable<T>` directly
3. **Send initial data immediately** before setting up change monitoring
4. **Use appropriate filters** to minimize unnecessary data transmission
5. **Consider throttling** for high-frequency changes to prevent overwhelming clients
6. **Implement error handling** to gracefully handle database connection issues
7. **Clean up resources** properly when clients disconnect
8. **Use authentication** to control who can subscribe to observable endpoints
9. **Monitor performance** and consider the impact of many concurrent subscriptions

## Connection Management

The application model automatically handles WebSocket connections for observable queries:

- **Connection establishment** - Automatic WebSocket upgrade for observable endpoints
- **Message serialization** - Automatic JSON serialization of observable data
- **Connection cleanup** - Proper disposal of resources when clients disconnect
- **Reconnection handling** - Clients can reconnect and resume subscriptions

## Frontend Integration

Observable queries integrate seamlessly with frontend frameworks through the proxy generator:

```typescript
// Generated TypeScript proxy automatically handles WebSocket connections
const accountsObservable = await accountsProxy.getAllAccountsObservable();

accountsObservable.subscribe(accounts => {
    // Handle real-time account updates
    updateUI(accounts);
});
```

> **Important**: When using `ClientObservable<T>` directly, the `ClientDisconnected` callback is essential for cleaning up resources like MongoDB cursors to prevent memory leaks.

> **Note**: The [proxy generator](../proxy-generation.md) automatically creates TypeScript types for your observable queries,
> making them strongly typed on the frontend as well.
