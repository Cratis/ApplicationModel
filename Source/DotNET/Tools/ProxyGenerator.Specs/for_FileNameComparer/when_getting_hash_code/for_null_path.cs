// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.for_FileNameComparer.when_getting_hash_code;

public class for_null_path : given.a_file_name_comparer
{
    Exception? _exception;

    void Because() => _exception = Catch.Exception(() => _comparer.GetHashCode(null!));

    [Fact] void should_throw_argument_null_exception() => _exception.ShouldBeOfExactType<ArgumentNullException>();
}
