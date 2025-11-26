// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;

namespace Cratis.Applications.Authorization.for_AuthorizationEvaluator.given;

public class TypeWithMethodAuthorization
{
    [Authorize]
    public void MethodWithAuthorization() { }

    public void MethodWithoutAuthorization() { }
}