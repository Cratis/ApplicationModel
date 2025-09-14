// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;

namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_remove_and_add_last_in_query;

public class and_there_are_multiple_documents_in_the_query : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;

    SomeClassWithSomeId _firstItem;
    SomeClassWithSomeId _secondItem;
    SomeClassWithSomeId _thirdItem;
    SomeClassWithSomeId _fourthItem;
    IEnumerable<SomeClassWithSomeId> _queryDocuments;

    void Establish()
    {
        _set = new(QueryContextBuilder.New()
            .WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Ascending))
            .WithPageSize(4)
            .Build());
        _queryDocuments = [new(Guid.NewGuid(), 49), new(Guid.NewGuid(), 46), new(Guid.NewGuid(), 52)];
        _firstItem = new(Guid.NewGuid(), 40);
        _secondItem = new(Guid.NewGuid(), 41);
        _thirdItem = new(Guid.NewGuid(), 42);
        _fourthItem = new(Guid.NewGuid(), 42);
        _set.Add(_secondItem);
        _set.Add(_thirdItem);
        _set.Add(_fourthItem);
        _set.Add(_firstItem);
    }

    async Task Because() => await _set.RemoveAndAddLastInQuery(_thirdItem.Id, new InMemoryFluentFind<SomeClassWithSomeId>(_queryDocuments));

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([_firstItem, _secondItem, _fourthItem, _queryDocuments.Last()], _set);
}