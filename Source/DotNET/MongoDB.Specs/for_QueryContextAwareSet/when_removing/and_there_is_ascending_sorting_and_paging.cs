// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_removing;

public class and_there_is_ascending_sorting_and_paging : Specification
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
            .WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Ascending))
            .WithPageSize(4)
            .Build());
        firstItem = new(Guid.NewGuid(), 40);
        secondItem = new(Guid.NewGuid(), 41);
        thirdItem = new(Guid.NewGuid(), 42);
        fourthItem = new(Guid.NewGuid(), 42);
        fifthItem = new(Guid.NewGuid(), 44);
        sixthItem = new(Guid.NewGuid(), 45);
        set.Add(sixthItem);
        set.Add(secondItem);
        set.Add(thirdItem);
        set.Add(fifthItem);
        set.Add(fourthItem);
        set.Add(firstItem);
    }

    void Because()
    {
        set.Remove(secondItem.Id);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([firstItem, thirdItem, fourthItem], set);
}