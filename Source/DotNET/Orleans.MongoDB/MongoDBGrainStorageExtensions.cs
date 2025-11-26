// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.Orleans.MongoDB;
using Orleans.Providers;
using Orleans.Runtime.Hosting;

namespace Orleans.Hosting;

/// <summary>
/// Extension methods for configuring MongoDB storage.
/// </summary>
public static class MongoDBGrainStorageExtensions
{
    /// <summary>
    /// Adds MongoDB grain storage as default.
    /// </summary>
    /// <param name="builder"><see cref="ISiloBuilder"/> to add to.</param>
    /// <returns><see cref="ISiloBuilder"/> for continuation.</returns>
    public static ISiloBuilder AddCratisMongoDBStorageAsDefault(this ISiloBuilder builder) =>
        AddCratisMongoDBStorage(builder, ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME);

    /// <summary>
    /// Adds MongoDB grain storage.
    /// </summary>
    /// <param name="builder"><see cref="ISiloBuilder"/> to add to.</param>
    /// <param name="providerName">Name of the provider.</param>
    /// <returns><see cref="ISiloBuilder"/> for continuation.</returns>
    public static ISiloBuilder AddCratisMongoDBStorage(this ISiloBuilder builder, string providerName)
    {
        builder.ConfigureServices(services => services.AddGrainStorage(providerName, MongoDBGrainStorageFactory.Create));
        return builder;
    }
}