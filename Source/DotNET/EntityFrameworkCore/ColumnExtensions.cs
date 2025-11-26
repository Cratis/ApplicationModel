// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Numerics;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Cratis.Arc.EntityFrameworkCore;

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
    /// <param name="defaultValue">Optional default value for the column.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> StringColumn(this ColumnsBuilder cb, MigrationBuilder mb, int? maxLength = null, bool nullable = true, string? defaultValue = null) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => maxLength.HasValue ?
                cb.Column<string>($"VARCHAR({maxLength})", nullable: nullable, defaultValue: defaultValue) :
                cb.Column<string>("TEXT", nullable: nullable, defaultValue: defaultValue),
            DatabaseType.SqlServer => maxLength.HasValue ?
                cb.Column<string>($"NVARCHAR({maxLength})", nullable: nullable, defaultValue: defaultValue) :
                cb.Column<string>("NVARCHAR(MAX)", nullable: nullable, defaultValue: defaultValue),
            _ => cb.Column<string>("TEXT", nullable: nullable, defaultValue: defaultValue)
        };

    /// <summary>
    /// Adds a number column with appropriate database-specific type.
    /// </summary>
    /// <typeparam name="T">The numeric type (char, byte, sbyte, short, ushort, int, uint, long, ulong, float, double, decimal).</typeparam>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <param name="defaultValue">Optional default value for the column.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> NumberColumn<T>(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true, object? defaultValue = null)
        where T : INumber<T>
    {
        var sqlType = GetSqlTypeForNumber<T>(mb.GetDatabaseType());
        return cb.Column<T>(sqlType, nullable: nullable, defaultValue: defaultValue);
    }

    /// <summary>
    /// Adds a boolean column with appropriate database-specific type.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <param name="nullable">Whether the column should be nullable.</param>
    /// <param name="defaultValue">Optional default value for the column.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> BoolColumn(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true, bool defaultValue = false) =>
        mb.GetDatabaseType() switch
        {
            DatabaseType.PostgreSql => cb.Column<bool>("BOOLEAN", nullable: nullable, defaultValue: defaultValue),
            DatabaseType.SqlServer => cb.Column<bool>("BIT", nullable: nullable, defaultValue: defaultValue),
            _ => cb.Column<bool>("INTEGER", nullable: nullable, defaultValue: defaultValue)
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
            _ => cb.Column<Guid>("BLOB", nullable: nullable)
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

    /// <summary>
    /// Gets the appropriate SQL type for a numeric type based on the database provider.
    /// </summary>
    /// <typeparam name="T">The numeric type.</typeparam>
    /// <param name="databaseType">The database type.</param>
    /// <returns>The SQL type string.</returns>
    static string GetSqlTypeForNumber<T>(DatabaseType databaseType)
        where T : INumber<T>
    {
        var type = typeof(T);

        return databaseType switch
        {
            DatabaseType.PostgreSql => type.Name switch
            {
                nameof(Char) => "SMALLINT",
                nameof(Byte) => "SMALLINT",
                nameof(SByte) => "SMALLINT",
                nameof(Int16) => "SMALLINT",
                nameof(UInt16) => "INTEGER",
                nameof(Int32) => "INTEGER",
                nameof(UInt32) => "BIGINT",
                nameof(Int64) => "BIGINT",
                nameof(UInt64) => "NUMERIC(20,0)",
                nameof(Single) => "REAL",
                nameof(Double) => "DOUBLE PRECISION",
                nameof(Decimal) => "DECIMAL",
                _ => "INTEGER"
            },
            DatabaseType.SqlServer => type.Name switch
            {
                nameof(Char) => "SMALLINT",
                nameof(Byte) => "TINYINT",
                nameof(SByte) => "SMALLINT",
                nameof(Int16) => "SMALLINT",
                nameof(UInt16) => "INT",
                nameof(Int32) => "INT",
                nameof(UInt32) => "BIGINT",
                nameof(Int64) => "BIGINT",
                nameof(UInt64) => "DECIMAL(20,0)",
                nameof(Single) => "REAL",
                nameof(Double) => "FLOAT",
                nameof(Decimal) => "DECIMAL(18,2)",
                _ => "INT"
            },
            _ => type.Name switch // SQLite and others
            {
                nameof(Single) => "REAL",
                nameof(Double) => "REAL",
                nameof(Decimal) => "REAL",
                _ => "INTEGER"
            }
        };
    }
}
