// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MongoDB.Driver;

/// <summary>
/// Exception that gets thrown when type is missing an Id property mapping.
/// </summary>
/// <param name="type">Type that is missing the Id property mapping.</param>
public class MissingIdMapping(Type type) : Exception($"Missing Id mapping for type {type.FullName}");