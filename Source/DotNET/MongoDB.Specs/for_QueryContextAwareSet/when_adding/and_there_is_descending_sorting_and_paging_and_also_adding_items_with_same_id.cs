// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_descending_sorting_and_paging_and_also_adding_items_with_same_id : Specification
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
        firstItem = new(Guid.NewGuid(), 40);
        secondItem = firstItem with
        {
            Value = 41
        };
        thirdItem = new(Guid.NewGuid(), 42);
        fourthItem = new(Guid.NewGuid(), 42);
        fifthItem = new(Guid.NewGuid(), 44);
        sixthItem = new(Guid.NewGuid(), 45);
    }

    void Because()
    {
        // 45, 41 a, 42, 44, 42, 40 a
        set.Add(sixthItem);
        set.Add(secondItem);
        set.Add(thirdItem);
        set.Add(fifthItem);
        set.Add(fourthItem);
        set.Add(firstItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([sixthItem, fifthItem, thirdItem, fourthItem], set);
}
