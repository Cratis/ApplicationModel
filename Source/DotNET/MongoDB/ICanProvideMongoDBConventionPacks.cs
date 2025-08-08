// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Defines a system that can provide convention packs for MongoDB.
/// </summary>
public interface ICanProvideMongoDBConventionPacks
{
    /// <summary>
    /// Returns the convention packs that should be added to the MongoDB setup.
    /// </summary>
    /// <returns>A collection of <see cref="MongoDBConventionPackDefinition"/>.</returns>
    IEnumerable<MongoDBConventionPackDefinition> Provide();
}