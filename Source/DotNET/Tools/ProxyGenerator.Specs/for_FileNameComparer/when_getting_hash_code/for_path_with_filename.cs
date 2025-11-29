// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_FileNameComparer.when_getting_hash_code;

public class for_path_with_filename : given.a_file_name_comparer
{
    int _result;

    void Because() => _result = _comparer.GetHashCode("/path/to/file.txt");

    [Fact] void should_return_hash_of_filename() => _result.ShouldEqual(StringComparer.OrdinalIgnoreCase.GetHashCode("file.txt"));
}
