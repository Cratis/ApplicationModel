// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Tenancy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Cratis.Applications;

/// <summary>
/// Represents an implementation of <see cref="IStartupFilter"/> that configures Application Model middleware at the beginning of the pipeline.
/// </summary>
public class ApplicationModelStartupFilter : IStartupFilter
{
    /// <inheritdoc/>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            // Add Application Model middleware at the beginning of the pipeline
            app.UseMiddleware<TenantIdMiddleware>();

            // Continue with the rest of the pipeline
            next(app);
        };
    }
}