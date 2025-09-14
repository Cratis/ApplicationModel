// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

    BsonClassMap<SomeType> _classMap;

    void Establish()
    {
        _classMap = new BsonClassMap<SomeType>();
        _classMap.MapMember(_ => _.SomeProperty).SetElementName(nameof(SomeType.SomeProperty).ToCamelCase());
        _classMap.MapMember(_ => _.SomeOtherProperty).SetElementName(nameof(SomeType.SomeOtherProperty).ToCamelCase());
    }

    void Because() => _classMap.UnmapAllMembersExcept(_ => _.SomeOtherProperty);

    [Fact] void should_have_one_member() => _classMap.DeclaredMemberMaps.Count().ShouldEqual(1);
    [Fact] void should_have_the_correct_member() => _classMap.DeclaredMemberMaps.First().ElementName.ShouldEqual(nameof(SomeType.SomeOtherProperty).ToCamelCase());
}
