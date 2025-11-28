// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.Authorization.for_AllowAnonymousExtensions.when_checking_if_anonymous_is_allowed;

public class with_type_with_allow_anonymous_and_authorize : Specification
{
    Exception _exception;

    void Because() => _exception = Catch.Exception(() => typeof(given.TypeWithAllowAnonymousAndAuthorize).IsAnonymousAllowed());

    [Fact] void should_throw_ambiguous_authorization_level() => _exception.ShouldBeOfExactType<AmbiguousAuthorizationLevel>();
}
