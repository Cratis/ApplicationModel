// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator.Templates.for_ImportStatementExtensions.when_ordering_imports;

public class with_only_package_imports : Specification
{
    List<ImportStatement> _imports;
    IOrderedEnumerable<ImportStatement> _result;

    void Establish() => _imports =
    [
        new ImportStatement(typeof(string), "TypeB", "@package/b"),
        new ImportStatement(typeof(string), "TypeA", "@package/a")
    ];

    void Because() => _result = _imports.ToOrderedImports();

    [Fact] void should_return_all_imports() => _result.Count().ShouldEqual(2);
}
