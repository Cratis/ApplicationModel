// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Serialization;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Defines the builder for MongoDB.
/// </summary>
public interface IMongoDBBuilder
{
    /// <summary>
    /// Gets the class maps to register.
    /// </summary>
    IList<Type> ClassMaps { get; }

    /// <summary>
    /// Gets the convention pack filters to register.
    /// </summary>
    IList<Type> ConventionPackFilters { get; }

    /// <summary>
    /// Gets the convention pack providers, implementations of <see cref="ICanProvideMongoDBConventionPacks"/>.
    /// </summary>
    IList<Type> ConventionPackProviders { get; }

    /// <summary>
    /// Gets or sets the URL resolver type.
    /// </summary>
    Type ServerResolverType { get; set; }

    /// <summary>
    /// Gets or sets the database name resolver type.
    /// </summary>
    Type DatabaseNameResolverType { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="INamingPolicy"/> instance.
    /// </summary>
    INamingPolicy? NamingPolicy { get; set; }

    /// <summary>
    /// Validate the builder.
    /// </summary>
    void Validate();
}
