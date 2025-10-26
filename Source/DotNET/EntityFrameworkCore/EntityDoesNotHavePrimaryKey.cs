// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Exception that gets thrown when an entity does not have a primary key defined.
/// </summary>
/// <param name="entityType">The entity type that does not have a primary key.</param>
public class EntityDoesNotHavePrimaryKey(Type entityType)
    : Exception($"Entity type {entityType.Name} does not have a primary key defined.");
