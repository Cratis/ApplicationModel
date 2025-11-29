// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.ModelBound.for_TypeExtensionsModelBound.when_checking_is_command;

public class with_command_without_handle_method : Specification
{
    bool _result;

    void Because() => _result = typeof(CommandWithoutHandleMethod).IsCommand();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
