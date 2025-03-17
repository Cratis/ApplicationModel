using MongoDB.Driver;
using SortDirection = Cratis.Applications.Queries.SortDirection;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_remove_and_add_last_in_query;

public class and_there_are_multiple_documents_in_the_query : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;

    SomeClassWithSomeId firstItem;
    SomeClassWithSomeId secondItem;
    SomeClassWithSomeId thirdItem;
    SomeClassWithSomeId fourthItem;
    IEnumerable<SomeClassWithSomeId> queryDocuments;

    void Establish()
    {
        set = new(QueryContextBuilder.New()
            .WithSorting(new(nameof(SomeClassWithSomeId.Value), SortDirection.Ascending))
            .WithPageSize(4)
            .Build());
        queryDocuments = [new(Guid.NewGuid(), 49), new(Guid.NewGuid(), 46), new(Guid.NewGuid(), 52)];
        firstItem = new(Guid.NewGuid(), 40);
        secondItem = new(Guid.NewGuid(), 41);
        thirdItem = new(Guid.NewGuid(), 42);
        fourthItem = new(Guid.NewGuid(), 42);
        set.Add(secondItem);
        set.Add(thirdItem);
        set.Add(fourthItem);
        set.Add(firstItem);
    }

    async Task Because() => await set.RemoveAndAddLastInQuery(thirdItem.Id, new InMemoryFluentFind<SomeClassWithSomeId>(queryDocuments));

    [Fact] void should_have_items_in_correct_order() => Assert.Equal([firstItem, secondItem, fourthItem, queryDocuments.Last()], set);
}