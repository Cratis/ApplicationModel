using Cratis.Collections;
using MongoDB.Bson.Serialization;

namespace Cratis.Applications.MongoDB.for_AcronymFriendlyCamelCaseElementNameConvention;

public class when_applying_convention_to_all_class_map_members : Specification
{
    class SomeType
    {
        public string SomeProperty { get; init; }
        public string SomeOtherProperty { get; init; }
    }

    BsonClassMap<SomeType> class_map;
    AcronymFriendlyCamelCaseElementNameConvention convention;

    void Establish()
    {
        class_map = new BsonClassMap<SomeType>();
        convention = new AcronymFriendlyCamelCaseElementNameConvention();
        class_map.AutoMap();
        class_map.MapMember(_ => _.SomeOtherProperty).SetElementName("SomeOtherPropertyName");
    }

    void Because() => class_map.DeclaredMemberMaps.ForEach(convention.Apply);

    [Fact] void should_have_two_members() => class_map.DeclaredMemberMaps.Count().ShouldEqual(2);
    [Fact] void should_convert_SomeProperty_to_camelCase() => class_map.DeclaredMemberMaps.Where(_ => _.ElementName == "someProperty")
        .ShouldContainSingleItem();
    [Fact] void should_convert_SomeOtherProperty_to_someOtherPropertyName() => class_map.DeclaredMemberMaps.Where(_ => _.ElementName == "someOtherPropertyName")
        .ShouldContainSingleItem();
}