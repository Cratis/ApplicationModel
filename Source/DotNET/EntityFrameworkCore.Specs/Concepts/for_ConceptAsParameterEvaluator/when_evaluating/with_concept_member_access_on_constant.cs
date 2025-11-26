// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsParameterEvaluator.when_evaluating;

public class with_concept_member_access_on_constant : Specification
{
    TestClosure _closure;
    Expression _expression;
    Expression _result;
    string _expectedName;

    void Establish()
    {
        _expectedName = "Test Name";
        _closure = new TestClosure { Name = new TestNameConcept(_expectedName) };

        // Create member access on constant: closure.Name
        var closureConstant = Expression.Constant(_closure);
        _expression = Expression.Property(closureConstant, nameof(TestClosure.Name));
    }

    void Because() => _result = ConceptAsParameterEvaluator.Evaluate(_expression);

    [Fact] void should_evaluate_to_constant() => _result.ShouldBeOfExactType<ConstantExpression>();
    [Fact] void should_have_primitive_type() => _result.Type.ShouldEqual(typeof(string));
    [Fact] void should_have_unwrapped_value() => ((ConstantExpression)_result).Value.ShouldEqual(_expectedName);
}
