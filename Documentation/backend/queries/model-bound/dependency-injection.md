# Dependency Injection

Model-bound queries use method-level dependency injection, where dependencies are resolved and injected as parameters to your static query methods. This approach provides flexibility and testability while keeping the query logic clean and focused.

## How Method-Level Dependency Injection Works

Unlike controller-based queries that use constructor injection, model-bound queries inject dependencies directly as method parameters. The Arc framework automatically resolves these dependencies from the service collection based on their parameter types.

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static IEnumerable<DebitAccount> GetAllAccounts(
        IMongoCollection<DebitAccount> collection) // ← Dependency injected as parameter
    {
        return collection.Find(_ => true).ToList();
    }
}
```

## Common Dependency Types

### Database Collections

MongoDB collections are the most common dependencies:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetActiveAccountsAsync(
        IMongoCollection<DebitAccount> collection)
    {
        var result = await collection.FindAsync(a => a.Balance > 0);
        return result.ToList();
    }
}
```

### Entity Framework DbContext

For Entity Framework Core scenarios:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetAccountsFromEFAsync(
        ApplicationDbContext dbContext)
    {
        return await dbContext.DebitAccounts
            .Where(a => a.Balance >= 0)
            .ToListAsync();
    }
}
```

### Business Services

Inject domain services for complex business logic:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<AccountRiskAssessment> GetAccountRiskAssessment(
        AccountId accountId,
        IMongoCollection<DebitAccount> collection,
        IRiskCalculationService riskService,
        ITransactionHistoryService transactionService)
    {
        var account = await collection.Find(a => a.Id == accountId).FirstOrDefaultAsync();
        if (account is null)
            throw new AccountNotFoundException(accountId);
            
        var transactions = await transactionService.GetRecentTransactionsAsync(accountId);
        var riskScore = await riskService.CalculateRiskAsync(account, transactions);
        
        return new AccountRiskAssessment(accountId, riskScore);
    }
}
```

### Logging

Structured logging with dependency injection:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> SearchAccountsWithLogging(
        string searchTerm,
        IMongoCollection<DebitAccount> collection,
        ILogger<DebitAccount> logger)
    {
        logger.LogInformation("Searching accounts with term: {SearchTerm}", searchTerm);
        
        var filter = Builders<DebitAccount>.Filter.Regex(
            a => a.Name, 
            new BsonRegularExpression(searchTerm, "i"));
        
        var result = await collection.FindAsync(filter);
        var accounts = result.ToList();
        
        logger.LogInformation("Found {AccountCount} accounts matching '{SearchTerm}'", 
            accounts.Count, searchTerm);
            
        return accounts;
    }
}
```

### Configuration

Inject configuration objects using `IOptions<T>` or `IConfiguration`:

```csharp
public class AccountQueryOptions
{
    public int MaxSearchResults { get; set; } = 100;
    public TimeSpan CacheExpiry { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableAuditLogging { get; set; } = true;
}

[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetPagedAccountsWithOptions(
        int page,
        int pageSize,
        IMongoCollection<DebitAccount> collection,
        IOptions<AccountQueryOptions> options,
        ILogger<DebitAccount> logger)
    {
        var opts = options.Value;
        var actualPageSize = Math.Min(pageSize, opts.MaxSearchResults);
        
        if (opts.EnableAuditLogging)
        {
            logger.LogInformation("Retrieving page {Page} with size {PageSize}", page, actualPageSize);
        }
        
        var result = await collection.FindAsync(_ => true);
        return result.Skip(page * actualPageSize).Limit(actualPageSize).ToList();
    }
}
```

### Caching Services

Integrate caching for performance optimization:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetCachedAccountsByOwner(
        CustomerId ownerId,
        IMongoCollection<DebitAccount> collection,
        IMemoryCache cache,
        ILogger<DebitAccount> logger)
    {
        var cacheKey = $"accounts-by-owner-{ownerId}";
        
        if (cache.TryGetValue(cacheKey, out IEnumerable<DebitAccount>? cachedAccounts))
        {
            logger.LogInformation("Returning cached accounts for owner {OwnerId}", ownerId);
            return cachedAccounts ?? Enumerable.Empty<DebitAccount>();
        }
        
        logger.LogInformation("Loading accounts for owner {OwnerId} from database", ownerId);
        var accounts = await collection.Find(a => a.Owner == ownerId).ToListAsync();
        
        cache.Set(cacheKey, accounts, TimeSpan.FromMinutes(5));
        return accounts;
    }
}
```

## Parameter Order Flexibility

Dependencies can be placed in any position among your method parameters. The framework resolves them by type, not by position:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    // Dependencies first, then query parameters
    public static async Task<IEnumerable<DebitAccount>> GetAccountsByStatusPattern1(
        IMongoCollection<DebitAccount> collection,
        ILogger<DebitAccount> logger,
        AccountStatus status,
        bool includeInactive)
    {
        logger.LogInformation("Getting accounts by status: {Status}", status);
        // Implementation...
        return await collection.Find(_ => true).ToListAsync();
    }
    
    // Query parameters first, then dependencies
    public static async Task<IEnumerable<DebitAccount>> GetAccountsByStatusPattern2(
        AccountStatus status,
        bool includeInactive,
        IMongoCollection<DebitAccount> collection,
        ILogger<DebitAccount> logger)
    {
        logger.LogInformation("Getting accounts by status: {Status}", status);
        // Implementation...
        return await collection.Find(_ => true).ToListAsync();
    }
    
    // Mixed order
    public static async Task<IEnumerable<DebitAccount>> GetAccountsByStatusPattern3(
        AccountStatus status,
        IMongoCollection<DebitAccount> collection,
        bool includeInactive,
        ILogger<DebitAccount> logger)
    {
        logger.LogInformation("Getting accounts by status: {Status}", status);
        // Implementation...
        return await collection.Find(_ => true).ToListAsync();
    }
}
```

## Multiple Dependencies of Same Type

When you need multiple dependencies of the same type, use named dependencies or specific implementations:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<CrossAccountSummary> GetCrossAccountSummary(
        IMongoCollection<DebitAccount> debitCollection,
        IMongoCollection<CreditAccount> creditCollection,
        ILogger<DebitAccount> logger)
    {
        var debitAccounts = await debitCollection.Find(_ => true).ToListAsync();
        var creditAccounts = await creditCollection.Find(_ => true).ToListAsync();
        
        logger.LogInformation("Processing {DebitCount} debit and {CreditCount} credit accounts", 
            debitAccounts.Count, creditAccounts.Count);
        
        return new CrossAccountSummary(
            debitAccounts.Sum(a => a.Balance),
            creditAccounts.Sum(a => a.Balance));
    }
}
```

## Generic Dependencies

Use generic dependencies for flexible, reusable patterns:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetAccountsWithGenericRepository(
        IRepository<DebitAccount> repository,
        ILogger<DebitAccount> logger)
    {
        logger.LogInformation("Loading accounts using generic repository");
        return await repository.GetAllAsync();
    }
    
    public static async Task<DebitAccount?> GetAccountByIdWithGenericRepository(
        AccountId id,
        IRepository<DebitAccount> repository,
        IValidator<AccountId> validator)
    {
        var validationResult = await validator.ValidateAsync(id);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        
        return await repository.GetByIdAsync(id);
    }
}
```

## Scoped Dependencies

Dependencies are resolved with their registered lifetime (Singleton, Scoped, Transient):

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetAccountsWithScopedServices(
        IMongoCollection<DebitAccount> collection,      // Scoped
        ICurrentUserService currentUserService,         // Scoped  
        ISystemClock systemClock,                      // Singleton
        IAuditService auditService)                    // Scoped
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        var currentTime = systemClock.UtcNow;
        
        await auditService.LogQueryAsync("GetAccountsWithScopedServices", currentUser.Id, currentTime);
        
        // Filter based on user permissions
        var filter = BuildUserFilter(currentUser);
        return await collection.Find(filter).ToListAsync();
    }
    
    private static FilterDefinition<DebitAccount> BuildUserFilter(User user)
    {
        if (user.IsAdmin)
            return Builders<DebitAccount>.Filter.Empty;
            
        return Builders<DebitAccount>.Filter.Eq(a => a.Owner, user.CustomerId);
    }
}
```

## Service Registration

Ensure your dependencies are properly registered in the service collection:

```csharp
// In Program.cs or Startup.cs
builder.Services.AddScoped<IRiskCalculationService, RiskCalculationService>();
builder.Services.AddScoped<ITransactionHistoryService, TransactionHistoryService>();
builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.Configure<AccountQueryOptions>(
    builder.Configuration.GetSection("AccountQueries"));

// MongoDB collections are typically registered as:
builder.Services.AddScoped<IMongoCollection<DebitAccount>>(provider =>
{
    var database = provider.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<DebitAccount>("debit-accounts");
});
```

## Dependency Injection Best Practices

1. **Order parameters logically** - Group related parameters together, but remember that dependency resolution is by type
2. **Use specific interface types** - Prefer `ILogger<T>` over `ILogger`, `IOptions<TOptions>` over `IConfiguration`
3. **Avoid service locator pattern** - Don't inject `IServiceProvider` and resolve services manually
4. **Keep methods focused** - If you need many dependencies, consider if the method is doing too much
5. **Use appropriate lifetimes** - Understand Singleton, Scoped, and Transient lifetimes for your dependencies
6. **Test with mocked dependencies** - The method-level injection makes unit testing straightforward

## Testing with Dependency Injection

Method-level dependency injection makes unit testing simple:

```csharp
[Fact]
public async Task GetAccountsByOwner_Should_Return_Filtered_Accounts()
{
    // Arrange
    var mockCollection = Substitute.For<IMongoCollection<DebitAccount>>();
    var mockLogger = Substitute.For<ILogger<DebitAccount>>();
    var ownerId = new CustomerId(Guid.NewGuid());
    var expectedAccounts = new List<DebitAccount>
    {
        new(new AccountId(Guid.NewGuid()), new AccountName("Test Account"), ownerId, 1000m)
    };

    var mockCursor = Substitute.For<IAsyncCursor<DebitAccount>>();
    mockCursor.ToList().Returns(expectedAccounts);
    
    mockCollection.FindAsync(Arg.Any<FilterDefinition<DebitAccount>>())
        .Returns(mockCursor);

    // Act
    var result = await DebitAccount.GetAccountsByOwnerWithLogging(
        ownerId, 
        mockCollection, 
        mockLogger);

    // Assert
    result.Should().BeEquivalentTo(expectedAccounts);
    mockLogger.Received(1).LogInformation(
        Arg.Is<string>(s => s.Contains("Getting accounts for owner")),
        ownerId);
}
```

## Error Handling with Dependencies

Handle dependency-related errors gracefully:

```csharp
[ReadModel]
public record DebitAccount(AccountId Id, AccountName Name, CustomerId Owner, decimal Balance)
{
    public static async Task<IEnumerable<DebitAccount>> GetAccountsWithErrorHandling(
        IMongoCollection<DebitAccount> collection,
        ILogger<DebitAccount> logger,
        IHealthCheckService healthCheck)
    {
        try
        {
            // Check if database is healthy before querying
            var healthResult = await healthCheck.CheckHealthAsync();
            if (healthResult.Status != HealthStatus.Healthy)
            {
                logger.LogWarning("Database health check failed: {Status}", healthResult.Status);
                return Enumerable.Empty<DebitAccount>();
            }
            
            var result = await collection.FindAsync(_ => true);
            return result.ToList();
        }
        catch (MongoException ex)
        {
            logger.LogError(ex, "MongoDB error while retrieving accounts");
            throw new DataAccessException("Unable to retrieve accounts", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while retrieving accounts");
            throw;
        }
    }
}
```

Method-level dependency injection in model-bound queries provides a clean, testable, and flexible approach to accessing services and repositories while keeping your query logic focused and maintainable.
