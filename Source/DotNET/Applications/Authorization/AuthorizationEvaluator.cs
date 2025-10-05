// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Authorization;

/// <summary>
/// Helper class for performing authorization checks.
/// </summary>
/// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HTTP context.</param>
public class AuthorizationEvaluator(IHttpContextAccessor httpContextAccessor) : IAuthorizationEvaluator
{
    /// <inheritdoc/>
    public bool IsAuthorized(Type type)
    {
        var authorizeAttribute = type.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        if (authorizeAttribute is null)
        {
            return true;
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User is null)
        {
            return false;
        }

        var user = httpContext.User;

        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        // Check roles if specified
        if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
        {
            var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim());
            var userHasRequiredRole = requiredRoles.Any(user.IsInRole);

            if (!userHasRequiredRole)
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public bool IsAuthorized(MethodInfo method)
    {
        var authorizeAttribute = method.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        if (authorizeAttribute is null)
        {
            return true;
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User is null)
        {
            return false;
        }

        var user = httpContext.User;

        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return false;
        }

        // Check roles if specified
        if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
        {
            var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim());
            var userHasRequiredRole = requiredRoles.Any(user.IsInRole);

            if (!userHasRequiredRole)
            {
                return false;
            }
        }

        return true;
    }
}