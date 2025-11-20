// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor.when_executing_reader;

public class with_concept_parameters : given.a_concept_as_db_command_interceptor
{
    TestStringConcept _stringConcept;
    TestIntConcept _intConcept;
    TestDbParameter _stringParam;
    TestDbParameter _intParam;

    void Establish()
    {
        _stringConcept = new TestStringConcept("test value");
        _intConcept = new TestIntConcept(42);

        _stringParam = new TestDbParameter { ParameterName = "@p1", Value = _stringConcept };
        _intParam = new TestDbParameter { ParameterName = "@p2", Value = _intConcept };

        _command.Parameters.Add(_stringParam);
        _command.Parameters.Add(_intParam);
    }

    void Because()
    {
        // Directly test the unwrapping logic without needing EventDefinition
        _interceptor.ReaderExecuting(_command, default!, default);
    }

    [Fact] void should_unwrap_string_concept_to_underlying_value() => _stringParam.Value.ShouldEqual("test value");
    [Fact] void should_unwrap_int_concept_to_underlying_value() => _intParam.Value.ShouldEqual(42);
}
