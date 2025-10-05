// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Commands.Filters;

/// <summary>
/// Represents a command filter that authorizes commands before they are handled.
/// </summary>
/// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HTTP context.</param>
public class AuthorizationFilter(IHttpContextAccessor httpContextAccessor) : ICommandFilter
{
    /// <inheritdoc/>
    public Task<CommandResult> OnExecution(CommandContext context)
    {
        var authorizeAttribute = context.Type.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        if (authorizeAttribute is null)
        {
            return Task.FromResult(CommandResult.Success(context.CorrelationId));
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User is null)
        {
            return Task.FromResult(CommandResult.Unauthorized(context.CorrelationId));
        }

        var user = httpContext.User;

        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return Task.FromResult(CommandResult.Unauthorized(context.CorrelationId));
        }

        // Check roles if specified
        if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
        {
            var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim());
            var userHasRequiredRole = requiredRoles.Any(user.IsInRole);

            if (!userHasRequiredRole)
            {
                return Task.FromResult(CommandResult.Unauthorized(context.CorrelationId));
            }
        }

        return Task.FromResult(CommandResult.Success(context.CorrelationId));
    }
}