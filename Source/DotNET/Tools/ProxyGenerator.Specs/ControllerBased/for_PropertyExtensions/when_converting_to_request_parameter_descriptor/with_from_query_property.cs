// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_PropertyExtensions.when_converting_to_request_parameter_descriptor;

public class with_from_query_property : Specification
{
    PropertyInfo _property;
    RequestParameterDescriptor _result;

    void Establish() => _property = typeof(TypeWithRequestProperties).GetProperty(nameof(TypeWithRequestProperties.Filter))!;

    void Because() => _result = _property.ToRequestParameterDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("Filter");
    [Fact] void should_have_string_type() => _result.Type.ShouldEqual("string");
    [Fact] void should_be_query_string_parameter() => _result.IsQueryStringParameter.ShouldBeTrue();
}
