// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Cratis.Arc.ProxyGenerator.ControllerBased.for_PropertyExtensions.when_checking_if_request_parameter;

public class with_property_without_attribute : Specification
{
    PropertyInfo _property;
    bool _result;

    void Establish() => _property = typeof(TypeWithRequestProperties).GetProperty(nameof(TypeWithRequestProperties.Name))!;

    void Because() => _result = _property.IsRequestParameter();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
