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
    [Counter<int>("cratis-applicationmodel-mongodb-open-connections", "Number of open connections")]
    internal static partial void OpenConnections(this IMeterScope<IMongoClient> meter, int count);

    [Counter<int>("cratis-applicationmodel-mongodb-open-connections", "Number of connections checked out from the pool")]
    internal static partial void CheckedOutConnections(this IMeterScope<IMongoClient> meter, int count);

    [Counter<int>("cratis-applicationmodel-mongodb-commands", "Number of commands performed")]
    internal static partial void CommandsPerformed(this IMeterScope<IMongoClient> meter, int count);
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
