// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Defines a provider for commands.
/// </summary>
public interface IArtifactsProvider
{
    /// <summary>
    /// Get all commands.
    /// </summary>
    IEnumerable<CommandDescriptor> Commands { get; }

    /// <summary>
    /// Get all queries.
    /// </summary>
    IEnumerable<QueryDescriptor> Queries { get; }
}
