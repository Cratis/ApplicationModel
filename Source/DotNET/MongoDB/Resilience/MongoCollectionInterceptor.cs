// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Castle.DynamicProxy;
using MongoDB.Driver;
using Polly;

namespace Cratis.Applications.MongoDB.Resilience;

/// <summary>
/// Represents an interceptor for <see cref="IMongoCollection{TDocument}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MongoCollectionInterceptorForReturnValues"/> class.
/// </remarks>
/// <param name="resiliencePipeline">The <see cref="ResiliencePipeline"/> to use.</param>
/// <param name="openConnectionSemaphore">The <see cref="SemaphoreSlim"/> for keeping track of open connections.</param>
public class MongoCollectionInterceptor(
    ResiliencePipeline resiliencePipeline,
    SemaphoreSlim openConnectionSemaphore) : IInterceptor
{
    /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        invocation.ReturnValue = tcs.Task;

        var cancellationToken = invocation.Arguments
            .FirstOrDefault(argument => argument is CancellationToken) as CancellationToken? ?? CancellationToken.None;

#pragma warning disable CA2012 // Use ValueTasks correctly
        resiliencePipeline.ExecuteAsync(
            async _ =>
            {
                if (!await openConnectionSemaphore.WaitAsync(1000, cancellationToken))
                {
                    tcs.SetException(new TimeoutException("Failed to acquire semaphore."));
                    return ValueTask.CompletedTask;
                }

                try
                {
                    var result = (invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments) as Task)!;
                    await result.ConfigureAwait(false);

                    if (result.IsCanceled)
                        tcs.SetCanceled();
                    else
                        tcs.SetResult();
                }
                catch (OperationCanceledException)
                {
                    tcs.SetCanceled();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    openConnectionSemaphore.Release();
                }

                return ValueTask.CompletedTask;
            },
            cancellationToken);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }
}
