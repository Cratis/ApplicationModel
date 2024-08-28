// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;

namespace Cratis.Applications.Authorization;

/// <summary>
/// Represents an <see cref="AuthorizeAttribute"/> for specifying roles.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RolesAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolesAttribute"/> class.
    /// </summary>
    /// <param name="roles">Roles that is needed.</param>
    public RolesAttribute(params string[] roles)
    {
        Roles = string.Join(',', roles);
    }
}