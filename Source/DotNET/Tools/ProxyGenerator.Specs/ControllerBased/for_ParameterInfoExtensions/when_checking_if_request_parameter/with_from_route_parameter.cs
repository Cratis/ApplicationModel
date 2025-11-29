// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_ParameterInfoExtensions.when_checking_if_request_parameter;

public class with_from_route_parameter : Specification
{
    ParameterInfo _parameter;
    bool _result;

    void Establish() => _parameter = typeof(TypeWithRequest).GetConstructors()[0].GetParameters().First(_ => _.Name == "id");

    void Because() => _result = _parameter.IsRequestParameter();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
