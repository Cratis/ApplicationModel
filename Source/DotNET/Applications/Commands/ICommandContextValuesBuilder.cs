// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands;

/// <summary>
/// Defines a builder for creating instances of <see cref="CommandContextValues"/>.
/// </summary>
public interface ICommandContextValuesBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="CommandContextValues"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="CommandContextValues"/>.</returns>
    CommandContextValues Build();
}
