// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsExpressionRewriter.when_rewriting;

public class with_concept_property_access : Specification
{
    Expression _expression;
    Expression _result;

    void Establish()
    {
        // Create expression: entity.Id
        var entityParam = Expression.Parameter(typeof(TestEntity), "e");
        _expression = Expression.Property(entityParam, nameof(TestEntity.Id));
    }

    void Because() => _result = ConceptAsExpressionRewriter.Rewrite(_expression);

    [Fact] void should_be_member_expression() => (_result is MemberExpression).ShouldBeTrue();
    [Fact] void should_access_value_property() => ((MemberExpression)_result).Member.Name.ShouldEqual("Value");
    [Fact] void should_have_primitive_type() => _result.Type.ShouldEqual(typeof(Guid));
}
