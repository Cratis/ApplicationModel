// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB.for_QueryContextAwareSet.when_initializing_with_query;

public class and_there_are_no_documents : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;
    void Establish()
    {
        _set = new(QueryContextBuilder.New().Build());
    }

    void Because()
    {
        _set.InitializeWithQuery(new InMemoryFluentFind<SomeClassWithSomeId>([]));
    }

    [Fact] void should_have_no_documents() => Assert.Equal([], _set);
}