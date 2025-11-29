// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.for_FileNameComparer.given;

public class a_file_name_comparer : Specification
{
    protected FileNameComparer _comparer;

    void Establish() => _comparer = new FileNameComparer();
}
