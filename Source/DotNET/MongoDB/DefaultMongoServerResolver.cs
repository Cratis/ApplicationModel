// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IMongoServerResolver"/> that resolves a configured <see cref="MongoUrl"/>.
/// </summary>
/// <param name="options">The <see cref="IOptionsMonitor{TOptions}"/>.</param>
public class DefaultMongoServerResolver(IOptionsMonitor<MongoDBOptions> options) : IMongoServerResolver
{
    /// <inheritdoc/>
    public MongoUrl Resolve() => new(options.CurrentValue.Server);
}