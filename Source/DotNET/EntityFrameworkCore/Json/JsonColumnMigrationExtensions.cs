// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;

namespace Cratis.Applications.EntityFrameworkCore.Json;

/// <summary>
/// Extension methods for working with Json columns.
/// </summary>
public static class JsonColumnMigrationExtensions
{
    /// <summary>
    /// The annotation used to store the column type information.
    /// </summary>
    public const string CratisColumnTypeAnnotation = "cratis:ColumnType";

    /// <summary>
    /// The column type used to indicate a JSON column.
    /// </summary>
    public const string JsonColumnType = "json";

    /// <summary>
    /// Adds a Json column to the specified table.
    /// </summary>
    /// <param name="cb">Columns builder.</param>
    /// <param name="mb">Migration builder.</param>
    /// <typeparam name="TProperty">Type of property.</typeparam>
    /// <returns>Operation builder for the column.</returns>
    public static OperationBuilder<AddColumnOperation> JsonColumn<TProperty>(this ColumnsBuilder cb, MigrationBuilder mb) =>
        (mb.ActiveProvider switch
        {
            "Npgsql.EntityFrameworkCore.PostgreSQL" => cb.Column<TProperty>("jsonb", nullable: false),
            "Microsoft.EntityFrameworkCore.SqlServer" => cb.Column<TProperty>("nvarchar(max)", nullable: false),
            _ => cb.Column<TProperty>("text", nullable: false)
        }).Annotation("cratis:ColumnType", JsonColumnType);

    /// <summary>
    /// Checks if the column is a JSON column.
    /// </summary>
    /// <param name="column">The column to check.</param>
    /// <returns>True if the column is a JSON column.</returns>
    public static bool IsJson(this AddColumnOperation column) => column[CratisColumnTypeAnnotation]?.ToString() == JsonColumnType;
}
