// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Reflection;

namespace Cratis.Applications.Queries.ModelBound;

/// <summary>
/// Extension methods for the <see cref="ReadModelAttribute"/>.
/// </summary>
public static class ReadModelAttributeExtensions
{
    /// <summary>
    /// Check if a type is a read model.
    /// </summary>
    /// <param name="type">Type to check.</param>
    /// <returns>True if the type is a read model; otherwise, false.</returns>
    public static bool IsReadModel(this Type type) => type.HasAttribute<ReadModelAttribute>();
}
