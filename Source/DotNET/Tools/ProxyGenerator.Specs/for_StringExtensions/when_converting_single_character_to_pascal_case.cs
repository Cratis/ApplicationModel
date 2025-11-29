// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_StringExtensions;

public class when_converting_single_character_to_pascal_case : Specification
{
    string _result;

    void Because() => _result = "a".ToPascalCase();

    [Fact] void should_return_upper_case_character() => _result.ShouldEqual("A");
}
