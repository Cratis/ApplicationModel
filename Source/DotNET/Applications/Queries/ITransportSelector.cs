// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a system that determines which transport to use for observable queries.
/// </summary>
public interface ITransportSelector
{
    /// <summary>
    /// Selects the appropriate transport type for the given HTTP context.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/> to evaluate.</param>
    /// <returns>The selected <see cref="TransportType"/>, or null if no suitable transport is available.</returns>
    TransportType? SelectTransport(HttpContext httpContext);
}
