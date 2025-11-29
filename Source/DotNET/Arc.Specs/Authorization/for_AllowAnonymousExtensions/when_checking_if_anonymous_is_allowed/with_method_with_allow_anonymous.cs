// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.Authorization.for_AllowAnonymousExtensions.when_checking_if_anonymous_is_allowed;

public class with_method_with_allow_anonymous : Specification
{
    bool _result;
    MethodInfo _method;

    void Establish() => _method = typeof(given.TypeWithMethodAllowAnonymous).GetMethod(nameof(given.TypeWithMethodAllowAnonymous.Method))!;

    void Because() => _result = _method.IsAnonymousAllowed();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
