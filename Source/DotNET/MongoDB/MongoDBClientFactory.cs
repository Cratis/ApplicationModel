// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using Castle.DynamicProxy;
using Cratis.Applications.MongoDB.Resilience;
using Cratis.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using Polly;
using Polly.Retry;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IMongoDBClientFactory"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MongoDBClientFactory"/> class.
/// </remarks>
/// <param name="serverResolver"><see cref="IMongoServerResolver"/> for resolving the server.</param>
/// <param name="options"><see cref="IOptions{TOptions}"/> for getting the options.</param>
/// <param name="logger"><see cref="ILogger"/> for logging.</param>
[Singleton]
public class MongoDBClientFactory(IMongoServerResolver serverResolver, IOptions<MongoDBOptions> options, ILogger<MongoDBClientFactory> logger) : IMongoDBClientFactory
{
    readonly ConcurrentDictionary<string, IMongoClient> _clients = new();

    /// <inheritdoc/>
    public IMongoClient Create() => Create(serverResolver.Resolve());

    /// <inheritdoc/>
#pragma warning disable MA0106 // Avoid closure by using an overload with the 'factoryArgument' parameter
    public IMongoClient Create(MongoClientSettings settings) => _clients.GetOrAdd(settings.Server.ToString(), (_) => CreateImplementation(settings));
#pragma warning restore MA0106 // Avoid closure by using an overload with the 'factoryArgument' parameter

    /// <inheritdoc/>
    public IMongoClient Create(MongoUrl url) => Create(MongoClientSettings.FromUrl(url));

    /// <inheritdoc/>
    public IMongoClient Create(string connectionString) => Create(MongoClientSettings.FromConnectionString(connectionString));

    IMongoClient CreateImplementation(MongoClientSettings settings)
    {
        settings.DirectConnection = options.Value.DirectConnection;
        settings.ClusterConfigurator = ClusterConfigurator;
        logger.CreateClient(settings.Server.ToString());
#pragma warning disable CA2000 // Dispose objects before losing scope - we're returning the client
        var client = new MongoClient(settings);
#pragma warning restore CA2000 // Dispose objects before losing scope

        var resiliencePipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                UseJitter = true,
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromMilliseconds(500)
            }).Build();

        var proxyGenerator = new ProxyGenerator();
        var proxyGeneratorOptions = new ProxyGenerationOptions
        {
            Selector = new MongoClientInterceptorSelector(proxyGenerator, resiliencePipeline, client)
        };

        return proxyGenerator.CreateInterfaceProxyWithTarget<IMongoClient>(client, proxyGeneratorOptions);
    }

    void ClusterConfigurator(ClusterBuilder builder)
    {
        if (logger.IsEnabled(LogLevel.Trace))
        {
            builder
                .Subscribe<CommandStartedEvent>(CommandStarted)
                .Subscribe<CommandFailedEvent>(CommandFailed)
                .Subscribe<CommandSucceededEvent>(CommandSucceeded);
        }
    }

    void CommandStarted(CommandStartedEvent command) => logger.CommandStarted(command.RequestId, command.CommandName, command.Command.ToJson());

    void CommandFailed(CommandFailedEvent command) => logger.CommandFailed(command.RequestId, command.CommandName, command.Failure.Message);

    void CommandSucceeded(CommandSucceededEvent command) => logger.CommandSucceeded(command.RequestId, command.CommandName);
}
