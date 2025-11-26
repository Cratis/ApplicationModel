// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace Cratis.Applications.Authorization;

/// <summary>
/// Extension methods for checking if anonymous access is allowed.
/// </summary>
public static class AllowAnonymousExtensions
{
    /// <summary>
    /// Checks if anonymous access is allowed for the specified member.
    /// </summary>
    /// <param name="member">The member to check for <see cref="AllowAnonymousAttribute"/>.</param>
    /// <returns>True if anonymous access is allowed, false otherwise.</returns>
    public static bool IsAnonymousAllowed(this MemberInfo member) =>
        member.GetCustomAttributes(typeof(AllowAnonymousAttribute), inherit: true).Length > 0;
}
