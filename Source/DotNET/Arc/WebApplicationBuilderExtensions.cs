// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications;
using Microsoft.Extensions.Hosting;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Provides extension methods for <see cref="WebApplicationBuilder"/> for configuring the application model services.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Use Cratis ApplicationModel with the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <remarks>
    /// Binds the <see cref="ApplicationModelOptions"/> configuration to the given config section path or the default
    /// Cratis:ApplicationModel section path.
    /// </remarks>
    /// <param name="builder"><see cref="WebApplicationBuilder"/> to extend.</param>
    /// <param name="configSectionPath">The optional configuration section path.</param>
    /// <returns><see cref="WebApplicationBuilder"/> for building continuation.</returns>
    public static WebApplicationBuilder UseCratisApplicationModel(this WebApplicationBuilder builder, string? configSectionPath = null)
    {
        builder.Host.UseCratisApplicationModel(configSectionPath);
        return builder;
    }

    /// <summary>
    /// Use Cratis ApplicationModel with the <see cref="WebApplicationBuilder"/>.
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/> to extend.</param>
    /// <param name="configureOptions">Action to configure the <see cref="ApplicationModelOptions"/>.</param>
    /// <returns><see cref="WebApplicationBuilder"/> for building continuation.</returns>
    public static WebApplicationBuilder UseCratisApplicationModel(this WebApplicationBuilder builder, Action<ApplicationModelOptions> configureOptions)
    {
        builder.Host.UseCratisApplicationModel(configureOptions);
        return builder;
    }
}