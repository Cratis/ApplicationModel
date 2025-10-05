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
/// <param name="contextModifier">The <see cref="ICommandContextModifier"/> to use for setting the current command context.</param>
/// <param name="contextValuesBuilder">The <see cref="ICommandContextValuesBuilder"/> to use for building command context values.</param>
/// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use for resolving dependencies.</param>
[Singleton]
public class CommandPipeline(
    ICorrelationIdAccessor correlationIdAccessor,
    ICommandFilters commandFilters,
    ICommandHandlerProviders handlerProviders,
    ICommandResponseValueHandlers valueHandlers,
    ICommandContextModifier contextModifier,
    ICommandContextValuesBuilder contextValuesBuilder,
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
            var commandContext = new CommandContext(
                correlationId,
                command.GetType(),
                command,
                dependencies,
                contextValuesBuilder.Build());
            contextModifier.SetCurrent(commandContext);
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
                    var tupleResult = ProcessTuple(tuple, commandContext);

                    if (tupleResult.ResponseValue is not null)
                    {
                        var commandResultType = typeof(CommandResult<>).MakeGenericType(tupleResult.ResponseValue.GetType());
                        commandContext = commandContext with { Response = tupleResult.ResponseValue };
                        result = (Activator.CreateInstance(commandResultType, tupleResult.ResponseValue) as CommandResult)!;
                    }

                    foreach (var valueToHandle in tupleResult.ValuesToHandle)
                    {
                        if (valueToHandle is IOneOf oneOf)
                        {
                            result.MergeWith(await valueHandlers.Handle(commandContext, oneOf.Value));
                        }
                        else
                        {
                            result.MergeWith(await valueHandlers.Handle(commandContext, valueToHandle));
                        }
                    }
                }
                else if (response is IOneOf oneOf)
                {
                    if (valueHandlers.CanHandle(commandContext, oneOf.Value))
                    {
                        result.MergeWith(await valueHandlers.Handle(commandContext, oneOf.Value));
                    }
                    else
                    {
                        commandContext = commandContext with { Response = oneOf.Value };
                        result = CreateCommandResultWithResponse(oneOf.Value);
                    }
                }
                else if (valueHandlers.CanHandle(commandContext, response))
                {
                    result.MergeWith(await valueHandlers.Handle(commandContext, response));
                }
                else
                {
                    commandContext = commandContext with { Response = response };
                    result = CreateCommandResultWithResponse(response);
                }
            }
        }
        catch (Exception ex)
        {
            result.MergeWith(CommandResult.Error(correlationId, ex));
        }

        return result;
    }

    (object? ResponseValue, IEnumerable<object> ValuesToHandle) ProcessTuple(ITuple tuple, CommandContext commandContext)
    {
        var allValues = new List<object>();
        for (var i = 0; i < tuple.Length; i++)
        {
            if (tuple[i] is not null)
            {
                allValues.Add(tuple[i]!);
            }
        }

        var handledValues = new List<object>();
        var unhandledValues = new List<object>();

        foreach (var value in allValues)
        {
            if (valueHandlers.CanHandle(commandContext, value))
            {
                handledValues.Add(value);
            }
            else
            {
                unhandledValues.Add(value);
            }
        }

        if (unhandledValues.Count > 1)
        {
            throw new MultipleUnhandledTupleValues(unhandledValues);
        }

        var responseValue = unhandledValues.Count == 1 ? unhandledValues[0] : null;

        return (responseValue, handledValues);
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

    CommandResult CreateCommandResultWithResponse(object response)
    {
        var commandResultType = typeof(CommandResult<>).MakeGenericType(response.GetType());
        return (Activator.CreateInstance(commandResultType, response) as CommandResult)!;
    }
}
