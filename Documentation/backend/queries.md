# Queries

Queries represents data that is queryable in the system
There are encapsulated as objects that is the output of a HTTP Get controller action in the backend.
A query can have inputs in the form of arguments that is typically part of the routing information or as the query-string,
these can have validation rules around them.

In addition to this, the controller can have authorization policies associated with it that applies to the action.

## Proxy Generation

As part of the toolchain there is a proxy generator that automatically generates TypeScript code from the C# code for
queries. Read more about how that works [here](../frontend/cqrs/proxy-generation.md).

## Regular Queries

To create a query, all you need is a model type and a controller with an action on it.
Lets start with the model:

```csharp
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, double Balance);
```

> Note: This particular model represents its values as concepts - a value type encapsulation that
> makes us not use primitives - thus creating clearer APIs and models.

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet]
    public IEnumerable<DebitAccount> AllAccounts() => _collection.Find(_ => true).ToList();
}
```

## Arguments

Sometimes you need to pass an argument into a query to do a concrete filter on from the datasource,
you can do this in the standard ASP.NET way and it will automatically be generated by the
[proxy generator](../frontend/cqrs/proxy-generation.md).

Below is an example:

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet("starting-with")]
    public async Task<IEnumerable<DebitAccount>> StartingWith([FromQuery] string? filter)
    {
        var filterDocument = Builders<DebitAccount>
            .Filter
            .Regex("name", $"^{filter ?? string.Empty}.*");

        var result = await _accountsCollection.FindAsync(filterDocument);
        return result.ToList();
    }
}
```

## Observable Queries

Another feature that is part of the application model is to have live observable queries
that leverages WebSockets. This is very useful when you want to create a reactive
user experience and present data or state changes when they actually change, rather
than doing a pull on an interval.

For instance, MongoDB has an API for watching for changes. Once a change occurs,
we can either send just the change that happened or reissue the query and get all that.

The key to an observable query is to leverage the `ClientObservable` generic type.
Every request to your controller action is considered a new client connecting and
you would therefor have one instance of this type per request.

Below is an example showing how to do this with the MongoDB watch API.

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet]
    public ISubject<IEnumerable<DebitAccount>> AllAccounts()
    {
        var observable = new ClientObservable<IEnumerable<DebitAccount>>();
        var accounts = _accountsCollection.Find(_ => true).ToList();
        observable.OnNext(accounts);
        var cursor = _accountsCollection.Watch();

        Task.Run(() =>
        {
            while (cursor.MoveNext())
            {
                if (!cursor.Current.Any()) continue;
                observable.OnNext(_accountsCollection.Find(_ => true).ToList());
            }
        });

        observable.ClientDisconnected = () => cursor.Dispose();

        return observable;
    }
}
```

> Note: The `ClientObservable` holds a `ClientDisconnected` callback that gets called when a client
> has disconnected. Use this to cleanup.

### Simplified

The pattern of getting data and respond to changes is a very common one, instead of having to
do the boilerplate above all over the place - there are extension methods that allow you do this
in one line:

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection) => _collection = collection;

    [HttpGet]
    public ISubject<IEnumerable<DebitAccount>> AllAccounts()
    {
        return _accountsCollection.Observe();   // <-
    }
}
```

> Note: The `.Observe()` method can take filters in the form of `Expression` or MongoDB `FilterDefinition`
> in the same way the regular `Find()/FindAsync()` methods of the MongoDB API works.
