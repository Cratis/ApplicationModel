// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_PropertyExtensions.when_converting_to_request_parameter_descriptor;

public class with_from_route_property : Specification
{
    PropertyInfo _property;
    RequestParameterDescriptor _result;

    void Establish() => _property = typeof(TypeWithRequestProperties).GetProperty(nameof(TypeWithRequestProperties.Id))!;

    void Because() => _result = _property.ToRequestParameterDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("Id");
    [Fact] void should_have_number_type() => _result.Type.ShouldEqual("number");
    [Fact] void should_not_be_optional() => _result.IsOptional.ShouldBeFalse();
    [Fact] void should_not_be_query_string_parameter() => _result.IsQueryStringParameter.ShouldBeFalse();
}
