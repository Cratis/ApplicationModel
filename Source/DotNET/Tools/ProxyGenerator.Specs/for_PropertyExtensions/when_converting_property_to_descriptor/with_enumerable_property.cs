// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.for_PropertyExtensions.when_converting_property_to_descriptor;

public class with_enumerable_property : Specification
{
    PropertyInfo _property;
    PropertyDescriptor _result;

    void Establish() => _property = typeof(TypeWithEnumerableProperty).GetProperty(nameof(TypeWithEnumerableProperty.Items));

    void Because() => _result = _property.ToPropertyDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("Items");
    [Fact] void should_have_string_type() => _result.Type.ShouldEqual("string");
    [Fact] void should_be_enumerable() => _result.IsEnumerable.ShouldBeTrue();
}
