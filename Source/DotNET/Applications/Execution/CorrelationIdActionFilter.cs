// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Execution;

/// <summary>
/// Represents an implementation of <see cref="IAsyncActionFilter"/> that sets the correlation ID for the request.
/// </summary>
/// <param name="options">The options for the correlation ID.</param>
/// <param name="correlationIdAccessor">The accessor for the correlation ID.</param>
public class CorrelationIdActionFilter(IOptions<ApplicationModelOptions> options, ICorrelationIdAccessor correlationIdAccessor) : IAsyncActionFilter
{
    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        CorrelationIdHelpers.Handle(correlationIdAccessor, options.Value.CorrelationId, context.HttpContext);
        await next();
    }
}
