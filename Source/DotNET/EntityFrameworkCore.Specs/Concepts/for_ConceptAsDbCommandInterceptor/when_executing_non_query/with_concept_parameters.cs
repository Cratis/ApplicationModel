// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor.when_executing_non_query;

public class with_concept_parameters : given.a_concept_as_db_command_interceptor
{
    TestIntConcept _intConcept;
    TestDbParameter _intParam;

    void Establish()
    {
        _intConcept = new TestIntConcept(42);
        _intParam = new TestDbParameter { ParameterName = "@p1", Value = _intConcept };
        _command.Parameters.Add(_intParam);
    }

    void Because()
    {
        // Directly test the unwrapping logic without needing EventDefinition
        _interceptor.NonQueryExecuting(_command, default, default);
    }

    [Fact] void should_unwrap_concept_to_underlying_value() => _intParam.Value.ShouldEqual(42);
}
