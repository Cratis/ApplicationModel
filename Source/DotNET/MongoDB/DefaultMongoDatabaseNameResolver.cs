// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IMongoDatabaseNameResolver"/> that resolves a configured database name.
/// </summary>
/// <param name="options">The <see cref="IOptionsMonitor{TOptions}"/>.</param>
public class DefaultMongoDatabaseNameResolver(IOptionsMonitor<MongoDBOptions> options) : IMongoDatabaseNameResolver
{
    /// <inheritdoc/>
    public string Resolve() => new(options.CurrentValue.Database);
}