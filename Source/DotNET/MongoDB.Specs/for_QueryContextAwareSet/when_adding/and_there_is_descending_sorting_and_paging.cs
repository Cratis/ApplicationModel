// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_descending_sorting_and_paging : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    SomeClassWithSomeId firstItem;
    SomeClassWithSomeId secondItem;
    SomeClassWithSomeId thirdItem;
    SomeClassWithSomeId fourthItem;
    SomeClassWithSomeId fifthItem;
    SomeClassWithSomeId sixthItem;
    void Establish()
    {
        set = new(QueryContextBuilder.New()
            .WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Descending))
            .WithPageSize(4)
            .Build());
        firstItem = new(Guid.NewGuid(), 45);
        secondItem = new(Guid.NewGuid(), 44);
        thirdItem = new(Guid.NewGuid(), 43);
        fourthItem = new(Guid.NewGuid(), 43);
        fifthItem = new(Guid.NewGuid(), 42);
        sixthItem = new(Guid.NewGuid(), 41);
    }

    void Because()
    {
        set.Add(sixthItem);
        set.Add(secondItem);
        set.Add(thirdItem);
        set.Add(fifthItem);
        set.Add(fourthItem);
        set.Add(firstItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([firstItem, secondItem, thirdItem, fourthItem], set);
}
