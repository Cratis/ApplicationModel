// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator.ControllerBased.for_MethodInfoExtensions;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_ParameterInfoExtensions;

public class TypeWithRequest([FromRoute] int id, [FromQuery] string? filter)
{
    public int Id { get; set; } = id;
    public string? Filter { get; set; } = filter;
}
