// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications;

/// <summary>
/// Represents the options for command handling.
/// </summary>
public class CommandOptions
{
    /// <summary>
    /// Gets or sets the route prefix to use for commands.
    /// </summary>
    public string RoutePrefix { get; set; } = "api";

    /// <summary>
    /// Number of segments to skip from the start of the command type's namespace when constructing the route.
    /// </summary>
    public int SegmentsToSkipForRoute { get; set; }

    /// <summary>
    /// Whether to include the command name as the last segment of the route.
    /// </summary>
    public bool IncludeCommandNameInRoute { get; set; } = true;
}

