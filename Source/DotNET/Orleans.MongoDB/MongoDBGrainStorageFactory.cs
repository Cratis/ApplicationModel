// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.MongoDB;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Storage;

namespace Cratis.Arc.Orleans.MongoDB;

/// <summary>
/// Represents a factory for creating instances of <see cref="MongoDBGrainStorage"/>.
/// </summary>
internal static class MongoDBGrainStorageFactory
{
    /// <summary>
    /// Create a new instance of <see cref="IGrainStorage"/>.
    /// </summary>
    /// <param name="services"><see cref="IServiceProvider"/> to use for creation.</param>
    /// <param name="name">Name of the storage.</param>
    /// <returns>A new instance of the storage.</returns>
#pragma warning disable IDE0060 // Remove unused parameter - see MongoDBGrainStorageExtensions for usage
    internal static IGrainStorage Create(IServiceProvider services, string name)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return ActivatorUtilities.CreateInstance<MongoDBGrainStorage>(
            services,
            services.GetRequiredService<IMongoDBClientFactory>(),
            services.GetRequiredService<IMongoDatabaseNameResolver>());
    }
}
