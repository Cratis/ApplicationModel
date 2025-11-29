// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_StringExtensions;

public class when_converting_null_string_to_camel_case : Specification
{
    string? _result;

    void Because() => _result = ((string)null!).ToCamelCase();

    [Fact] void should_return_null() => _result.ShouldBeNull();
}
