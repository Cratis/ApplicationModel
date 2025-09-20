// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Sqlite specific migrations SQL generator.
/// </summary>
/// <param name="dependencies">The dependencies.</param>
/// <param name="migrationsAnnotations">The migrations annotations.</param>
public class MigrationsSqlGeneratorForSqlite(
        MigrationsSqlGeneratorDependencies dependencies,
        IRelationalAnnotationProvider migrationsAnnotations) : SqliteMigrationsSqlGenerator(dependencies, migrationsAnnotations)
{
    /// <inheritdoc/>
    protected override void ColumnDefinition(string? schema, string table, string name, ColumnOperation operation, IModel? model, MigrationCommandListBuilder builder)
    {
        base.ColumnDefinition(schema, table, name, operation, model, builder);

        // Append the JSON validation for columns marked as json
        if (operation is AddColumnOperation add && add.IsJson())
        {
            var h = Dependencies.SqlGenerationHelper;
            var col = h.DelimitIdentifier(name);
            builder.Append($" CHECK (json_valid({col}))");
        }
    }
}
