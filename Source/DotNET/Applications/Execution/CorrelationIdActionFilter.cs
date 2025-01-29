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
public class CorrelationIdActionFilter(IOptions<ApplicationModelOptions> options) : IAsyncActionFilter
{
    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var correlationIdAsString = context.HttpContext.Request.Headers[options.Value.CorrelationId.HttpHeader].ToString() ?? Guid.Empty.ToString();
        if (string.IsNullOrEmpty(correlationIdAsString))
        {
            correlationIdAsString = CorrelationId.New().ToString();
            context.HttpContext.Request.Headers[Constants.DefaultCorrelationIdHeader] = correlationIdAsString;
        }

        var correlationId = Guid.Parse(correlationIdAsString);
        context.HttpContext.Items[Constants.CorrelationIdItemKey] = correlationId;
        CorrelationIdAccessor.SetCurrent(correlationId);

        await next();
    }
}
