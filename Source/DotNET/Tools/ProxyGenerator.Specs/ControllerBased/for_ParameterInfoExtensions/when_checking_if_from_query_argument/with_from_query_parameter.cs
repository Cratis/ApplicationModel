// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_ParameterInfoExtensions.when_checking_if_from_query_argument;

public class with_from_query_parameter : Specification
{
    ParameterInfo _parameter;
    bool _result;

    void Establish() => _parameter = typeof(TypeWithRequest).GetConstructors()[0].GetParameters().First(_ => _.Name == "filter");

    void Because() => _result = _parameter.IsFromQueryArgument();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
