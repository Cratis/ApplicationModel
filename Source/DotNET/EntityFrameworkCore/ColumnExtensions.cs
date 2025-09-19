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
    /// Adds a column that auto-increments.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> AutoIncrement(this ColumnsBuilder cb, MigrationBuilder mb) =>
        mb.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => cb.Column<int>("INTEGER", nullable: false)
                .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            "Microsoft.EntityFrameworkCore.SqlServer" => cb.Column<int>("BIGINT", nullable: false)
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
    public static OperationBuilder<AddColumnOperation> Guid(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => cb.Column<Guid>("UUID", nullable: nullable),
            "Microsoft.EntityFrameworkCore.SqlServer" => cb.Column<Guid>("UNIQUEIDENTIFIER", nullable: nullable),
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
    public static OperationBuilder<AddColumnOperation> DateTimeOffset(this ColumnsBuilder cb, MigrationBuilder mb, bool nullable = true) =>
        mb.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => cb.Column<DateTimeOffset>("TIMESTAMPTZ", nullable: nullable),
            "Microsoft.EntityFrameworkCore.SqlServer" => cb.Column<DateTimeOffset>("DATETIMEOFFSET", nullable: nullable),
            _ => cb.Column<DateTimeOffset>("TEXT", nullable: nullable)
        };
}
