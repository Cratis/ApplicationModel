// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.for_TargetTypeExtensions.when_trying_to_get_import_statement;

public class with_target_type_having_empty_module : Specification
{
    TargetType _targetType;
    ImportStatement? _result;
    bool _success;

    void Establish() => _targetType = new TargetType(typeof(string), "string", "String", string.Empty);

    void Because() => _success = _targetType.TryGetImportStatement(out _result);

    [Fact] void should_return_false() => _success.ShouldBeFalse();
    [Fact] void should_return_null_import_statement() => _result.ShouldBeNull();
}
