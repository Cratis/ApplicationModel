using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_ascending_sorting_but_no_paging : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    SomeClassWithSomeId firstItem;
    SomeClassWithSomeId secondItem;
    SomeClassWithSomeId thirdItem;
    void Establish()
    {
        set = new(QueryContextBuilder.New().WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Ascending)).Build());
        firstItem = new(Guid.NewGuid(), 42);
        secondItem = new(Guid.NewGuid(), 43);
        thirdItem = new(Guid.NewGuid(), 44);
    }

    void Because()
    {
        set.Add(secondItem);
        set.Add(firstItem);
        set.Add(thirdItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([firstItem, secondItem, thirdItem], set);
}