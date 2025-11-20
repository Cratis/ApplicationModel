// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsExpressionRewriter.when_rewriting;

public class with_redundant_convert_expression : Specification
{
    Expression _expression;
    Expression _result;

    void Establish()
    {
        // Create expression: Convert(guid, Guid) - redundant conversion
        var guidValue = Expression.Constant(Guid.NewGuid(), typeof(Guid));
        _expression = Expression.Convert(guidValue, typeof(Guid));
    }

    void Because() => _result = ConceptAsExpressionRewriter.Rewrite(_expression);

    [Fact] void should_remove_redundant_convert() => _result.ShouldBeOfExactType<ConstantExpression>();
    [Fact] void should_have_primitive_type() => _result.Type.ShouldEqual(typeof(Guid));
}
