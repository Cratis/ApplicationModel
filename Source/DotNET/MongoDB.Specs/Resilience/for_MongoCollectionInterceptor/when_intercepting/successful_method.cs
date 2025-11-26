// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_MongoCollectionInterceptor.when_intercepting;

public class successful_method : given.an_interceptor
{
    protected override string GetInvocationTargetMethod() => nameof(for_MongoCollectionInterceptorForReturnValue.InvocationTarget.SuccessfulMethod);

    void Because() => _interceptor.Intercept(_invocation);

    [Fact] void should_return_successful_task() => _returnValue.IsCompletedSuccessfully.ShouldBeTrue();
    [Fact] void should_have_freed_up_semaphore() => _semaphore.CurrentCount.ShouldEqual(PoolSize);
}
