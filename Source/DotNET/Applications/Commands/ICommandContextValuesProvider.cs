// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Commands;

/// <summary>
/// Provides access to the command context values.
/// </summary>
public interface ICommandContextValuesProvider
{
    /// <summary>
    /// Gets the command context values.
    /// </summary>
    /// <returns>The command context values.</returns>
    CommandContextValues Provide();
}
