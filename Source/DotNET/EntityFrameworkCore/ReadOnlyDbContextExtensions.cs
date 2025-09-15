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
    /// Configures the DbContext to use the database specified in the connection string.
    /// The database type is inferred from the connection string.
    /// </summary>
    /// <param name="builder">The DbContext options builder to configure.</param>
    /// <param name="connectionString">The connection string to use.</param>
    /// <returns>The configured DbContext options builder.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the connection string does not have a supported database type.</exception>
    public static DbContextOptionsBuilder UseDatabaseFromConnectionString(this DbContextOptionsBuilder builder, string connectionString)
    {
        var type = connectionString.GetDatabaseType();
        return type switch
        {
            DatabaseType.Sqlite => builder.UseSqlite(connectionString),
            DatabaseType.SqlServer => builder.UseSqlServer(connectionString),
            DatabaseType.PostgreSql => builder.UseNpgsql(connectionString),
            _ => throw new UnsupportedDatabaseType(connectionString)
        };
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

    /// <summary>
    /// Infers the database type from the connection string.
    /// </summary>
    /// <param name="connectionString">The connection string to infer the database type from.</param>
    /// <returns>The inferred database type.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the connection string does not have a supported database type.</exception>
    public static DatabaseType GetDatabaseType(this string connectionString) => connectionString switch
    {
        _ when connectionString.Contains("Data Source=") || connectionString.Contains("Filename=") => DatabaseType.Sqlite,
        _ when connectionString.Contains("Server=") && connectionString.Contains("Database=") => DatabaseType.SqlServer,
        _ when connectionString.Contains("Host=") && connectionString.Contains("Database=") => DatabaseType.PostgreSql,
        _ => throw new UnsupportedDatabaseType(connectionString)
    };
}
