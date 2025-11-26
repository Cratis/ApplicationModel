// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.MongoDB.Resilience.for_MongoCollectionInterceptorForReturnValue.when_intercepting;

public class successful_method : given.an_interceptor
{
    string _result;
    protected override string GetInvocationTargetMethod() => nameof(for_MongoCollectionInterceptor.InvocationTarget.SuccessfulMethod);

    async Task Because()
    {
        _interceptor.Intercept(_invocation);
        _result = await _returnValue;
    }

    [Fact] void should_return_value_from_invocation() => _result.ShouldEqual("Hello");
    [Fact] void should_return_successful_task() => _returnValue.IsCompletedSuccessfully.ShouldBeTrue();
    [Fact] void should_have_freed_up_semaphore() => _semaphore.CurrentCount.ShouldEqual(PoolSize);
}
