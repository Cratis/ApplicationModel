// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsExpressionRewriter.when_rewriting;

public class with_binary_comparison : Specification
{
    Expression _expression;
    Expression _result;

    void Establish()
    {
        // Create expression: entity.Id == closure.Id (both concepts)
        // The rewriter should transform this to: entity.Id.Value == closure.Id.Value
        var entity = new TestEntity { Id = new TestIdConcept(Guid.NewGuid()) };
        var closure = new TestClosure { Id = new TestIdConcept(Guid.NewGuid()) };

        var entityParameter = Expression.Parameter(typeof(TestEntity), "e");
        var entityIdAccess = Expression.Property(entityParameter, nameof(TestEntity.Id));
        
        var closureConstant = Expression.Constant(closure);
        var closureIdAccess = Expression.Property(closureConstant, nameof(TestClosure.Id));

        _expression = Expression.Equal(entityIdAccess, closureIdAccess);
    }

    void Because() => _result = ConceptAsExpressionRewriter.Rewrite(_expression);

    [Fact] void should_return_binary_expression() => (_result is BinaryExpression).ShouldBeTrue();
    [Fact] void should_rewrite_left_side_to_access_value() => (((BinaryExpression)_result).Left is MemberExpression).ShouldBeTrue();
    [Fact] void should_have_primitive_comparison() => ((MemberExpression)((BinaryExpression)_result).Left).Type.ShouldEqual(typeof(Guid));
}
