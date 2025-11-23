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
/// Initializes a new instance of the <see cref="MongoCollectionInterceptor"/> class.
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

        var cancellationToken = ExtractCancellationToken(invocation);

#pragma warning disable CA2012 // Use ValueTasks correctly
        resiliencePipeline.ExecuteAsync(
            async _ =>
            {
                if (!await TryAcquireSemaphore(tcs, cancellationToken))
                {
                    return ValueTask.CompletedTask;
                }

                try
                {
                    await ExecuteMongoOperation(invocation, tcs);
                }
                catch (OperationCanceledException)
                {
                    tcs.SetCanceled();
                }
                catch (MongoCommandException ex) when (ex.Message.Contains(WellKnownErrorMessages.CollectionNotFound, StringComparison.OrdinalIgnoreCase))
                {
                    tcs.SetResult();
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

    static CancellationToken ExtractCancellationToken(IInvocation invocation) =>
        invocation.Arguments.FirstOrDefault(argument => argument is CancellationToken) as CancellationToken? ?? CancellationToken.None;

    static async Task ExecuteMongoOperation(IInvocation invocation, TaskCompletionSource tcs)
    {
        var result = (invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments) as Task)!;
        await result.ConfigureAwait(false);

        if (result.IsCanceled)
        {
            tcs.SetCanceled();
        }
        else
        {
            tcs.SetResult();
        }
    }

    async Task<bool> TryAcquireSemaphore(TaskCompletionSource tcs, CancellationToken cancellationToken)
    {
        if (!await openConnectionSemaphore.WaitAsync(1000, cancellationToken))
        {
            tcs.SetException(new TimeoutException("Failed to acquire semaphore."));
            return false;
        }
        return true;
    }
}
