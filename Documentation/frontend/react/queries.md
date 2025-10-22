# Queries

Queries in the frontend is divided into the following:

- The underlying `IQueryFor<>`, `IObservableQueryFor<>` interfaces
- The React hooks; `useQuery()` and `useObservableQuery()``
- Proxy generator that generates TypeScript from the C# code to leverage the constructs.

## HTTP Headers

Queries automatically include any HTTP headers provided by the `httpHeadersCallback` configured in the [ApplicationModel](./application-model.md). This enables you to dynamically include authentication cookies, authorization tokens, or other custom headers with every query request without manual configuration for each query.

## Proxy Generation

Starting with the latter; the [proxy generator](./proxy-generation.md) you'll get the queries generated directly to use
in the frontend. The proxies generated can be imported directly into your code.

## Query

From a React component you can now use the static `use()` method:

```typescript
export const MyComponent = () => {
    const [accounts, queryAccounts] = AllAccounts.use();

    return (
        <>
        </>
    )
};
```

> Note: All data resulting from a query will be strongly typed based on the metadata provided by the proxy generator.
> You can read more about how serialization works [here](../../../fundamentals/serialization.md).

### Return tuple

If the query is a regular request / response type of query, the tuple returned contains two elements.
If it is an observable query, it only returns the first element of the tuple.

The return values are:

- The query result
- Delegate for issuing the query again

#### QueryResultWithState

The query result returned is a type called `QueryResultWithState` this is a sub type of `QueryResult`
adding properties that are relevant when working in React.

From the base `QueryResult` one gets the following properties:

| Property | Description |
| -------- | ----------- |
| data     | The actual data returned in the type expected. |
| isSuccess | Boolean telling whether or not the query was successful or not. |
| isAuthorized | Boolean telling whether or not the query was authorized or not. |
| isValid | Boolean telling whether or not the query was valid or not. |
| ValidationResult | Collection with any validation errors. |
| hasExceptions | Boolean telling whether or not the query had exceptions or not. |
| exceptionMessages | Collection with any exception messages. |
| exceptionStackTrace | The stack trace for the exception if there was one. |
| paging | Contains paging information, with current page number, page size, total number of items and total number pages |

On top of this `QueryResultWithState` adds the following properties:

| Property | Description |
| -------- | ----------- |
| hasData  | Boolean indicating whether or not there is data in the result. |
| isPerforming | Boolean that is true when an operation is working to get data from the server. |

### Parameters

Queries can have parameters they can be used for instance for filtering.
Lets say you have a query called `StartingWith`:

```csharp
[HttpGet("starting-with")]
public IEnumerable<DebitAccount> StartingWith([FromQuery] string? filter)
{
    var filterDocument = Builders<DebitAccount>
        .Filter
        .Regex("name", $"^{filter ?? string.Empty}.*");

    return _collection.Find(filterDocument).ToList();
}
```

The `filter` parameter will be part of the generated proxy, since it has the `[FromQuery]`
attribute on it. Using the proxy requires you to now specify the parameter as well:

```typescript
export const MyComponent = () => {
    const [accounts, queryAccounts] = StartingWith.use({ filter: '' });

    return (
        <>
        </>
    )
};
```

> Note: Route values will also be considered parameters and generated when adorning
> a method parameter with `[HttpPost]`.
