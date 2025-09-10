// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_ascending_sorting_and_paging_and_also_adding_items_with_same_id : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;
    SomeClassWithSomeId _firstItem;
    SomeClassWithSomeId _secondItem;
    SomeClassWithSomeId _thirdItem;
    SomeClassWithSomeId _fourthItem;
    SomeClassWithSomeId _fifthItem;
    SomeClassWithSomeId _sixthItem;

    void Establish()
    {
        _set = new(QueryContextBuilder.New()
            .WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Ascending))
            .WithPageSize(4)
            .Build());
        _firstItem = new(Guid.NewGuid(), 40);
        _secondItem = _firstItem with
        {
            Value = 41
        };
        _thirdItem = new(Guid.NewGuid(), 42);
        _fourthItem = new(Guid.NewGuid(), 42);
        _fifthItem = new(Guid.NewGuid(), 44);
        _sixthItem = new(Guid.NewGuid(), 45);
    }

    void Because()
    {
        _set.Add(_sixthItem);
        _set.Add(_secondItem);
        _set.Add(_thirdItem);
        _set.Add(_fifthItem);
        _set.Add(_fourthItem);
        _set.Add(_firstItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([_firstItem, _thirdItem, _fourthItem, _fifthItem], _set);
}