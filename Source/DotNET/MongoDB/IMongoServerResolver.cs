// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Defines a system that can resolve a <see cref="MongoUrl"/>.
/// </summary>
public interface IMongoServerResolver
{
    /// <summary>
    /// Resolve the <see cref="MongoUrl"/> for a specific server.
    /// </summary>
    /// <returns>The resolved <see cref="MongoUrl"/>.</returns>
    MongoUrl Resolve();
}
