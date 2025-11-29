// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;

namespace Cratis.Arc.MongoDB.for_QueryContextAwareSet.when_adding;

public class two_items_with_the_same_id_while_sorting_is_disabled : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> _set;
    SomeClassWithSomeId _theItem;
    SomeId _id;

    void Establish()
    {
        _set = new(QueryContextBuilder.New().Build());
        _id = Guid.NewGuid();
        _theItem = new(_id, 50);
    }

    void Because()
    {
        _set.Add(new(_id, 42));
        _set.Add(_theItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([_theItem], _set);
}