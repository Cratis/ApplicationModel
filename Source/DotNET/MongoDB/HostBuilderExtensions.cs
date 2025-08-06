// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.Metrics;
using Cratis.Applications;
using Cratis.Applications.MongoDB;
using Cratis.Metrics;
using Cratis.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Microsoft.Extensions.Hosting;

/// <summary>
/// Provides extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    static IMongoDBClientFactory? _clientFactory;
    static IMongoServerResolver? _serverResolver;

    /// <summary>
    /// Use MongoDB in the solution. Configures default settings for the MongoDB Driver.
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/> to use MongoDB with.</param>
    /// <param name="configureMongoDB">The optional callback for configuring <see cref="IMongoDBBuilder"/>.</param>
    /// <param name="mongoDBConfigSectionPath">Optional string for the <see cref="MongoDBOptions"/> config section path.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    /// <remarks>
    /// It will automatically hook up any implementations of <see cref="IBsonClassMapFor{T}"/>
    /// and <see cref="ICanFilterMongoDBConventionPacksForType"/>.
    /// </remarks>
    public static IHostBuilder UseCratisMongoDB(
        this IHostBuilder builder,
        Action<IMongoDBBuilder>? configureMongoDB = default,
        string? mongoDBConfigSectionPath = null)
    {
        var mongoDBBuilder = CreateMongoDBBuilder(configureMongoDB);
        builder.ConfigureServices((_, services) => AddServices(
            services,
            mongoDBBuilder,
            mongoDBConfigSectionPath ?? ConfigurationPath.Combine("Cratis", "MongoDB")));
        return builder;
    }

    /// <summary>
    /// Use MongoDB in the solution. Configures default settings for the MongoDB Driver.
    /// </summary>
    /// <param name="builder"><see cref="IHostBuilder"/> to use MongoDB with.</param>
    /// <param name="configureOptions">Optional callback for configuring <see cref="MongoDBOptions"/>.</param>
    /// <param name="configureMongoDB">The optional callback for configuring <see cref="IMongoDBBuilder"/>.</param>
    /// <returns><see cref="IHostBuilder"/> for building continuation.</returns>
    /// <remarks>
    /// It will automatically hook up any implementations of <see cref="IBsonClassMapFor{T}"/>
    /// and <see cref="ICanFilterMongoDBConventionPacksForType"/>.
    /// </remarks>
    public static IHostBuilder UseCratisMongoDB(
        this IHostBuilder builder,
        Action<MongoDBOptions> configureOptions,
        Action<IMongoDBBuilder>? configureMongoDB = default)
    {
        var mongoDBBuilder = CreateMongoDBBuilder(configureMongoDB);
        builder.ConfigureServices((_, services) => AddServices(
            services,
            mongoDBBuilder,
            configureOptions: configureOptions));
        return builder;
    }

    static void AddServices(
        IServiceCollection services,
        MongoDBBuilder mongoDBBuilder,
        string? mongoDBConfigSectionPath = null,
        Action<MongoDBOptions>? configureOptions = default)
    {
        var optionsBuilder = AddOptions(services, configureOptions);
        if (!string.IsNullOrWhiteSpace(mongoDBConfigSectionPath))
        {
            optionsBuilder.BindConfiguration(mongoDBConfigSectionPath);
        }

        services.AddKeyedSingleton<IMeter<IMongoClient>>(Internals.MeterName, (sp, key) =>
        {
            var meter = sp.GetRequiredKeyedService<Meter>(key);
            return new Meter<IMongoClient>(meter);
        });

        services.AddHostedService<MongoDBInitializer>();

        if (mongoDBBuilder.NamingPolicy is not null)
        {
            services.AddSingleton(mongoDBBuilder.NamingPolicy);
        }
        else
        {
            services.AddSingleton(typeof(INamingPolicy), mongoDBBuilder.NamingPolicyType ?? typeof(DefaultNamingPolicy));
        }

        services.AddSingleton(sp =>
        {
            var policy = mongoDBBuilder.NamingPolicy ?? sp.GetRequiredService<INamingPolicy>() ?? new DefaultNamingPolicy();
            DatabaseExtensions.SetNamingPolicy(policy);
            return policy;
        });

        services.AddSingleton(typeof(IMongoServerResolver), mongoDBBuilder.ServerResolverType);
        services.AddSingleton(typeof(IMongoDatabaseNameResolver), mongoDBBuilder.DatabaseNameResolverType);
        services.AddSingleton<IMongoDBClientFactory, MongoDBClientFactory>();
        services.AddScoped(sp =>
        {
            // TODO: This will not work when multi tenant.
            _clientFactory ??= sp.GetRequiredService<IMongoDBClientFactory>();
            _serverResolver ??= sp.GetRequiredService<IMongoServerResolver>();
            return _clientFactory.Create();
        });

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var databaseNameResolver = sp.GetRequiredService<IMongoDatabaseNameResolver>();
            return client.GetDatabase(databaseNameResolver.Resolve());
        });

        services.AddScoped(typeof(IMongoCollection<>), typeof(MongoCollectionAdapter<>));
    }

    static MongoDBBuilder CreateMongoDBBuilder(Action<MongoDBBuilder>? configure)
    {
        var builder = new MongoDBBuilder();
        configure?.Invoke(builder);
        MongoDBDefaults.Initialize(builder);
        builder.Validate();
        return builder;
    }

    static OptionsBuilder<MongoDBOptions> AddOptions(IServiceCollection services, Action<MongoDBOptions>? configureOptions = default)
    {
        var builder = services
            .AddOptions<MongoDBOptions>()
            .ValidateOnStart()
            .ValidateDataAnnotations();

        if (configureOptions is not null)
        {
            builder.Configure(configureOptions);
        }

        return builder;
    }
}
