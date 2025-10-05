// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Authorization;

namespace Cratis.Applications.Queries.Filters;

/// <summary>
/// Represents a query filter that authorizes queries before they are performed.
/// </summary>
/// <param name="authorizationHelper">The <see cref="IAuthorizationHelper"/> to use for authorization checks.</param>
/// <param name="queryPerformerProviders">The <see cref="IQueryPerformerProviders"/> to use for finding query performers.</param>
public class AuthorizationFilter(IAuthorizationHelper authorizationHelper, IQueryPerformerProviders queryPerformerProviders) : IQueryFilter
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

        if (authorizationHelper.IsAuthorized(performer.Type))
        {
            return Task.FromResult(QueryResult.Success(context.CorrelationId));
        }

        return Task.FromResult(QueryResult.Unauthorized(context.CorrelationId));
    }
}