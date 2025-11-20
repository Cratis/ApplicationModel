// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor.when_executing_scalar;

public class with_concept_parameters : given.a_concept_as_db_command_interceptor
{
    TestStringConcept _stringConcept;
    TestDbParameter _stringParam;

    void Establish()
    {
        _stringConcept = new TestStringConcept("test value");
        _stringParam = new TestDbParameter { ParameterName = "@p1", Value = _stringConcept };
        _command.Parameters.Add(_stringParam);
    }

    void Because()
    {
        // Directly test the unwrapping logic without needing EventDefinition
        _interceptor.ScalarExecuting(_command, default!, default);
    }

    [Fact] void should_unwrap_concept_to_underlying_value() => _stringParam.Value.ShouldEqual("test value");
}
