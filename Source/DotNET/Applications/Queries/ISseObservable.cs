// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET10_0_OR_GREATER
using Microsoft.AspNetCore.Http;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines an observable that can stream using Server-Sent Events.
/// </summary>
public interface ISseObservable
{
    /// <summary>
    /// Streams data using Server-Sent Events.
    /// </summary>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task StreamAsSse(HttpContext httpContext);
}
#endif
