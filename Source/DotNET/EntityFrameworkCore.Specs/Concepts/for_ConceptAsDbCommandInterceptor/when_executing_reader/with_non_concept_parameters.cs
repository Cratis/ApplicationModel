// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor.when_executing_reader;

public class with_non_concept_parameters : given.a_concept_as_db_command_interceptor
{
    TestDbParameter _stringParam;
    TestDbParameter _intParam;
    string _originalStringValue;
    int _originalIntValue;

    void Establish()
    {
        _originalStringValue = "test value";
        _originalIntValue = 42;

        _stringParam = new TestDbParameter { ParameterName = "@p1", Value = _originalStringValue };
        _intParam = new TestDbParameter { ParameterName = "@p2", Value = _originalIntValue };

        _command.Parameters.Add(_stringParam);
        _command.Parameters.Add(_intParam);
    }

    void Because()
    {
        // Directly test that non-concepts are not modified
        _interceptor.ReaderExecuting(_command, default!, default);
    }

    [Fact] void should_not_modify_string_parameter() => _stringParam.Value.ShouldEqual(_originalStringValue);
    [Fact] void should_not_modify_int_parameter() => _intParam.Value.ShouldEqual(_originalIntValue);
}
