// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc;

/// <summary>
/// Extension methods for <see cref="IArcBuilder"/> for adding Chronicle support.
/// </summary>
public static class ChronicleBuilderExtensions
{
    /// <summary>
    /// Adds ApplicationModel support to Chronicle configuration.
    /// </summary>
    /// <param name="builder">The <see cref="IArcBuilder"/> to add to.</param>
    /// <returns><see cref="IArcBuilder"/> for continuation.</returns>
    public static IArcBuilder WithChronicle(this IArcBuilder builder)
    {
        // builder.Services.AddAggregateRoots(builder.ClientArtifactsProvider);
        // builder.Services.AddReadModels(builder.ClientArtifactsProvider);
        return builder;
    }
}
