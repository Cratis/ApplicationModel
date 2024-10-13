// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orleans.Configuration;

namespace Cratis.Applications.Orleans.MongoDB;

public record MembershipInfo(
        string Id,
        string ClusterId,
        string Address,
        string Port,
        string Generation,
        string HostName,
        SiloStatus Status,
        int ProxyPort,
        string RoleName,
        string SiloName,
        string UpdateZone,
        string FaultZone,
        string SuspectingSilos,
        string SuspectingTimes,
        DateTime StartTime,
        DateTime IAmAliveTime,
        DateTimeOffset? TimeStamp);

public static class MembershipInfoConverters
{
    public string ToKey(this SiloAddress address) => $"{address.Endpoint.Address}-{address.Endpoint.Port}-{address.Generation}";

    public static MembershipInfo ToMembershipInfo(this MembershipEntry entry, string clusterId) => new(
        entry.SiloAddress.ToKey(),
        clusterId,
        entry.SiloAddress.Endpoint.Address.ToString(),
        entry.SiloAddress.Endpoint.Port.ToString(),
        entry.SiloAddress.Generation.ToString(),
        entry.HostName,
        entry.Status,
        entry.ProxyPort,
        entry.RoleName,
        entry.SiloName,
        entry.UpdateZone.ToString(CultureInfo.InvariantCulture),
        entry.FaultZone.ToString(CultureInfo.InvariantCulture),
        entry.SuspectingSilos,
        entry.SuspectTimes,
        entry.StartTime,
        entry.IAmAliveTime,
        DateTimeOffset.UtcNow)
}


/// <summary>
/// Represents the options for configuring the membership table.
/// </summary>
public class MongoDBMembershipOptions
{
    /// <summary>
    /// Gets or sets the connection string to the MongoDB server.
    /// </summary>
    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    /// <summary>
    /// Gets or sets the name of the database to use.
    /// </summary>
    public string DatabaseName { get; set; } = "orleans";

    /// <summary>
    /// Gets or sets the name of the collection to use.
    /// </summary>
    public string CollectionName { get; set; } = "silos";
}

/// <summary>
/// Represents an implementation of <see cref="IMembershipTable"/> for MongoDB.
/// </summary>
/// <param name="mongoOptions"><see cref="MongoDBMembershipOptions"/> for configuring the membership table.</param>
public class MongoDBMembershipTable : IMembershipTable
{
    readonly IMongoCollection<MembershipInfo> _collection;

    /// <summary>
    /// Initializes a new instance of <see cref="MongoDBMembershipTable"/>.
    /// </summary>
    /// <param name="clusterOptions"><see cref="ClusterOptions"/> for configuring the cluster.</param>
    /// <param name="mongoOptions"><see cref="MongoDBMembershipOptions"/> for configuring the membership table.</param>
    public MongoDBMembershipTable(
        IOptions<ClusterOptions> clusterOptions,
        IOptions<MongoDBMembershipOptions> mongoOptions)
    {
        var client = new MongoClient(mongoOptions.Value.ConnectionString);
        var database = client.GetDatabase(mongoOptions.Value.DatabaseName);
        _collection = database.GetCollection<MembershipInfo>(mongoOptions.Value.CollectionName);
    }

    /// <inheritdoc/>
    public Task InitializeMembershipTable(bool tryInitTableVersion) => Task.CompletedTask;

    /// <inheritdoc/>
    public Task DeleteMembershipTableEntries(string clusterId) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task CleanupDefunctSiloEntries(DateTimeOffset beforeDate) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<bool> InsertRow(MembershipEntry entry, TableVersion tableVersion) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadAll() => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<MembershipTableData> ReadRow(SiloAddress key) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task UpdateIAmAlive(MembershipEntry entry) => throw new NotImplementedException();

    /// <inheritdoc/>
    public Task<bool> UpdateRow(MembershipEntry entry, string etag, TableVersion tableVersion) => throw new NotImplementedException();
}
