// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class many_items_with_the_same_id_while_sorting_is_disabled : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    SomeClassWithSomeId theItem;
    SomeId id;

    void Establish()
    {
        set = new(QueryContextBuilder.New().Build());
        id = Guid.NewGuid();
        theItem = new(id, 50);
    }

    void Because()
    {
        set.Add(new(id, 42));
        set.Add(new(id, 41));
        set.Add(new(id, 44));
        set.Add(new(id, 40));
        set.Add(new(id, 46));
        set.Add(new(id, 49));
        set.Add(theItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([theItem], set);
}