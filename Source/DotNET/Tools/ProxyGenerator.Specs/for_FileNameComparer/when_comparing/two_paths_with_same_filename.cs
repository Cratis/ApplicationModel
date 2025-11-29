// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.for_FileNameComparer.when_comparing;

public class two_paths_with_same_filename : given.a_file_name_comparer
{
    bool _result;

    void Because() => _result = _comparer.Equals("/path/to/file.txt", "/other/path/file.txt");

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
