// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience;

/// <summary>
/// Contains known error messages used for resilience handling in MongoDB operations.
/// </summary>
public static class WellKnownErrorMessages
{
    /// <summary>
    /// The error message indicating that a MongoDB collection was not found given by at least the Azure Cosmos DB MongoDB API.
    /// </summary>
    public const string CollectionNotFound = "Collection not found";
}
