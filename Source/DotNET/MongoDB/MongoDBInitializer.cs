// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Models;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents a hosted service that initializes MongoDB when the application is ready.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="MongoDBInitializer"/>.
/// </remarks>
/// <param name="modelNameResolver"><see cref="IModelNameResolver"/>.</param>
sealed class MongoDBInitializer(IModelNameResolver modelNameResolver) : IHostedService
{
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        DatabaseExtensions.SetModelNameResolver(modelNameResolver);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}