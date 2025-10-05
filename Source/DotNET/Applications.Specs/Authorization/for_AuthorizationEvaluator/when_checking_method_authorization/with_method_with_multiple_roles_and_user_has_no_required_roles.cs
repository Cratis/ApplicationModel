// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.Authorization.for_AuthorizationEvaluator.when_checking_method_authorization;

public class with_method_with_multiple_roles_and_user_has_no_required_roles : given.an_authorization_helper
{
    bool _result;
    MethodInfo _method;

    void Establish()
    {
        _method = typeof(given.TypeWithMethodMultipleRoles).GetMethod(nameof(given.TypeWithMethodMultipleRoles.MethodWithMultipleRoles))!;
        SetupAuthenticatedUser("Guest");
    }

    void Because() => _result = _authorizationHelper.IsAuthorized(_method);

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}