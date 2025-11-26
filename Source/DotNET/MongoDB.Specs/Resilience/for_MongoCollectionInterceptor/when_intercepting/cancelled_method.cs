// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_MongoCollectionInterceptor.when_intercepting;

public class cancelled_method : given.an_interceptor
{
    protected override string GetInvocationTargetMethod() => nameof(for_MongoCollectionInterceptorForReturnValue.InvocationTarget.CancelledMethod);
    Exception _exception;

    async Task Because()
    {
        _interceptor.Intercept(_invocation);
        _exception = await Catch.Exception(async () => await _returnValue);
    }

    [Fact] void should_bubble_up_cancelled_exception() => _exception.ShouldBeOfExactType<TaskCanceledException>();
    [Fact] void should_have_cancelled_task() => _returnValue.IsCanceled.ShouldBeTrue();
    [Fact] void should_have_freed_up_semaphore() => _semaphore.CurrentCount.ShouldEqual(PoolSize);
}
