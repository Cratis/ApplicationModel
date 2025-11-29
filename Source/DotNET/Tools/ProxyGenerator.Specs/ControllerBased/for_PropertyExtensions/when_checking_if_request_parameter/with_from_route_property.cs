// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_PropertyExtensions.when_checking_if_request_parameter;

public class with_from_route_property : Specification
{
    PropertyInfo _property;
    bool _result;

    void Establish() => _property = typeof(TypeWithRequestProperties).GetProperty(nameof(TypeWithRequestProperties.Id))!;

    void Because() => _result = _property.IsRequestParameter();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
