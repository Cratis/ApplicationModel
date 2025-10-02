// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Extensions for working with DbContexts.
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Adds a DbContext with the specified connection string.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext.</typeparam>
    /// <param name="services">The service collection to add the DbContext to.</param>
    /// <param name="connectionString">The connection string to use for the DbContext.</param>
    /// <param name="optionsAction">An optional action to configure the DbContext options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddDbContextWithConnectionString<TDbContext>(this IServiceCollection services, string connectionString, Action<DbContextOptionsBuilder>? optionsAction = default)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(options =>
        {
            options.UseDatabaseFromConnectionString(connectionString);
            optionsAction?.Invoke(options);
        });
        return services;
    }
}
