// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsParameterEvaluator.when_evaluating;

public class with_concept_member_access_on_closure : Specification
{
    TestClosure _closure;
    Expression _expression;
    Expression _result;
    Guid _expectedId;

    void Establish()
    {
        _expectedId = Guid.NewGuid();
        _closure = new TestClosure { Id = new TestIdConcept(_expectedId) };

        // Create member access on constant: closure.Id
        var closureConstant = Expression.Constant(_closure);
        _expression = Expression.Property(closureConstant, nameof(TestClosure.Id));
    }

    void Because() => _result = ConceptAsParameterEvaluator.Evaluate(_expression);

    [Fact] void should_evaluate_to_constant() => _result.ShouldBeOfExactType<ConstantExpression>();
    [Fact] void should_have_primitive_type() => _result.Type.ShouldEqual(typeof(Guid));
    [Fact] void should_have_unwrapped_value() => ((ConstantExpression)_result).Value.ShouldEqual(_expectedId);
}
