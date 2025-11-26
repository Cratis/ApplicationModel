// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.Authorization.for_AllowAnonymousExtensions.when_checking_if_anonymous_is_allowed;

public class with_method_without_attributes_on_type_without_attributes : Specification
{
    bool _result;
    MethodInfo _method;

    void Establish() => _method = typeof(given.TypeWithoutAttributes).GetMethod(nameof(given.TypeWithoutAttributes.Method))!;

    void Because() => _result = _method.IsAnonymousAllowed();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
