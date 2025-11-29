// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Defines a system that can resolve the name of a MongoDB database.
/// </summary>
public interface IMongoDatabaseNameResolver
{
    /// <summary>
    /// Resolve the name of the MongoDB database.
    /// </summary>
    /// <returns>Name of the database.</returns>
    string Resolve();
}
