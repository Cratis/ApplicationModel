// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_MongoCollectionInterceptor.when_intercepting;

public class faulted_method_without_task : given.an_interceptor
{
    Exception _exception;

    protected override string GetInvocationTargetMethod() => nameof(for_MongoCollectionInterceptorForReturnValue.InvocationTarget.FaultedMethodWithoutTask);

    void Because()
    {
        _interceptor.Intercept(_invocation);
        _exception = _returnValue.Exception.InnerExceptions.Single().InnerException;
    }

    [Fact] void should_be_faulted() => _returnValue.IsFaulted.ShouldBeTrue();
    [Fact] void should_bubble_up_exception() => _exception.Message.ShouldEqual(for_MongoCollectionInterceptorForReturnValue.InvocationTarget.ErrorMessage);
    [Fact] void should_have_freed_up_semaphore() => _semaphore.CurrentCount.ShouldEqual(PoolSize);
}
