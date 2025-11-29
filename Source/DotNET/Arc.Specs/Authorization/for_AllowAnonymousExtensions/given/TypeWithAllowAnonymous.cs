// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;

namespace Cratis.Arc.Authorization.for_AllowAnonymousExtensions.given;

[AllowAnonymous]
public static class TypeWithAllowAnonymous
{
    public static void Method()
    {
    }
}
