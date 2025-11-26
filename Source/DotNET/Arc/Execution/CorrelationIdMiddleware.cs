// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cratis.Applications.Execution;

/// <summary>
/// Represents an implementation of <see cref="IMiddleware"/> that sets the correlation ID for the request.
/// </summary>
/// <param name="options">The options for the application model.</param>
/// <param name="correlationIdAccessor">The accessor for the correlation ID.</param>
public class CorrelationIdMiddleware(IOptions<ApplicationModelOptions> options, ICorrelationIdAccessor correlationIdAccessor) : IMiddleware
{
    /// <inheritdoc/>
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.HandleCorrelationId(correlationIdAccessor, options.Value.CorrelationId);
        await next(context);
    }
}