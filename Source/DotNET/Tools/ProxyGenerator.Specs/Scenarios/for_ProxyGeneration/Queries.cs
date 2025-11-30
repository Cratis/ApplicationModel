// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Queries.ModelBound;

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_ProxyGeneration;

/// <summary>
/// A simple read model for testing proxy generation.
/// </summary>
[ReadModel]
public class SimpleReadModel
{
    /// <summary>
    /// Gets or sets the ID.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// Gets all items.
    /// </summary>
    /// <returns>Collection of read models.</returns>
    public static IEnumerable<SimpleReadModel> GetAll() =>
    [
        new SimpleReadModel { Id = Guid.NewGuid(), Name = "Item 1", Value = 1 },
        new SimpleReadModel { Id = Guid.NewGuid(), Name = "Item 2", Value = 2 },
        new SimpleReadModel { Id = Guid.NewGuid(), Name = "Item 3", Value = 3 }
    ];

    /// <summary>
    /// Gets a single item by ID.
    /// </summary>
    /// <param name="id">The ID to find.</param>
    /// <returns>The read model or null.</returns>
    public static SimpleReadModel? GetById(Guid id) =>
        new() { Id = id, Name = $"Item {id}", Value = 42 };
}
