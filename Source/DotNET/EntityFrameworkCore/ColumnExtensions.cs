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
}
