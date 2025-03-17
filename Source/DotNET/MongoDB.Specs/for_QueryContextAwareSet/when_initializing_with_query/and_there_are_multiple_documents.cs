using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_initializing_with_query;

public class and_there_are_multiple_documents : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    IEnumerable<SomeClassWithSomeId> documents;

    void Establish()
    {
        documents = [new(Guid.NewGuid(), 42), new(Guid.NewGuid(), 44),new(Guid.NewGuid(), 43), new(Guid.NewGuid(), 429), new(Guid.NewGuid(), 1)];
        set = new(QueryContextBuilder.New().Build());
    }

    void Because()
    {
        set.InitializeWithQuery(new InMemoryFluentFind<SomeClassWithSomeId>(documents));
    }

    [Fact] void should_have_same_sequence() => Assert.Equal(documents, set);
}