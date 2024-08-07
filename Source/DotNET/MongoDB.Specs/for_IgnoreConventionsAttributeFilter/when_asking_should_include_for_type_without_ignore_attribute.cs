// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Applications.MongoDB.for_IgnoreConventionsAttributeFilter;

public class when_asking_should_include_for_type_without_ignore_attribute : Specification
{
    record TheType();

    IgnoreConventionsAttributeFilter filter;

    bool result;

    void Establish() => filter = new();

    void Because() => result = filter.ShouldInclude("SomePack", Substitute.For<IConventionPack>(), typeof(TheType));

    [Fact] void should_include_it() => result.ShouldBeTrue();
}
