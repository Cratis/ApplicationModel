// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_initializing_with_query;

public class and_there_is_only_one_document : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    IEnumerable<SomeClassWithSomeId> documents;
    void Establish()
    {
        documents = [new(Guid.NewGuid(), 42)];
        set = new(QueryContextBuilder.New().Build());
    }

    void Because()
    {
        set.InitializeWithQuery(new InMemoryFluentFind<SomeClassWithSomeId>(documents));
    }

    [Fact] void should_have_same_sequence() => Assert.Equal(documents, set);
}