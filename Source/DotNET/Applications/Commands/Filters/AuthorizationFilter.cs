// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Authorization;

namespace Cratis.Applications.Commands.Filters;

/// <summary>
/// Represents a command filter that authorizes commands before they are handled.
/// </summary>
/// <param name="authorizationHelper">The <see cref="IAuthorizationHelper"/> to use for authorization checks.</param>
public class AuthorizationFilter(IAuthorizationHelper authorizationHelper) : ICommandFilter
{
    /// <inheritdoc/>
    public Task<CommandResult> OnExecution(CommandContext context)
    {
        if (authorizationHelper.IsAuthorized(context.Type))
        {
            return Task.FromResult(CommandResult.Success(context.CorrelationId));
        }

        return Task.FromResult(CommandResult.Unauthorized(context.CorrelationId));
    }
}