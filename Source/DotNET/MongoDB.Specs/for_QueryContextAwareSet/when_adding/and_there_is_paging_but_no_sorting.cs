using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class and_there_is_paging_but_no_sorting : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    SomeClassWithSomeId firstItem;
    SomeClassWithSomeId secondItem;
    SomeClassWithSomeId thirdItem;
    void Establish()
    {
        set = new(QueryContextBuilder.New().WithPageSize(2).Build());
        firstItem = new(Guid.NewGuid(), 42);
        secondItem = new(Guid.NewGuid(), 43);
        thirdItem = new(Guid.NewGuid(), 44);
    }

    void Because()
    {
        set.Add(firstItem);
        set.Add(secondItem);
        set.Add(thirdItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([firstItem, secondItem], set);
}