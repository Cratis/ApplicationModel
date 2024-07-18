// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace MongoDB.Driver;

#pragma warning disable MA0048 // File name must match type name

internal static partial class MongoCollectionExtensionsLogMessages
{
    [LoggerMessage(LogLevel.Trace, "Cursor '{Name}' disposed")]
    internal static partial void CursorDisposed(this ILogger<MongoCollectionExtensions.MongoCollection> logger, string name);
}