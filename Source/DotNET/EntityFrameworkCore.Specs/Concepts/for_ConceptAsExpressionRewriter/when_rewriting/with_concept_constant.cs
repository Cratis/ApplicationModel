// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsExpressionRewriter.when_rewriting;

public class with_concept_constant : Specification
{
    TestIdConcept _concept;
    Expression _expression;
    Expression _result;

    void Establish()
    {
        _concept = new TestIdConcept(Guid.NewGuid());
        _expression = Expression.Constant(_concept);
    }

    void Because() => _result = ConceptAsExpressionRewriter.Rewrite(_expression);

    [Fact] void should_unwrap_to_constant_with_primitive_type() => _result.ShouldBeOfExactType<ConstantExpression>();
    [Fact] void should_have_primitive_type() => _result.Type.ShouldEqual(typeof(Guid));
    [Fact] void should_have_unwrapped_value() => ((ConstantExpression)_result).Value.ShouldEqual(_concept.Value);
}
