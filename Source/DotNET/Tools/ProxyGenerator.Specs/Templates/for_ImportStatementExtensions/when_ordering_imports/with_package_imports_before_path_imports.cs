// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Templates.for_ImportStatementExtensions.when_ordering_imports;

public class with_package_imports_before_path_imports : Specification
{
    List<ImportStatement> _imports;
    IOrderedEnumerable<ImportStatement> _result;

    void Establish() => _imports =
    [
        new ImportStatement(typeof(string), "LocalType", "./local/path"),
        new ImportStatement(typeof(string), "PackageType", "@some/package")
    ];

    void Because() => _result = _imports.ToOrderedImports();

    [Fact] void should_put_package_import_first() => _result.First().Module.ShouldEqual("@some/package");
    [Fact] void should_put_path_import_last() => _result.Last().Module.ShouldEqual("./local/path");
}
