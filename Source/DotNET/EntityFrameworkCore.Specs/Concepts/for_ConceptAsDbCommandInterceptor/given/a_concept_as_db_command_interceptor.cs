// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.EntityFrameworkCore.Concepts.for_ConceptAsDbCommandInterceptor.given;

public class a_concept_as_db_command_interceptor : Specification
{
    protected ConceptAsDbCommandInterceptor _interceptor;
    protected TestDbCommand _command;

    void Establish()
    {
        _interceptor = new ConceptAsDbCommandInterceptor();
        _command = new TestDbCommand();
    }
}
