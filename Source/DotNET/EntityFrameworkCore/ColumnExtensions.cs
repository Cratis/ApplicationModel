// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Common column types supported across different database providers.
/// </summary>
public static class ColumnExtensions
{
    /// <summary>
    /// Adds a string column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="maxLength">Maximum length of the string column. If null, uses unlimited length.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> StringColumn(this ColumnsBuilder cb, MigrationBuilder mb, int? maxLength = null, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => maxLength.HasValue ?
                cb.Column<string>($"VARCHAR({maxLength})", nullable: nullable) :
                cb.Column<string>("TEXT", nullable: nullable),
            DatabaseType.SqlServer => maxLength.HasValue ?
                cb.Column<string>($"NVARCHAR({maxLength})", nullable: nullable) :
                cb.Column<string>("NVARCHAR(MAX)", nullable: nullable),
            _ => cb.Column<string>("TEXT", nullable: nullable)
        };

    /// <summary>
    /// Adds an integer column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> IntColumn(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<int>("INTEGER", nullable: nullable),
            DatabaseType.SqlServer => cb.Column<int>("INT", nullable: nullable),
            _ => cb.Column<int>("INTEGER", nullable: nullable)
        };

    /// <summary>
    /// Adds a 64-bit integer column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> Int64Column(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<long>("BIGINT", nullable: nullable),
            DatabaseType.SqlServer => cb.Column<long>("BIGINT", nullable: nullable),
            _ => cb.Column<long>("INTEGER", nullable: nullable)
        };

    /// <summary>
    /// Adds a boolean column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> BoolColumn(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<bool>("BOOLEAN", nullable: nullable),
            DatabaseType.SqlServer => cb.Column<bool>("BIT", nullable: nullable),
            _ => cb.Column<bool>("INTEGER", nullable: nullable)
        };

    /// <summary>
    /// Adds a column that auto-increments.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> AutoIncrementColumn(this ColumnsBuilder cb, MigrationBuilder mb) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<int>("INTEGER", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            DatabaseType.SqlServer => cb.Column<int>("BIGINT", nullable: false)
                .Annotation("SqlServer:ValueGenerationStrategy", Microsoft.EntityFrameworkCore.Metadata.SqlServerValueGenerationStrategy.IdentityColumn),
            _ => cb.Column<int>("INTEGER", nullable: false)
                .Annotation("Sqlite:Autoincrement", true)
        };

    /// <summary>
    /// Adds a Guid column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
#pragma warning disable CA1720 // Identifier contains type name
    public static OperationBuilder<AddColumnOperation> GuidColumn(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<Guid>("UUID", nullable: nullable),
            DatabaseType.SqlServer => cb.Column<Guid>("UNIQUEIDENTIFIER", nullable: nullable),
            _ => cb.Column<Guid>("TEXT", nullable: nullable)
        };
#pragma warning restore CA1720 // Identifier contains type name

    /// <summary>
    /// Adds a DateTimeOffset column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> DateTimeOffsetColumn(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<DateTimeOffset>("TIMESTAMPTZ", nullable: nullable),
            DatabaseType.SqlServer => cb.Column<DateTimeOffset>("DATETIMEOFFSET", nullable: nullable),
            _ => cb.Column<DateTimeOffset>("TEXT", nullable: nullable)
        };
}
