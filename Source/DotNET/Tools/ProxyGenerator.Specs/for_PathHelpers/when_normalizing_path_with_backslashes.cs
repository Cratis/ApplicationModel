// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_PathHelpers;

public class when_normalizing_path_with_backslashes : Specification
{
    string _result;
    string _input;

    void Establish() => _input = "some\\path\\to\\file";

    void Because() => _result = PathHelpers.Normalize(_input);

    [Fact] void should_return_full_path() => _result.ShouldNotBeEmpty();
    [Fact] void should_contain_normalized_separators() => _result.ShouldContain(Path.DirectorySeparatorChar.ToString());
}
