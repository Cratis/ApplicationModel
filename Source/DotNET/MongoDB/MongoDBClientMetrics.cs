// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Metrics;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type

internal static partial class MongoDBClientMetrics
{
    [Gauge<int>("mongodb-open-connections", "Number of open connections")]
    internal static partial void OpenConnections(this IMeterScope<IMongoClient> meter, int count);

    [Gauge<int>("mongodb-open-connections", "Number of connections checked out from the pool")]
    internal static partial void CheckedOutConnections(this IMeterScope<IMongoClient> meter, int count);

    [Gauge<int>("mongodb-connections-in-pool", "Number of connections added to the pool")]
    internal static partial void ConnectionsAddedToPool(this IMeterScope<IMongoClient> meter, int count);

    [Gauge<int>("mongodb-commands", "Number of commands")]
    internal static partial void Commands(this IMeterScope<IMongoClient> meter, int count);

    [Counter<int>("mongodb-failed-connections", "Number of failed connections")]
    internal static partial void FailedConnections(this IMeterScope<IMongoClient> meter);

    [Counter<int>("mongodb-aggregated-commands", "Number of aggregated commands")]
    internal static partial void AggregatedCommands(this IMeterScope<IMongoClient> meter);
}

internal static class MongoDBClientMetricsScopes
{
    internal static IMeterScope<IMongoClient> BeginScope(
        this IMeter<IMongoClient> meter,
        string server) =>
        meter.BeginScope(new Dictionary<string, object>
        {
            ["Server"] = server
        });
}
