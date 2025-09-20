// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Json;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Update;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// SQL Server specific migrations SQL generator.
/// </summary>
/// <param name="dependencies">The dependencies.</param>
/// <param name="commandBatchPreparer">The command batch preparer.</param>
public class MigrationsSqlGeneratorForSqlServer(
        MigrationsSqlGeneratorDependencies dependencies,
        ICommandBatchPreparer commandBatchPreparer) : SqlServerMigrationsSqlGenerator(dependencies, commandBatchPreparer)
{
    /// <inheritdoc/>
    protected override void Generate(AddColumnOperation operation, IModel? model, MigrationCommandListBuilder builder, bool terminate)
    {
        base.Generate(operation, model, builder, terminate);
        if (!operation.IsJson()) return;

        var helper = Dependencies.SqlGenerationHelper;
        var table = helper.DelimitIdentifier(operation.Table, operation.Schema);
        var col = helper.DelimitIdentifier(operation.Name);
        var ck = helper.DelimitIdentifier($"CK_{operation.Table}_{operation.Name}_IsJson");
        builder
            .Append($"ALTER TABLE {table} ADD CONSTRAINT {ck} CHECK (ISJSON({col}) = 1)")
            .AppendLine(helper.StatementTerminator)
            .EndCommand();
    }
}
