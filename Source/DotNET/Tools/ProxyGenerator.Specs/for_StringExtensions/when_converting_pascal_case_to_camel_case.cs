// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.for_StringExtensions;

public class when_converting_pascal_case_to_camel_case : Specification
{
    string _result;

    void Because() => _result = "HelloWorld".ToCamelCase();

    [Fact] void should_lowercase_first_letter() => _result.ShouldEqual("helloWorld");
}
