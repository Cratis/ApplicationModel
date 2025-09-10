// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cratis.Applications.MongoDB.Resilience.for_MongoCollectionInterceptorSelector.when_selecting_interceptors;

public class for_async_methods : given.an_interceptor_selector
{
    protected IEnumerable<MethodInfo> _asyncMethods;
    int _interceptedMethods;

    void Establish()
    {
        _asyncMethods = [.. typeof(IMongoCollection<BsonDocument>).GetMethods().Where(m => m.ReturnType == typeof(Task))];
    }

    void Because() => _interceptedMethods = _asyncMethods.Count(methodInfo =>
    {
        var interceptors = selector.SelectInterceptors(typeof(IMongoCollection<BsonDocument>), methodInfo, []);
        return interceptors.Length == 1 && interceptors[0] is MongoCollectionInterceptor;
    });

    [Fact] void should_have_the_mongo_collection_interceptor_for_all() => _interceptedMethods.ShouldEqual(_asyncMethods.Count());
}
