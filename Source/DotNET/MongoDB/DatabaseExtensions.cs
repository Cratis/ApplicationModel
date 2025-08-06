// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.MongoDB;
using Cratis.Serialization;

namespace MongoDB.Driver;

/// <summary>
/// Convenience extension methods for <see cref="IMongoDatabase"/>.
/// </summary>
public static class DatabaseExtensions
{
    static INamingPolicy? _namingPolicy;

    /// <summary>
    /// The <see cref="INamingPolicy"/> to use.
    /// </summary>
    /// <exception cref="NamingPolicyNotConfigured">Thrown when the resolver is not configured.</exception>
    internal static INamingPolicy NamingPolicy =>
        _namingPolicy ?? throw new NamingPolicyNotConfigured($"Cannot use {nameof(IMongoDatabase)}.{nameof(GetCollection)}() before it has been configured. Please configure it using {nameof(MongoDBBuilderExtensions.WithNamingPolicy)}");

    /// <summary>
    /// Get a collection - with name of collection as convention (camelCase of typename).
    /// </summary>
    /// <param name="database"><see cref="IMongoDatabase"/> to extend.</param>
    /// <param name="settings">Optional <see cref="MongoCollectionSettings"/>.</param>
    /// <typeparam name="T">Type of collection to get.</typeparam>
    /// <returns>The collection for your type.</returns>
    public static IMongoCollection<T> GetCollection<T>(this IMongoDatabase database, MongoCollectionSettings? settings = default)
    {
        return database.GetCollection<T>(NamingPolicy!.GetReadModelName(typeof(T)), settings);
    }

    /// <summary>
    /// Sets the <see cref="INamingPolicy"/>.
    /// </summary>
    /// <param name="namingPolicy">The <see cref="INamingPolicy"/>.</param>
    internal static void SetNamingPolicy(INamingPolicy namingPolicy) => _namingPolicy = namingPolicy;
}
