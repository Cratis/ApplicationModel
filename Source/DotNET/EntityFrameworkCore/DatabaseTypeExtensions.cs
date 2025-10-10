// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Extensions for working with database connections.
/// </summary>
public static class DatabaseTypeExtensions
{
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
            DatabaseType.Sqlite => builder
                .UseSqlite(connectionString)
                .ReplaceService<IMigrationsSqlGenerator, MigrationsSqlGeneratorForSqlite>(),

            DatabaseType.SqlServer => builder
                .UseSqlServer(connectionString)
                .ReplaceService<IMigrationsSqlGenerator, MigrationsSqlGeneratorForSqlServer>(),

            DatabaseType.PostgreSql => builder
                .UseNpgsql(connectionString)
                .ReplaceService<IMigrationsSqlGenerator, MigrationsSqlGeneratorForPostgreSQL>(),

            _ => throw new UnsupportedDatabaseType(connectionString)
        };
    }

    /// <summary>
    /// Gets the database type from the migration builder's active provider.
    /// </summary>
    /// <param name="migrationBuilder">The migration builder to get the database type from.</param>
    /// <returns>The database type.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the active provider is not supported.</exception>
    public static DatabaseType GetDatabaseType(this MigrationBuilder migrationBuilder) => migrationBuilder.ActiveProvider.GetDatabaseTypeFromProvider();

    /// <summary>
    /// Gets the database type from the database connection.
    /// </summary>
    /// <param name="database">The database connection to get the type from.</param>
    /// <returns>The database type.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the connection string does not have a supported database type.</exception>
    public static DatabaseType GetDatabaseType(this DatabaseFacade database) => database.ProviderName.GetDatabaseTypeFromProvider();

    /// <summary>
    /// Gets the database type from the provider name.
    /// </summary>
    /// <param name="providerName">The provider name to get the database type from.</param>
    /// <returns>The database type.</returns>
    /// <exception cref="UnsupportedDatabaseType">Thrown if the provider name is not supported.</exception>
    public static DatabaseType GetDatabaseTypeFromProvider(this string? providerName) => providerName switch
    {
        "Microsoft.EntityFrameworkCore.Sqlite" => DatabaseType.Sqlite,
        "Microsoft.EntityFrameworkCore.SqlServer" => DatabaseType.SqlServer,
        "Npgsql.EntityFrameworkCore.PostgreSQL" => DatabaseType.PostgreSql,
        _ => throw new UnsupportedDatabaseType(providerName!)
    };
}