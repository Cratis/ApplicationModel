// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using Polly;

namespace Cratis.Applications.MongoDB.Resilience.for_MongoCollectionInterceptorSelector.given;

public class an_interceptor_selector : Specification
{
    protected MongoCollectionInterceptorSelector selector;
    protected ResiliencePipeline resilience_pipeline;
    protected IMongoClient mongo_client;
    protected MongoClientSettings settings;

    void Establish()
    {
        resilience_pipeline = new ResiliencePipelineBuilder().Build();
        mongo_client = Substitute.For<IMongoClient>();
        settings = new MongoClientSettings();
        mongo_client.Settings.Returns(settings);
        selector = new MongoCollectionInterceptorSelector(resilience_pipeline, mongo_client);
    }
}
