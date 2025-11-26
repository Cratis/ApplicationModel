// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Arc.MongoDB.for_BsonClassMapExtensions;

public class when_applying_conventions : Specification
{
    class SomeType
    {
        public string SomeProperty { get; init; }
        public string SomeOtherProperty { get; init; }
    }

    BsonClassMap<SomeType> _classMap;
    IMemberMapConvention _convention;

    void Establish()
    {
        _classMap = new BsonClassMap<SomeType>();
        _classMap.AutoMap();
        _convention = Substitute.For<IMemberMapConvention>();
        ConventionRegistry.Register(Guid.NewGuid().ToString(), new ConventionPack { _convention }, type => type == typeof(SomeType));
    }

    void Because() => _classMap.ApplyConventions();

    [Fact] void should_have_two_members() => _classMap.DeclaredMemberMaps.Count().ShouldEqual(2);
    [Fact] void should_call_convention_for_first_property() => _convention.Received(1).Apply(_classMap.DeclaredMemberMaps.First());
    [Fact] void should_call_convention_for_second_property() => _convention.Received(1).Apply(_classMap.DeclaredMemberMaps.ToArray()[1]);
}