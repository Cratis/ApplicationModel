// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsEvaluatableExpressionFilter.given;

public class an_evaluatable_expression_filter : Specification
{
    protected ConceptAsEvaluatableExpressionFilter _filter;
    protected IModel _model;

    void Establish()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(":memory:")
            .Options;

        using var context = new TestDbContext(options);
        _model = context.Model;

        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlite();
        var serviceProvider = services.BuildServiceProvider();

        var dependencies = serviceProvider.GetRequiredService<EvaluatableExpressionFilterDependencies>();
        var relationalDependencies = serviceProvider.GetRequiredService<RelationalEvaluatableExpressionFilterDependencies>();

        _filter = new ConceptAsEvaluatableExpressionFilter(dependencies, relationalDependencies);
    }
}
