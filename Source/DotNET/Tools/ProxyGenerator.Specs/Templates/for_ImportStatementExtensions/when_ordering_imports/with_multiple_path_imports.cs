// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Templates.for_ImportStatementExtensions.when_ordering_imports;

public class with_multiple_path_imports : Specification
{
    List<ImportStatement> _imports;
    IOrderedEnumerable<ImportStatement> _result;

    void Establish() => _imports =
    [
        new ImportStatement(typeof(string), "TypeC", "./path/typeC"),
        new ImportStatement(typeof(string), "TypeA", "./path/typeA"),
        new ImportStatement(typeof(string), "TypeB", "../other/typeB")
    ];

    void Because() => _result = _imports.ToOrderedImports();

    [Fact] void should_order_by_filename_alphabetically() => _result.Select(_ => _.Type).ShouldContainOnly("TypeA", "TypeB", "TypeC");
}
