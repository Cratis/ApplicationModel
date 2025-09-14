// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Applications.MongoDB.for_IgnoreConventionsAttributeFilter;

public class when_asking_should_include_for_type_with_ignore_attribute_ignoring_all : Specification
{
    [IgnoreConventions]
    record TheType();

    IgnoreConventionsAttributeFilter _filter;

    bool _result;

    void Establish() => _filter = new();

    void Because() => _result = _filter.ShouldInclude("SomePack", Substitute.For<IConventionPack>(), typeof(TheType));

    [Fact] void should_not_include_it() => _result.ShouldBeFalse();
}
