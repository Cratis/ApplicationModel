// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="WebApplicationBuilder"/> for adding the correlation Id log enricher.
/// </summary>
public static class CorrelationIdWebApplicationExtensions
{
    /// <summary>
    /// Adds the correlation ID enricher to the logging configuration.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The web application builder for chaining.</returns>
    public static WebApplicationBuilder AddCorrelationIdLogEnricher(this WebApplicationBuilder builder)
    {
        builder.Logging.EnableEnrichment();
        builder.Services.AddLogEnricher<CorrelationIdLogEnricher>();
        return builder;
    }
}