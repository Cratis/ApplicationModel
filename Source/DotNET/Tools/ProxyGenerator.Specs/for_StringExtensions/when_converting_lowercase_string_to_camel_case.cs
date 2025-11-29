// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.for_StringExtensions;

public class when_converting_lowercase_string_to_camel_case : Specification
{
    string _result;

    void Because() => _result = "hello".ToCamelCase();

    [Fact] void should_return_string_unchanged() => _result.ShouldEqual("hello");
}
