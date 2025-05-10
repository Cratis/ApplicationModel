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
        var correlationIdAsString = context.HttpContext.Request.Headers[options.Value.CorrelationId.HttpHeader].ToString() ?? Guid.Empty.ToString();
        CorrelationId correlationId;
        var setCurrent = false;
        if (string.IsNullOrEmpty(correlationIdAsString) ||
            correlationIdAsString == Guid.Empty.ToString() ||
            !Guid.TryParse(correlationIdAsString, out _))
        {
            correlationId = correlationIdAccessor.Current;
            if (correlationId == CorrelationId.NotSet)
            {
                correlationId = CorrelationId.New();
                setCurrent = true;
            }

            correlationIdAsString = correlationId.ToString();
            context.HttpContext.Request.Headers[Constants.DefaultCorrelationIdHeader] = correlationIdAsString;
        }
        else
        {
            setCurrent = true;
            correlationId = Guid.Parse(correlationIdAsString);
        }

        context.HttpContext.Items[Constants.CorrelationIdItemKey] = correlationId;

        if (setCurrent)
        {
            if (correlationIdAccessor is ICorrelationIdModifier correlationIdModifier)
            {
                correlationIdModifier.Modify(correlationId);
            }
            else
            {
                CorrelationIdAccessor.SetCurrent(correlationId);
            }
        }

        await next();
    }
}
