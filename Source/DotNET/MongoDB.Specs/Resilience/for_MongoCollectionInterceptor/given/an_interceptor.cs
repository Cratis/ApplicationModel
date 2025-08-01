// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using Polly;

namespace Cratis.Applications.MongoDB.Resilience.for_MongoCollectionInterceptor.given;

public abstract class an_interceptor : Specification
{
    protected const int pool_size = 10;
    protected ResiliencePipeline resilience_pipeline;
    protected MongoClientSettings settings;
    protected MongoCollectionInterceptor interceptor;
    protected Castle.DynamicProxy.IInvocation invocation;
    protected Task return_value;
    protected for_MongoCollectionInterceptorForReturnValue.InvocationTarget target;
    protected SemaphoreSlim semaphore;

    protected abstract string GetInvocationTargetMethod();

    void Establish()
    {
        resilience_pipeline = new ResiliencePipelineBuilder().Build();

        semaphore = new SemaphoreSlim(pool_size, pool_size);

        interceptor = new(resilience_pipeline, semaphore);

        invocation = Substitute.For<Castle.DynamicProxy.IInvocation>();
        invocation.Method.Returns(typeof(for_MongoCollectionInterceptorForReturnValue.InvocationTarget).GetMethod(GetInvocationTargetMethod())!);
        target = new();
        invocation.InvocationTarget.Returns(target);
        invocation.When(_ => _.ReturnValue = Arg.Any<Task>()).Do((_) => return_value = _.Arg<Task>());
    }
}
