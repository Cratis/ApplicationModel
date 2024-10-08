// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Represents a target type and optional import module.
/// </summary>
/// <param name="OriginalType">The original type.</param>
/// <param name="Type">Type.</param>
/// <param name="Constructor">The JavaScript constructor type.</param>
/// <param name="Module">Module the type should be imported from. default or empty means no need to import.</param>
/// <param name="Final">Whether or not it absolutely is this type and do not try to resolve a more specific one.</param>
public record TargetType(Type OriginalType, string Type, string Constructor, string Module = "", bool Final = false);
