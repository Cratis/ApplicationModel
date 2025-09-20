// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Extensions for working with read-only DbContexts.
/// </summary>
public static class ReadOnlyDbContextExtensions
{
    /// <summary>
    /// Adds a DbContext with the specified connection string.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="connectionString">The connection string to use for the DbContext.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDbContextWithConnectionString<TDbContext>(this IServiceCollection services, string connectionString, Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : BaseDbContext
    {
        services.AddDbContext<TDbContext>(options =>
        {
            options.UseDatabaseFromConnectionString(connectionString);
            optionsAction(options);
        });
        return services;
    }

    /// <summary>
    /// Adds a read-only DbContext to the service collection.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddReadOnlyDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .AddInterceptors(new ReadOnlySaveChangesInterceptor());
            optionsAction(options);
        });
        return services;
    }

    /// <summary>
    /// Adds a read-only DbContext to the service collection, configured with the provided connection string
    /// and database type.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="connectionString">The connection string to use for the DbContext.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddReadOnlyDbContextWithConnectionString<TDbContext>(this IServiceCollection services, string connectionString, Action<DbContextOptionsBuilder> optionsAction)
        where TDbContext : BaseDbContext
    {
        services.AddReadOnlyDbContext<TDbContext>(builder =>
        {
            builder.UseDatabaseFromConnectionString(connectionString);
            optionsAction(builder);
        });

        return services;
    }
}
