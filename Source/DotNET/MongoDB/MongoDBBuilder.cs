// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Models;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IMongoDBBuilder"/>.
/// </summary>
public class MongoDBBuilder : IMongoDBBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDBBuilder"/> class.
    /// </summary>
    public MongoDBBuilder()
    {
        var types = Types.Types.Instance;
        ClassMaps = types.FindMultiple(typeof(IBsonClassMapFor<>)).ToList();
        ConventionPackFilters = types.FindMultiple<ICanFilterMongoDBConventionPacksForType>().ToList();
    }

    /// <inheritdoc/>
    public IList<Type> ClassMaps { get; }

    /// <inheritdoc/>
    public IList<Type> ConventionPackFilters { get; }

    /// <inheritdoc/>
    public Type ServerResolverType { get; set; } = typeof(DefaultMongoServerResolver);

    /// <inheritdoc/>
    public Type DatabaseNameResolverType { get; set; } = typeof(DefaultMongoDatabaseNameResolver);

    /// <inheritdoc/>
    public IModelNameConvention? ModelNameConventionInstance { get; set; }

    /// <inheritdoc/>
    public Type? ModelNameConventionType { get; set; } = typeof(DefaultModelNameConvention);

    /// <inheritdoc/>
    public void Validate()
    {
        MongoServerResolverNotConfigured.ThrowIfNotConfigured(ServerResolverType);
        MongoDatabaseNameResolverNotConfigured.ThrowIfNotConfigured(DatabaseNameResolverType);
        ModelNameResolverNotConfigured.ThrowIfNotConfigured(ModelNameConventionInstance, ModelNameConventionType);
    }
}
