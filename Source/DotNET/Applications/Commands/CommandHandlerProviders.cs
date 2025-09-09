// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Cratis.DependencyInjection;
using Cratis.Types;

namespace Cratis.Applications.Commands;

/// <summary>
/// Represents an implementation of <see cref="ICommandHandlerProviders"/>.
/// </summary>
/// <param name="providers">The collection of <see cref="ICommandHandlerProvider"/> to use for providing command handlers.</param>
[Singleton]
public class CommandHandlerProviders(IInstancesOf<ICommandHandlerProvider> providers) : ICommandHandlerProviders
{
    /// <inheritdoc/>
    public IEnumerable<ICommandHandler> Handlers => providers.SelectMany(p => p.Handlers);

    /// <inheritdoc/>
    public bool TryGetHandlerFor(object command, [NotNullWhen(true)] out ICommandHandler? handler)
    {
        handler = Handlers.FirstOrDefault(h => h.CommandType == command.GetType());
        return handler is not null;
    }
}