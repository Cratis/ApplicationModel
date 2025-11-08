# Dependency Injection

Controller-based queries support full dependency injection through their constructors, allowing you to inject services, repositories, loggers, and other dependencies from the service collection.

## Constructor Injection

The most common pattern is to inject dependencies through the controller's constructor:

```csharp
[Route("api/accounts")]
public class Accounts : Controller
{
    readonly IAccountService _accountService;
    readonly ILogger<Accounts> _logger;
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(
        IAccountService accountService, 
        ILogger<Accounts> logger,
        IMongoCollection<DebitAccount> collection)
    {
        _accountService = accountService;
        _logger = logger;
        _collection = collection;
    }

    [HttpGet]
    public IEnumerable<DebitAccount> AllAccounts()
    {
        _logger.LogInformation("Retrieving all accounts");
        return _accountService.GetAllAccounts();
    }
}
```

## Common Dependency Types

### Database Collections

MongoDB collections are commonly injected:

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;

    public Accounts(IMongoCollection<DebitAccount> collection)
    {
        _collection = collection;
    }

    [HttpGet]
    public async Task<IEnumerable<DebitAccount>> GetAccountsAsync()
    {
        var result = await _collection.FindAsync(_ => true);
        return result.ToList();
    }
}
```

### Entity Framework DbContext

For Entity Framework Core scenarios:

```csharp
public class Accounts : Controller
{
    readonly ApplicationDbContext _dbContext;

    public Accounts(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IEnumerable<DebitAccount>> GetAccountsAsync()
    {
        return await _dbContext.Accounts.ToListAsync();
    }
}
```

### Business Services

Inject business logic services:

```csharp
public class Accounts : Controller
{
    readonly IAccountService _accountService;
    readonly ICustomerService _customerService;

    public Accounts(IAccountService accountService, ICustomerService customerService)
    {
        _accountService = accountService;
        _customerService = customerService;
    }

    [HttpGet("{id}/details")]
    public async Task<AccountDetails> GetAccountDetails(AccountId id)
    {
        var account = await _accountService.GetAccountAsync(id);
        var customer = await _customerService.GetCustomerAsync(account.Owner);
        
        return new AccountDetails(account, customer);
    }
}
```

### Logging

Structured logging with dependency injection:

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;
    readonly ILogger<Accounts> _logger;

    public Accounts(IMongoCollection<DebitAccount> collection, ILogger<Accounts> logger)
    {
        _collection = collection;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<IEnumerable<DebitAccount>> SearchAccounts([FromQuery] string term)
    {
        _logger.LogInformation("Searching accounts with term: {SearchTerm}", term);

        var filter = Builders<DebitAccount>.Filter.Regex(
            a => a.Name, 
            new BsonRegularExpression(term, "i"));

        var result = await _collection.FindAsync(filter);
        var accounts = result.ToList();

        _logger.LogInformation("Found {AccountCount} accounts", accounts.Count);
        return accounts;
    }
}
```

### Configuration

Inject configuration objects:

```csharp
public class Accounts : Controller
{
    readonly IMongoCollection<DebitAccount> _collection;
    readonly AccountQueryOptions _options;

    public Accounts(
        IMongoCollection<DebitAccount> collection, 
        IOptions<AccountQueryOptions> options)
    {
        _collection = collection;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<IEnumerable<DebitAccount>> GetAccounts()
    {
        var result = await _collection.FindAsync(_ => true);
        return result.Limit(_options.DefaultPageSize).ToList();
    }
}
```

## Service Registration

Make sure your dependencies are registered in the service collection:

```csharp
// In Program.cs or Startup.cs
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.Configure<AccountQueryOptions>(
    builder.Configuration.GetSection("AccountQueries"));
```

## Multiple Dependencies

Controllers can have many dependencies injected:

```csharp
public class Accounts : Controller
{
    readonly IAccountService _accountService;
    readonly ICustomerService _customerService;
    readonly ICachingService _cache;
    readonly ILogger<Accounts> _logger;
    readonly IMapper _mapper;
    readonly AccountQueryOptions _options;

    public Accounts(
        IAccountService accountService,
        ICustomerService customerService,
        ICachingService cache,
        ILogger<Accounts> logger,
        IMapper mapper,
        IOptions<AccountQueryOptions> options)
    {
        _accountService = accountService;
        _customerService = customerService;
        _cache = cache;
        _logger = logger;
        _mapper = mapper;
        _options = options.Value;
    }

    [HttpGet("{id}")]
    public async Task<AccountDetails> GetAccount(AccountId id)
    {
        var cacheKey = $"account-{id}";
        
        var cached = await _cache.GetAsync<AccountDetails>(cacheKey);
        if (cached is not null)
        {
            _logger.LogInformation("Returning cached account {AccountId}", id);
            return cached;
        }

        _logger.LogInformation("Loading account {AccountId} from database", id);
        var account = await _accountService.GetAccountAsync(id);
        var customer = await _customerService.GetCustomerAsync(account.Owner);
        
        var result = _mapper.Map<AccountDetails>((account, customer));
        await _cache.SetAsync(cacheKey, result, _options.CacheExpiry);
        
        return result;
    }
}
```

## Generic Dependencies

You can inject generic types:

```csharp
public class GenericQueries<T> : Controller where T : class
{
    readonly IRepository<T> _repository;
    readonly ILogger<GenericQueries<T>> _logger;

    public GenericQueries(IRepository<T> repository, ILogger<GenericQueries<T>> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<T>> GetAll()
    {
        _logger.LogInformation("Getting all {EntityType}", typeof(T).Name);
        return await _repository.GetAllAsync();
    }
}
```

## Best Practices

1. **Use readonly fields** - Store injected dependencies as `readonly` fields
2. **Prefer constructor injection** over method injection or service locator patterns
3. **Keep constructors clean** - Don't perform logic in constructors, just store dependencies
4. **Use appropriate lifetimes** - Register services with appropriate lifetimes (Singleton, Scoped, Transient)
5. **Validate dependencies** - Ensure all required dependencies are registered in the DI container
6. **Use IOptions&lt;T&gt;** for configuration objects rather than injecting raw configuration

## Avoiding Service Locator

Don't use `IServiceProvider` directly in your controllers:

```csharp
// ❌ Don't do this - service locator anti-pattern
public class BadAccounts : Controller
{
    readonly IServiceProvider _serviceProvider;

    public BadAccounts(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [HttpGet]
    public IEnumerable<DebitAccount> GetAccounts()
    {
        var service = _serviceProvider.GetRequiredService<IAccountService>();
        return service.GetAllAccounts();
    }
}

// ✅ Do this instead - constructor injection
public class GoodAccounts : Controller
{
    readonly IAccountService _accountService;

    public GoodAccounts(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public IEnumerable<DebitAccount> GetAccounts()
    {
        return _accountService.GetAllAccounts();
    }
}
```
