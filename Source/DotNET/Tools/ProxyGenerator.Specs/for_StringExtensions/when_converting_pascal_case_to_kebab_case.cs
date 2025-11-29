// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_StringExtensions;

public class when_converting_pascal_case_to_kebab_case : Specification
{
    string _result;

    void Because() => _result = "HelloWorld".ToKebabCase();

    [Fact] void should_convert_to_kebab_case() => _result.ShouldEqual("hello-world");
}
