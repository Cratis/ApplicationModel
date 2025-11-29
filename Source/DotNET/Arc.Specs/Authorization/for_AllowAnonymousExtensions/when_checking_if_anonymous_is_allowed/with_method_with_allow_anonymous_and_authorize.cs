// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.Authorization.for_AllowAnonymousExtensions.when_checking_if_anonymous_is_allowed;

public class with_method_with_allow_anonymous_and_authorize : Specification
{
    Exception _exception;
    MethodInfo _method;

    void Establish() => _method = typeof(given.TypeWithMethodAllowAnonymousAndAuthorize).GetMethod(nameof(given.TypeWithMethodAllowAnonymousAndAuthorize.Method))!;

    void Because() => _exception = Catch.Exception(() => _method.IsAnonymousAllowed());

    [Fact] void should_throw_ambiguous_authorization_level() => _exception.ShouldBeOfExactType<AmbiguousAuthorizationLevel>();
}
