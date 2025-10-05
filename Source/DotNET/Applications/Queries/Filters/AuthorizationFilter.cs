// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Queries.Filters;

/// <summary>
/// Represents a query filter that authorizes queries before they are performed.
/// </summary>
/// <param name="httpContextAccessor">The <see cref="IHttpContextAccessor"/> to access the current HTTP context.</param>
/// <param name="queryPerformerProviders">The <see cref="IQueryPerformerProviders"/> to use for finding query performers.</param>
public class AuthorizationFilter(IHttpContextAccessor httpContextAccessor, IQueryPerformerProviders queryPerformerProviders) : IQueryFilter
{
    /// <inheritdoc/>
    public Task<QueryResult> OnPerform(QueryContext context)
    {
        // Get the query performer to understand the query type
        if (!queryPerformerProviders.TryGetPerformersFor(context.Name, out var performer))
        {
            // No performer found, authorization cannot be determined
            return Task.FromResult(QueryResult.Success(context.CorrelationId));
        }

        // Check for Authorize attribute on the query performer type (includes RolesAttribute)
        var authorizeAttribute = performer.Type.GetCustomAttributes(typeof(AuthorizeAttribute), inherit: true)
            .OfType<AuthorizeAttribute>()
            .FirstOrDefault();

        if (authorizeAttribute is null)
        {
            return Task.FromResult(QueryResult.Success(context.CorrelationId));
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext?.User is null)
        {
            return Task.FromResult(QueryResult.Unauthorized(context.CorrelationId));
        }

        var user = httpContext.User;

        // Check if user is authenticated
        if (!user.Identity?.IsAuthenticated ?? true)
        {
            return Task.FromResult(QueryResult.Unauthorized(context.CorrelationId));
        }

        // Check roles if specified
        if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
        {
            var requiredRoles = authorizeAttribute.Roles.Split(',').Select(r => r.Trim());
            var userHasRequiredRole = requiredRoles.Any(user.IsInRole);

            if (!userHasRequiredRole)
            {
                return Task.FromResult(QueryResult.Unauthorized(context.CorrelationId));
            }
        }

        return Task.FromResult(QueryResult.Success(context.CorrelationId));
    }
}