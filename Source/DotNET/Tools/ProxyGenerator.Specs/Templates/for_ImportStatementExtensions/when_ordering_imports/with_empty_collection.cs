// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Templates.for_ImportStatementExtensions.when_ordering_imports;

public class with_empty_collection : Specification
{
    List<ImportStatement> _imports;
    IOrderedEnumerable<ImportStatement> _result;

    void Establish() => _imports = [];

    void Because() => _result = _imports.ToOrderedImports();

    [Fact] void should_return_empty_collection() => _result.ShouldBeEmpty();
}
