// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Castle.DynamicProxy;
using MongoDB.Driver;
using Polly;

namespace Cratis.Applications.MongoDB.Resilience;

/// <summary>
/// Represents an interceptor for <see cref="IMongoCollection{TDocument}"/> for methods that returns a <see cref="Task{T}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MongoCollectionInterceptorForReturnValues"/> class.
/// </remarks>
/// <param name="resiliencePipeline">The <see cref="ResiliencePipeline"/> to use.</param>
/// <param name="openConnectionSemaphore">The <see cref="SemaphoreSlim"/> for keeping track of open connections.</param>
public class MongoCollectionInterceptorForReturnValues(
    ResiliencePipeline resiliencePipeline,
    SemaphoreSlim openConnectionSemaphore) : IInterceptor
{
    /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    {
        Task returnTask = null!;
        MethodInfo setResultMethod = null!;
        MethodInfo setExceptionMethod = null!;
        MethodInfo setCanceledMethod = null!;

        var returnType = invocation.Method.ReturnType.GetGenericArguments()[0];
        var taskType = typeof(TaskCompletionSource<>).MakeGenericType(returnType);
        var tcs = Activator.CreateInstance(taskType, new[] { TaskCreationOptions.RunContinuationsAsynchronously })!;
        var tcsType = tcs.GetType();
        setResultMethod = tcsType.GetMethod(nameof(TaskCompletionSource<object>.SetResult))!;
        setExceptionMethod = tcsType.GetMethod(nameof(TaskCompletionSource<object>.SetException), [typeof(Exception)])!;
        setCanceledMethod = tcsType.GetMethod(nameof(TaskCompletionSource<object>.SetCanceled), [])!;
        returnTask = (tcsType.GetProperty(nameof(TaskCompletionSource<object>.Task))!.GetValue(tcs) as Task)!;

        invocation.ReturnValue = returnTask!;
        var cancellationToken = invocation.Arguments.FirstOrDefault(argument => argument is CancellationToken) as CancellationToken? ?? CancellationToken.None;

#pragma warning disable CA2012 // Use ValueTasks correctly
        resiliencePipeline.ExecuteAsync(
            async (_) =>
            {
                if (!await openConnectionSemaphore.WaitAsync(1000, cancellationToken))
                {
                    setExceptionMethod.Invoke(tcs, [new TimeoutException("Failed to acquire semaphore.")]);
                    return ValueTask.CompletedTask;
                }
                try
                {
                    var result = (invocation.Method.Invoke(invocation.InvocationTarget, invocation.Arguments) as Task)!;
                    await result.ConfigureAwait(false);
                    if (result.IsCanceled)
                    {
                        setCanceledMethod.Invoke(tcs, []);
                    }
                    else
                    {
#pragma warning disable CA1849 // Synchronous blocks
                        var taskResult = result.GetType().GetProperty(nameof(Task<object>.Result))!.GetValue(result);
                        setResultMethod.Invoke(tcs, [taskResult]);
#pragma warning restore CA1849 // Synchronous blocks
                    }
                }
                catch (OperationCanceledException)
                {
                    openConnectionSemaphore.Release(1);
                    setCanceledMethod.Invoke(tcs, []);
                }
                catch (Exception ex)
                {
                    openConnectionSemaphore.Release(1);
                    setExceptionMethod.Invoke(tcs, [ex]);
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
