// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_FileNameComparer.when_comparing;

public class two_paths_with_different_filenames : given.a_file_name_comparer
{
    bool _result;

    void Because() => _result = _comparer.Equals("/path/to/file1.txt", "/other/path/file2.txt");

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
