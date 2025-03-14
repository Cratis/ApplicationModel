using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_adding;

public class two_items_with_the_same_id_while_sorting_is_disabled : Specification
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
        set.Add(theItem);
    }

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([theItem], set);
}