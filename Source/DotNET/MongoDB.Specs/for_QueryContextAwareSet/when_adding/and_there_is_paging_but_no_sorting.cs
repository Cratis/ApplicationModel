// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_paging_but_no_sorting : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;
    SomeClassWithSomeId _firstItem;
    SomeClassWithSomeId _secondItem;
    SomeClassWithSomeId _thirdItem;
    void Establish()
    {
        _set = new(QueryContextBuilder.New().WithPageSize(2).Build());
        _firstItem = new(Guid.NewGuid(), 42);
        _secondItem = new(Guid.NewGuid(), 43);
        _thirdItem = new(Guid.NewGuid(), 44);
    }

    void Because()
    {
        _set.Add(_firstItem);
        _set.Add(_secondItem);
        _set.Add(_thirdItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([_firstItem, _secondItem], _set);
}