// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Cratis.DependencyInjection;
using Cratis.Execution;
using Microsoft.Extensions.DependencyInjection;
using OneOf;

namespace Cratis.Applications.Commands;

/// <summary>
/// Represents an implementation of <see cref="ICommandPipeline"/>.
/// </summary>
/// <param name="correlationIdAccessor">The <see cref="ICorrelationIdAccessor"/> to use for accessing correlation IDs.</param>
/// <param name="commandFilters">The <see cref="ICommandFilters"/> to use for filtering commands.</param>
/// <param name="handlerProviders">The <see cref="ICommandHandlerProviders"/> to use for finding command handlers.</param>
/// <param name="valueHandlers">The <see cref="ICommandResponseValueHandlers"/> to use for handling response values.</param>
/// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use for resolving dependencies.</param>
[Singleton]
public class CommandPipeline(
    ICorrelationIdAccessor correlationIdAccessor,
    ICommandFilters commandFilters,
    ICommandHandlerProviders handlerProviders,
    ICommandResponseValueHandlers valueHandlers,
    IServiceProvider serviceProvider) : ICommandPipeline
{
    /// <inheritdoc/>
    public async Task<CommandResult> Execute(object command)
    {
        var correlationId = GetCorrelationId();
        var result = CommandResult.Success(correlationId);
        try
        {
            handlerProviders.TryGetHandlerFor(command, out var commandHandler);
            if (commandHandler is null)
            {
                return CommandResult.MissingHandler(correlationId, command.GetType());
            }

            var dependencies = commandHandler.Dependencies.Select(serviceProvider.GetRequiredService);
            var commandContext = new CommandContext(correlationId, command.GetType(), command, dependencies);
            result = await commandFilters.OnExecution(commandContext);
            if (!result.IsSuccess)
            {
                return result;
            }

            var response = await commandHandler.Handle(commandContext);
            if (response is not null)
            {
                if (response is ITuple tuple)
                {
                    var actualResponse = tuple[0];
                    var commandResultType = typeof(CommandResult<>).MakeGenericType(actualResponse?.GetType() ?? typeof(object));
                    result = (Activator.CreateInstance(commandResultType, actualResponse) as CommandResult)!;
                    if (tuple.Length > 1 && tuple[1] is not null)
                    {
                        response = tuple[1]!;
                    }
                }

                if (response is IOneOf oneOf)
                {
                    result.MergeWith(await valueHandlers.Handle(commandContext, oneOf.Value));
                }
                else
                {
                    result.MergeWith(await valueHandlers.Handle(commandContext, response));
                }
            }
        }
        catch (Exception ex)
        {
            result.MergeWith(CommandResult.Error(correlationId, ex));
        }

        return result;
    }

    CorrelationId GetCorrelationId()
    {
        var correlationId = correlationIdAccessor.Current;
        if (correlationId == CorrelationId.NotSet)
        {
            correlationId = CorrelationId.New();
        }

        return correlationId;
    }
}
