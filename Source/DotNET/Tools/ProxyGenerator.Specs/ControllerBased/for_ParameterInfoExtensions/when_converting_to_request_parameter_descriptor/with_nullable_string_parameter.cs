// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_ParameterInfoExtensions.when_converting_to_request_parameter_descriptor;

public class with_nullable_string_parameter : Specification
{
    ParameterInfo _parameter;
    RequestParameterDescriptor _result;

    void Establish() => _parameter = typeof(TypeWithRequest).GetConstructors()[0].GetParameters().First(_ => _.Name == "filter");

    void Because() => _result = _parameter.ToRequestParameterDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("filter");
    [Fact] void should_have_string_type() => _result.Type.ShouldEqual("string");
    [Fact] void should_be_query_string_parameter() => _result.IsQueryStringParameter.ShouldBeTrue();
}
