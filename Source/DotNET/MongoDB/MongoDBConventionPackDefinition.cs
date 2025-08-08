// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents a definition for a MongoDB convention pack.
/// </summary>
/// <param name="Name">The name of the convention pack.</param>
/// <param name="ConventionPack">The convention pack instance.</param>
public record MongoDBConventionPackDefinition(string Name, IConventionPack ConventionPack);
