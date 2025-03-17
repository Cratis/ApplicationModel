using MongoDB.Driver;
namespace Cratis.Applications.MongoDB.for_QueryContextAwareSet.when_initializing_with_query;

public class and_there_are_no_documents : Specification
{
    QueryContextAwareSet<SomeClassWithSomeId> set;
    void Establish()
    {
        set = new(QueryContextBuilder.New().Build());
    }

    void Because()
    {
        set.InitializeWithQuery(new InMemoryFluentFind<SomeClassWithSomeId>([]));
    }

    [Fact] void should_have_no_documents() => Assert.Equal([], set);
}