// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB.for_QueryContextAwareSet.when_initializing_with_query;

public class and_there_are_multiple_documents : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;
    IEnumerable<SomeClassWithSomeId> _documents;

    void Establish()
    {
        _documents = [new(Guid.NewGuid(), 42), new(Guid.NewGuid(), 44), new(Guid.NewGuid(), 43), new(Guid.NewGuid(), 429), new(Guid.NewGuid(), 1)];
        _set = new(QueryContextBuilder.New().Build());
    }

    void Because()
    {
        _set.InitializeWithQuery(new InMemoryFluentFind<SomeClassWithSomeId>(_documents));
    }

    [Fact] void should_have_same_sequence() => Assert.Equal(_documents, _set);
}