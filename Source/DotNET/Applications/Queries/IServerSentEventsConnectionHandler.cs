// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NET10_0_OR_GREATER
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Queries;

/// <summary>
/// Defines a system that knows how to handle Server-Sent Events connections for observable queries.
/// </summary>
public interface IServerSentEventsConnectionHandler
{
    /// <summary>
    /// Streams query results as Server-Sent Events.
    /// </summary>
    /// <typeparam name="T">The type of data being streamed.</typeparam>
    /// <param name="httpContext">The <see cref="HttpContext"/>.</param>
    /// <param name="items">The async enumerable of items to stream.</param>
    /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions"/>.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <param name="logger">The optional <see cref="ILogger"/> to use.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous action.</returns>
    Task StreamQueryResults<T>(
        HttpContext httpContext,
        IAsyncEnumerable<QueryResult> items,
        JsonSerializerOptions jsonSerializerOptions,
        CancellationToken cancellationToken,
        ILogger? logger = null);
}
#endif
