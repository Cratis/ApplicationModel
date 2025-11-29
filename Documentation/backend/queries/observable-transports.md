# Observable Query Transports

Starting with .NET 10, observable queries support multiple transport mechanisms for streaming real-time data updates to clients. The application model provides a flexible transport selection system with automatic fallback, similar to SignalR's approach.

## Supported Transports

### WebSocket

WebSocket provides full-duplex communication channels over a single TCP connection. This is the preferred transport for observable queries when available.

**Advantages:**
- Bidirectional communication
- Low latency
- Efficient for high-frequency updates
- Widely supported by modern browsers

**Requirements:**
- Client must send WebSocket upgrade headers
- HTTP/1.1 or HTTP/2 connection
- Server and all proxies must support WebSocket upgrades

### Server-Sent Events (SSE)

Server-Sent Events (SSE) provides a unidirectional, server-to-client streaming mechanism over HTTP. This serves as a reliable fallback when WebSocket is not available.

**Advantages:**
- Works through most proxies and firewalls
- Automatic reconnection built into the browser API
- Simple HTTP-based protocol
- No special server configuration needed

**Limitations:**
- Unidirectional (server to client only)
- Limited to text-based data (JSON serialization)

## Transport Selection

The application model automatically selects the appropriate transport based on:

1. **Configured preferred transport** (default: ServerSentEvents)
2. **Client request headers**
3. **Transport availability**
4. **Fallback settings** (default: enabled)

### Default Behavior

By default, the system prefers Server-Sent Events (SSE) as the primary transport, with automatic fallback to WebSocket if needed:

```csharp
// Default configuration (no action needed)
// PreferredTransport: ServerSentEvents
// EnableFallback: true
// 1. Try SSE for any HTTP request
// 2. Fall back to WebSocket if client sends upgrade headers and SSE is not available
```

### Custom Transport Configuration

You can configure transport preferences in your application startup:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Configure observable query transports
        builder.Services.ConfigureObservableQueryTransport(options =>
        {
            // Prefer WebSocket over SSE
            options.PreferredTransport = TransportType.WebSocket;
            
            // Disable automatic fallback (use only preferred transport)
            options.EnableFallback = false;
        });
        
        var app = builder.Build();
        app.Run();
    }
}
```

### WebSocket-Preferred Configuration

To prefer WebSocket with SSE fallback:

```csharp
builder.Services.ConfigureObservableQueryTransport(options =>
{
    options.PreferredTransport = TransportType.WebSocket;
    // EnableFallback defaults to true
});
```

### SSE-Only Configuration

To use only Server-Sent Events (no fallback):

```csharp
builder.Services.ConfigureObservableQueryTransport(options =>
{
    options.PreferredTransport = TransportType.ServerSentEvents;
    options.EnableFallback = false;
});
```

### WebSocket-Only Configuration

To use only WebSocket (no fallback):

```csharp
builder.Services.ConfigureObservableQueryTransport(options =>
{
    options.PreferredTransport = TransportType.WebSocket;
    options.EnableFallback = false;
});
```

## Client-Side Transport Selection

Clients can influence transport selection through HTTP headers:

### Requesting WebSocket

```http
GET /api/accounts/get-all-accounts-observable
Host: example.com
Upgrade: websocket
Connection: Upgrade
Sec-WebSocket-Key: dGhlIHNhbXBsZSBub25jZQ==
Sec-WebSocket-Version: 13
```

### Requesting SSE

```http
GET /api/accounts/get-all-accounts-observable
Host: example.com
Accept: text/event-stream
```

### Default Request

For regular HTTP requests without specific headers, the server will use the first configured transport (defaulting to SSE as the universal fallback):

```http
GET /api/accounts/get-all-accounts-observable
Host: example.com
```

## Transport Capabilities

| Feature | WebSocket | SSE |
|---------|-----------|-----|
| Bidirectional | ✅ | ❌ |
| Automatic reconnection | ❌ (client must implement) | ✅ (built into browser) |
| Binary data | ✅ | ❌ |
| Proxy compatibility | ⚠️ (may require configuration) | ✅ |
| Browser support | ✅ (all modern browsers) | ✅ (all modern browsers) |
| Connection overhead | Low | Medium |
| Latency | Very low | Low |

## Backend Implementation

The transport mechanism is transparent to your observable query implementation. Both transports work with the same code:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static ISubject<IEnumerable<DebitAccount>> GetAllAccountsObservable(
        IMongoCollection<DebitAccount> collection)
    {
        var observable = new ClientObservable<IEnumerable<DebitAccount>>();
        
        // Implementation works with both WebSocket and SSE
        var accounts = GetAllAccounts(collection);
        observable.OnNext(accounts);
        
        return observable;
    }
}
```

## Frontend Integration

The proxy generator automatically handles transport negotiation on the frontend:

```typescript
// TypeScript client automatically uses the best available transport
const accountsObservable = await DebitAccount.getAllAccountsObservable();

accountsObservable.subscribe(accounts => {
    console.log('Received accounts:', accounts);
});
```

## Performance Considerations

### When to Use WebSocket

- High-frequency updates (multiple times per second)
- Low latency requirements
- Bidirectional communication needs
- Stable network connections

### When to Use SSE

- Moderate update frequencies (seconds to minutes)
- Operating behind restrictive proxies or firewalls
- Need for automatic reconnection
- Unidirectional data flow is sufficient

## Troubleshooting

### WebSocket Connection Fails

If WebSocket connections consistently fail:

1. Check that all proxies support WebSocket upgrades
2. Verify firewall rules allow WebSocket traffic
3. Consider configuring SSE as the primary transport
4. Check browser console for connection errors

### SSE Connection Drops

If SSE connections drop frequently:

1. Check proxy and load balancer timeout settings
2. Verify HTTP keep-alive is properly configured
3. Monitor server resource usage
4. Consider implementing custom reconnection logic

## Best Practices

1. **Use default transport preferences** unless you have specific requirements
2. **Let the system auto-select** transports based on client capabilities
3. **Monitor transport usage** to understand client patterns
4. **Test both transports** in your deployment environment
5. **Consider proxy and firewall configurations** when deploying
6. **Use SSE for scenarios with restrictive network environments**
7. **Prefer WebSocket for real-time gaming or collaborative editing scenarios**

## Migration from WebSocket-Only

If you're upgrading from a version that only supported WebSocket, no code changes are required. The default configuration maintains WebSocket as the preferred transport, with SSE as an automatic fallback for improved reliability.

Existing clients will continue using WebSocket, while new clients can benefit from the improved fallback mechanism.
