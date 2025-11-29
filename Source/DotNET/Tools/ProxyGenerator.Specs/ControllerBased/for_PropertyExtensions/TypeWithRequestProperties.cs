// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator.ControllerBased.for_MethodInfoExtensions;

namespace Cratis.Applications.ProxyGenerator.ControllerBased.for_PropertyExtensions;

public class TypeWithRequestProperties
{
    [FromRoute]
    public int Id { get; set; }

    [FromQuery]
    public string? Filter { get; set; }

    public string Name { get; set; } = string.Empty;
}
