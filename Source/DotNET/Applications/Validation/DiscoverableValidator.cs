// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Validation;

/// <summary>
/// Represents a base validator that can be discovered and automatically hooked up.
/// </summary>
/// <typeparam name="T">Type of object the validator is for.</typeparam>
public class DiscoverableValidator<T> : BaseValidator<T>, IDiscoverableValidator<T>;
