// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Castle.DynamicProxy;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB.Resilience;

/// <summary>
/// Represents an interceptor for <see cref="IMongoClient"/> Dispose method.
/// </summary>
public class MongoClientDisposeInterceptor : IInterceptor
{
     /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    {
        // Do nothing - we don't want to dispose the client as we want to keep a single instance across a running process
    }
}