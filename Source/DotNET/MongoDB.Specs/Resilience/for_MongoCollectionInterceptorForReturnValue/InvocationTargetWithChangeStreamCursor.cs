// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using MongoDB.Driver.Core.Connections;
using MongoDB.Driver.Core.Servers;

namespace Cratis.Arc.MongoDB.Resilience.for_MongoCollectionInterceptorForReturnValue;

public class InvocationTargetWithChangeStreamCursor(int failCount = 0, IChangeStreamCursor<string>? successCursor = null)
{
    int _callCount;

    public int FailCount { get; } = failCount;
    public int CallCount => _callCount;

    static MongoCommandException CreateCollectionNotFoundException()
    {
        var endPoint = new System.Net.DnsEndPoint("localhost", 27017);
        var serverId = new ServerId(new ClusterId(0), endPoint);
        var connectionId = new ConnectionId(serverId);
        return new(connectionId, InvocationTargetWithCollectionNotFound.ErrorMessage, new BsonDocument());
    }

    public Task<IChangeStreamCursor<string>> WatchAsyncCollectionNotFound()
    {
        _callCount++;
        if (_callCount <= FailCount)
        {
            return Task.FromException<IChangeStreamCursor<string>>(CreateCollectionNotFoundException());
        }

        return Task.FromResult(successCursor ?? Substitute.For<IChangeStreamCursor<string>>());
    }
}
