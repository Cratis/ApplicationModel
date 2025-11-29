// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_ParameterInfoExtensions.when_converting_to_request_parameter_descriptor;

public class with_int_parameter : Specification
{
    ParameterInfo _parameter;
    RequestParameterDescriptor _result;

    void Establish() => _parameter = typeof(TypeWithRequest).GetConstructors()[0].GetParameters().First(_ => _.Name == "id");

    void Because() => _result = _parameter.ToRequestParameterDescriptor();

    [Fact] void should_have_correct_name() => _result.Name.ShouldEqual("id");
    [Fact] void should_have_number_type() => _result.Type.ShouldEqual("number");
    [Fact] void should_not_be_optional() => _result.IsOptional.ShouldBeFalse();
    [Fact] void should_not_be_query_string_parameter() => _result.IsQueryStringParameter.ShouldBeFalse();
}
