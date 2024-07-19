using Cratis.Strings;
using MongoDB.Bson.Serialization;

namespace Cratis.Applications.MongoDB.for_BsonClassMapExtensions.when_unmapping_all_except;

public class and_members_one_wants_to_keep_are_camel_cased : Specification
{
    class SomeType
    {
        public string SomeProperty { get; init; }
        public string SomeOtherProperty { get; init; }
    }

    BsonClassMap<SomeType> class_map;

    void Establish()
    {
        class_map = new BsonClassMap<SomeType>();
        class_map.MapMember(_ => _.SomeProperty).SetElementName(nameof(SomeType.SomeProperty).ToCamelCase());
        class_map.MapMember(_ => _.SomeOtherProperty).SetElementName(nameof(SomeType.SomeOtherProperty).ToCamelCase());
    }

    void Because() => class_map.UnmapAllMembersExcept(_ => _.SomeOtherProperty);

    [Fact] void should_have_one_member() => class_map.DeclaredMemberMaps.Count().ShouldEqual(1);
    [Fact] void should_have_the_correct_member() => class_map.DeclaredMemberMaps.First().ElementName.ShouldEqual(nameof(SomeType.SomeOtherProperty).ToCamelCase());
}
