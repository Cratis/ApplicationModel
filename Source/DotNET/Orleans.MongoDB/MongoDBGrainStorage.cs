// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Arc.MongoDB;
using Cratis.Concepts;
using MongoDB.Driver;
using Orleans.Storage;

namespace Cratis.Arc.Orleans.MongoDB;

/// <summary>
/// Represents a MongoDB implementation of <see cref="IGrainStorage"/>.
/// </summary>
public class MongoDBGrainStorage : IGrainStorage
{
    readonly IMongoDatabase _database;

    /// <summary>
    /// Initializes a new instance of <see cref="MongoDBGrainStorage"/>.
    /// </summary>
    /// <param name="clientFactory"><see cref="IMongoDBClientFactory"/> for accessing MongoDB.</param>
    /// <param name="databaseNameResolver"><see cref="IMongoDatabaseNameResolver"/> for resolving the database name.</param>
    public MongoDBGrainStorage(IMongoDBClientFactory clientFactory, IMongoDatabaseNameResolver databaseNameResolver)
    {
        var client = clientFactory.Create();
        _database = client.GetDatabase(databaseNameResolver.Resolve());
    }

    /// <inheritdoc/>
    public Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var collection = _database.GetCollection<T>();
        var filter = GetFilter<T>(grainId);
        return collection.DeleteOneAsync(filter);
    }

    /// <inheritdoc/>
    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var collection = _database.GetCollection<T>();
        var filter = GetFilter<T>(grainId);
        var findResult = await collection.FindAsync(filter);
        var result = findResult.FirstOrDefault();
        if (result is not null)
        {
            grainState.State = result;
        }
    }

    /// <inheritdoc/>
    public Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var collection = _database.GetCollection<T>();
        var filter = GetFilter<T>(grainId);
        SetIdFromGrainId(grainState.State, grainId);
        return collection.ReplaceOneAsync(filter, grainState.State, new ReplaceOptions { IsUpsert = true });
    }

    FilterDefinition<T> GetFilter<T>(GrainId grainId)
    {
        if (grainId.TryGetGuidKey(out var guidKey, out _))
        {
            return Builders<T>.Filter.Eq("_id", guidKey);
        }
        if (grainId.TryGetIntegerKey(out var integerKey, out _))
        {
            return Builders<T>.Filter.Eq("_id", integerKey);
        }

        return Builders<T>.Filter.Eq("_id", grainId.Key.ToString());
    }

    void SetIdFromGrainId<T>(T state, GrainId grainId)
    {
        if (state is null) return;

        var idProperty = state.GetType().GetProperty("Id");
        if (idProperty?.CanWrite != true) return;

        object? value;

        if (grainId.TryGetGuidKey(out var guidKey, out _))
        {
            value = guidKey;
        }
        else if (grainId.TryGetIntegerKey(out var integerKey, out _))
        {
            value = integerKey;
        }
        else
        {
            value = grainId.Key.ToString();
        }

        if (idProperty.PropertyType.IsConcept())
        {
            value = ConceptFactory.CreateConceptInstance(idProperty.PropertyType, value);
        }

        idProperty.SetValue(state, value);
    }
}
