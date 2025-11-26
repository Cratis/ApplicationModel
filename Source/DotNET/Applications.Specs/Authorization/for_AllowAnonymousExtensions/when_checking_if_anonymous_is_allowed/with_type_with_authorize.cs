// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Authorization.for_AllowAnonymousExtensions.when_checking_if_anonymous_is_allowed;

public class with_type_with_authorize : Specification
{
    bool _result;

    void Because() => _result = typeof(given.TypeWithAuthorize).IsAnonymousAllowed();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
