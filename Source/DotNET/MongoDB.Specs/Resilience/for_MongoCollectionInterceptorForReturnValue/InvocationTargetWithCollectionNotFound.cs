// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;

namespace Cratis.Applications.MongoDB.Resilience.for_MongoCollectionInterceptorForReturnValue;

public class InvocationTargetWithCollectionNotFound
{
    public const string ErrorMessage = "Collection not found";

    static MongoCommandException CreateCollectionNotFoundException()
    {
        var endPoint = new System.Net.DnsEndPoint("localhost", 27017);
        var serverId = new ServerId(new ClusterId(0), endPoint);
        var connectionId = new ConnectionId(serverId);
        return new(connectionId, ErrorMessage, new BsonDocument());
    }

    public Task<IAsyncCursor<string>> FindAsyncCollectionNotFound() =>
        Task.FromException<IAsyncCursor<string>>(CreateCollectionNotFoundException());

    public Task<long> CountAsyncCollectionNotFound() =>
        Task.FromException<long>(CreateCollectionNotFoundException());

    public Task<string?> FindOneCollectionNotFound() =>
        Task.FromException<string?>(CreateCollectionNotFoundException());
}
