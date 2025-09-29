# Observable Queries

Observable queries provide real-time data streaming using WebSockets, enabling reactive user experiences
where data changes are pushed to clients as they occur, rather than requiring client-side polling.

## Basic Observable Query

The key to an observable query is to leverage the `ClientObservable<T>` generic type.
Every request to your controller action is considered a new client connecting and you would
therefore have one instance of this type per request.

```csharp
using System.Reactive.Subjects;

[Route("api/accounts")]
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet("observable")]
    public ISubject<IEnumerable<DebitAccount>> AllAccountsObservable()
    {
        var observable = new ClientObservable<IEnumerable<DebitAccount>>();
        
        // Send initial data
        var accounts = _collection.Find(_ => true).ToList();
        observable.OnNext(accounts);
        
        // Set up change monitoring
        var cursor = _collection.Watch();
        Task.Run(() =>
        {
            while (cursor.MoveNext())
            {
                if (!cursor.Current.Any()) continue;
                observable.OnNext(_collection.Find(_ => true).ToList());
            }
        });

        // Cleanup when client disconnects
        observable.ClientDisconnected = () => cursor.Dispose();

        return observable;
    }
}
```

## MongoDB Watch API Integration

MongoDB provides a change stream API that's perfect for observable queries:

```csharp
[HttpGet("watch")]
public ISubject<IEnumerable<DebitAccount>> WatchAccounts()
{
    var observable = new ClientObservable<IEnumerable<DebitAccount>>();
    
    // Initial data load
    var initialData = _collection.Find(_ => true).ToList();
    observable.OnNext(initialData);
    
    // Watch for changes
    var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup };
    var cursor = _collection.Watch(options);
    
    Task.Run(async () =>
    {
        while (await cursor.MoveNextAsync())
        {
            foreach (var change in cursor.Current)
            {
                // You can send incremental changes or full refresh
                switch (change.OperationType)
                {
                    case ChangeStreamOperationType.Insert:
                    case ChangeStreamOperationType.Update:
                    case ChangeStreamOperationType.Delete:
                        // Refresh all data (or implement incremental updates)
                        var updatedData = _collection.Find(_ => true).ToList();
                        observable.OnNext(updatedData);
                        break;
                }
            }
        }
    });

    observable.ClientDisconnected = () => cursor.Dispose();
    return observable;
}
```

## Simplified Extension Methods

The application model provides extension methods to simplify the common pattern of loading initial data
and watching for changes:

```csharp
[HttpGet("simple")]
public ISubject<IEnumerable<DebitAccount>> AllAccountsSimple()
{
    return _collection.Observe();
}
```

### With Filters

The `.Observe()` method accepts filters just like the regular MongoDB `Find()` methods:

```csharp
[HttpGet("active")]
public ISubject<IEnumerable<DebitAccount>> GetActiveAccounts()
{
    return _collection.Observe(account => account.Balance > 0);
}

[HttpGet("by-owner/{ownerId}")]
public ISubject<IEnumerable<DebitAccount>> GetAccountsByOwner(CustomerId ownerId)
{
    return _collection.Observe(account => account.Owner == ownerId);
}
```

### Using MongoDB FilterDefinition

```csharp
[HttpGet("advanced-filter")]
public ISubject<IEnumerable<DebitAccount>> GetAccountsWithAdvancedFilter()
{
    var filter = Builders<DebitAccount>.Filter.And(
        Builders<DebitAccount>.Filter.Gt(account => account.Balance, 100),
        Builders<DebitAccount>.Filter.Lt(account => account.Balance, 10000)
    );
    
    return _collection.Observe(filter);
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

For more complex scenarios, you can implement custom observable logic:

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

## Error Handling

Handle errors gracefully in observable queries:

```csharp
[HttpGet("with-error-handling")]
public ISubject<IEnumerable<DebitAccount>> GetAccountsWithErrorHandling()
{
    var observable = new ClientObservable<IEnumerable<DebitAccount>>();

    try
    {
        var accounts = _collection.Find(_ => true).ToList();
        observable.OnNext(accounts);

        var cursor = _collection.Watch();
        Task.Run(async () =>
        {
            try
            {
                while (await cursor.MoveNextAsync())
                {
                    if (cursor.Current.Any())
                    {
                        var updatedAccounts = _collection.Find(_ => true).ToList();
                        observable.OnNext(updatedAccounts);
                    }
                }
            }
            catch (Exception ex)
            {
                observable.OnError(ex);
            }
        });

        observable.ClientDisconnected = () => cursor.Dispose();
    }
    catch (Exception ex)
    {
        observable.OnError(ex);
    }

    return observable;
}
```

## Client Disconnection Handling

Always handle client disconnections to prevent resource leaks:

```csharp
[HttpGet("with-cleanup")]
public ISubject<IEnumerable<DebitAccount>> GetAccountsWithCleanup()
{
    var observable = new ClientObservable<IEnumerable<DebitAccount>>();
    var cursor = _collection.Watch();
    var cancellationTokenSource = new CancellationTokenSource();

    // Initial data
    observable.OnNext(_collection.Find(_ => true).ToList());

    // Background monitoring
    Task.Run(async () =>
    {
        while (!cancellationTokenSource.Token.IsCancellationRequested)
        {
            if (await cursor.MoveNextAsync(cancellationTokenSource.Token))
            {
                if (cursor.Current.Any())
                {
                    observable.OnNext(_collection.Find(_ => true).ToList());
                }
            }
        }
    }, cancellationTokenSource.Token);

    // Cleanup resources when client disconnects
    observable.ClientDisconnected = () =>
    {
        cancellationTokenSource.Cancel();
        cursor.Dispose();
        cancellationTokenSource.Dispose();
    };

    return observable;
}
```

> **Important**: The `ClientDisconnected` callback is essential for cleaning up resources.
> Always dispose of cursors, cancellation tokens, and other resources to prevent memory leaks.

## Best Practices

1. **Always handle client disconnection** with the `ClientDisconnected` callback
2. **Send initial data immediately** before setting up change monitoring
3. **Use try-catch blocks** to handle errors gracefully
4. **Dispose of resources properly** to prevent memory leaks
5. **Consider the frequency of changes** and implement throttling if necessary
6. **Use appropriate filters** to minimize unnecessary data transmission
7. **Test WebSocket connections** thoroughly in your development environment