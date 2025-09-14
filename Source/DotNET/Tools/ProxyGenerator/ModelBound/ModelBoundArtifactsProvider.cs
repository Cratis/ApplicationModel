// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ModelBound;

/// <summary>
/// Represents an implementation of <see cref="IArtifactsProvider"/> that provides model bound artifacts.
/// </summary>
public class ModelBoundArtifactsProvider : IArtifactsProvider
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelBoundArtifactsProvider"/> class.
    /// </summary>
    /// <param name="message">Logger to use for outputting messages.</param>
    /// <param name="outputPath">Output path for command.</param>
    /// <param name="segmentsToSkip">Segments to skip when generating output paths.</param>
    /// <param name="skipCommandNameInRoute">True if the command name should be skipped in the route, false if not.</param>
    /// <param name="apiPrefix">The API prefix to use in the route.</param>
    public ModelBoundArtifactsProvider(
        Action<string> message,
        string outputPath,
        int segmentsToSkip,
        bool skipCommandNameInRoute,
        string apiPrefix)
    {
        message($"  Discover model based commands and queries from {TypeExtensions.Assemblies.Count()} assemblies");

        var commands = TypeExtensions.Assemblies.SelectMany(_ => _.DefinedTypes).Where(__ => __.IsCommand()).ToArray();
        Commands = commands.Select(_ => _.ToCommandDescriptor(outputPath, segmentsToSkip, skipCommandNameInRoute, apiPrefix)).ToArray();
    }

    /// <inheritdoc/>
    public IEnumerable<CommandDescriptor> Commands { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<QueryDescriptor> Queries => [];
}
