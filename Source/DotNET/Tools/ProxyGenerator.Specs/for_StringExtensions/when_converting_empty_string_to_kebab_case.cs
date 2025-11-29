// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_StringExtensions;

public class when_converting_empty_string_to_kebab_case : Specification
{
    string _result;

    void Because() => _result = string.Empty.ToKebabCase();

    [Fact] void should_return_empty_string() => _result.ShouldEqual(string.Empty);
}
