// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Extension methods for adding DbContext services.
/// </summary>
public static class DbContextServiceCollectionExtensions
{
    /// <summary>
    /// Adds all DbContext types found in the specified assemblies to the service collection, configured as
    /// read-only DbContexts.
    /// </summary>
    /// <param name="services">The service collection to add the DbContext types to.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <param name="assemblies">The assemblies to scan for DbContext types.</param>
    /// <returns>The service collection, for chaining.</returns>
    public static IServiceCollection AddReadModelDbContextsFromAssemblies(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction, params Assembly[] assemblies)
    {
        var addDbContextMethod = typeof(ReadOnlyDbContextExtensions).GetMethod(nameof(ReadOnlyDbContextExtensions.AddReadOnlyDbContext), BindingFlags.Static | BindingFlags.Public)!;
        foreach (var dbContext in Types.Types.Instance.FindMultiple<ReadOnlyDbContext>().Where(t => assemblies.Contains(t.Assembly)))
        {
            addDbContextMethod.MakeGenericMethod(dbContext).Invoke(null, [services, optionsAction]);
        }
        return services;
    }

    /// <summary>
    /// Adds all DbContext types found in the specified assemblies to the service collection, configured with
    /// the provided connection string. The database type is inferred from the connection string.
    /// </summary>
    /// <param name="services">The service collection to add the DbContext types to.</param>
    /// <param name="connectionString">The connection string to use for the DbContext types.</param>
    /// <param name="optionsAction">An action to configure the DbContext options.</param>
    /// <param name="assemblies">The assemblies to scan for DbContext types.</param>
    /// <returns>The service collection, for chaining.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the connection string does not have a supported database type.</exception>
    public static IServiceCollection AddReadModelDbContextsWithConnectionStringFromAssemblies(this IServiceCollection services, string connectionString, Action<DbContextOptionsBuilder> optionsAction, params Assembly[] assemblies)
    {
        var addDbContextMethod = typeof(ReadOnlyDbContextExtensions).GetMethod(nameof(ReadOnlyDbContextExtensions.AddReadOnlyDbContextWithConnectionString), BindingFlags.Static | BindingFlags.Public)!;
        foreach (var dbContext in Types.Types.Instance.FindMultiple<ReadOnlyDbContext>().Where(t => assemblies.Contains(t.Assembly)))
        {
            addDbContextMethod.MakeGenericMethod(dbContext).Invoke(null, [services, connectionString, optionsAction]);
        }
        return services;
    }
}
