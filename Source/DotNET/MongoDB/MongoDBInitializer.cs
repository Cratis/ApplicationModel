// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents a hosted service that initializes MongoDB when the application is ready.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="MongoDBInitializer"/>.
/// </remarks>
/// <param name="serviceProvider"><see cref="IServiceProvider"/> for getting services.</param>
class MongoDBInitializer(IServiceProvider serviceProvider) : IHostedService
{
    /// <inheritdoc/>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        DatabaseExtensions.ModelNameResolver = serviceProvider.GetRequiredService<IModelNameResolver>();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}