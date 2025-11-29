// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for configuring observable query transport options.
/// </summary>
public static class ObservableQueryTransportExtensions
{
    /// <summary>
    /// Configures observable query transport options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/>.</param>
    /// <param name="configureOptions">Action to configure the options.</param>
    /// <returns>The <see cref="IServiceCollection"/> for continuation.</returns>
    public static IServiceCollection ConfigureObservableQueryTransport(
        this IServiceCollection services,
        Action<ObservableQueryTransportOptions> configureOptions)
    {
        services.Configure(configureOptions);
        return services;
    }
}
