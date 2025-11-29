// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_StringExtensions;

public class when_converting_string_starting_with_two_uppercase_letters_to_camel_case : Specification
{
    string _result;

    void Because() => _result = "XMLParser".ToCamelCase();

    [Fact] void should_return_string_unchanged() => _result.ShouldEqual("XMLParser");
}
