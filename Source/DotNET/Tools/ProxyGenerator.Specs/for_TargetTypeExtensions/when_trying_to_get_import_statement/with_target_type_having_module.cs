// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.for_TargetTypeExtensions.when_trying_to_get_import_statement;

public class with_target_type_having_module : Specification
{
    TargetType _targetType;
    ImportStatement? _result;
    bool _success;

    void Establish() => _targetType = new TargetType(typeof(string), "string", "String", "@some/module");

    void Because() => _success = _targetType.TryGetImportStatement(out _result);

    [Fact] void should_return_true() => _success.ShouldBeTrue();
    [Fact] void should_return_import_statement() => _result.ShouldNotBeNull();
    [Fact] void should_have_correct_type() => _result!.Type.ShouldEqual("string");
    [Fact] void should_have_correct_module() => _result!.Module.ShouldEqual("@some/module");
}
