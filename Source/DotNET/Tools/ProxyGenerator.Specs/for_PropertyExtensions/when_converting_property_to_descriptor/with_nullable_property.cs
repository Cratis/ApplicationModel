// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.for_PropertyExtensions.when_converting_property_to_descriptor;

public class with_nullable_property : Specification
{
    PropertyInfo _property;
    PropertyDescriptor _result;

    void Establish() => _property = typeof(TypeWithNullableProperty).GetProperty(nameof(TypeWithNullableProperty.NullableValue));

    void Because() => _result = _property.ToPropertyDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("NullableValue");
    [Fact] void should_have_number_type() => _result.Type.ShouldEqual("number");
    [Fact] void should_be_nullable() => _result.IsNullable.ShouldBeTrue();
}
