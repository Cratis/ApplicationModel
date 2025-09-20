// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// PostgreSQL specific migrations SQL generator.
/// </summary>
/// <param name="dependencies">The dependencies.</param>
/// <param name="npgsqlSingletonOptions">The Npgsql singleton options.</param>
public class MigrationsSqlGeneratorForPostgreSQL(
        MigrationsSqlGeneratorDependencies dependencies,
#pragma warning disable EF1001 // Internal EF Core API usage.
        INpgsqlSingletonOptions npgsqlSingletonOptions) : NpgsqlMigrationsSqlGenerator(dependencies, npgsqlSingletonOptions)
#pragma warning restore EF1001 // Internal EF Core API usage.
{
    /// <inheritdoc/>
    protected override void Generate(AddColumnOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate = true)
    {
        base.Generate(operation, model, builder, terminate);
        if (!operation.IsJson()) return;

        var helper = Dependencies.SqlGenerationHelper;
        var table = helper.DelimitIdentifier(operation.Table, operation.Schema);
        var column = helper.DelimitIdentifier(operation.Name);
        var constraintCheck = helper.DelimitIdentifier($"CK_{operation.Table}_{operation.Name}_IsJson");
        builder
            .Append($"ALTER TABLE {table} ADD CONSTRAINT {constraintCheck} CHECK ({column} IS NULL OR jsonb_typeof(({column})::jsonb) IS NOT NULL)")
            .Append($"CREATE INDEX IX_{operation.Table}_{operation.Name}_Jsonb ON {table} USING GIN ({column} jsonb_path_ops)")
            .AppendLine(helper.StatementTerminator)
            .EndCommand();
    }
}