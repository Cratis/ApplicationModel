using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Applications.MongoDB.for_BsonClassMapExtensions;

public class when_applying_conventions : Specification
{
    class SomeType
    {
        public string SomeProperty { get; init; }
        public string SomeOtherProperty { get; init; }
    }

    BsonClassMap<SomeType> class_map;
    Mock<IMemberMapConvention> convention;

    void Establish()
    {
        class_map = new BsonClassMap<SomeType>();
        class_map.AutoMap();
        convention = new Mock<IMemberMapConvention>();
        ConventionRegistry.Register(Guid.NewGuid().ToString(), new ConventionPack { convention.Object }, type => type == typeof(SomeType));
    }

    void Because() => class_map.ApplyConventions();

    [Fact] void should_have_two_members() => class_map.DeclaredMemberMaps.Count().ShouldEqual(2);
    [Fact] void should_call_convention_for_first_property() => convention.Verify(_ => _.Apply(class_map.DeclaredMemberMaps.First()), Once);
    [Fact] void should_call_convention_for_second_property() => convention.Verify(_ => _.Apply(class_map.DeclaredMemberMaps.ToArray()[1]), Once);
}