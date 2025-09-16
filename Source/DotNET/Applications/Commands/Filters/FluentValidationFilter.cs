// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.Validation;
using FluentValidation;

namespace Cratis.Applications.Commands.Filters;

/// <summary>
/// Represents a command filter that validates commands before they are handled.
/// </summary>
/// <param name="discoverableValidators">The <see cref="IDiscoverableValidators"/> to use for finding validators.</param>
public class FluentValidationFilter(IDiscoverableValidators discoverableValidators) : ICommandFilter
{
    /// <inheritdoc/>
    public async Task<CommandResult> OnExecution(CommandContext context)
    {
        var commandResult = CommandResult.Success(context.CorrelationId);
        commandResult.MergeWith(await Validate(context, context.Command));
        return commandResult;
    }

    async Task<CommandResult> Validate(CommandContext context, object instance)
    {
        var commandResult = CommandResult.Success(context.CorrelationId);

        var instanceType = instance.GetType();
        if (discoverableValidators.TryGet(instanceType, out var validator))
        {
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(instance.GetType());
            var validationContext = Activator.CreateInstance(validationContextType, instance) as IValidationContext;
            var validationResult = await validator.ValidateAsync(validationContext);
            if (!validationResult.IsValid)
            {
                commandResult.MergeWith(new CommandResult
                {
                    ValidationResults = validationResult.Errors.Select(_ =>
                        new ValidationResult(ValidationResultSeverity.Error, _.ErrorMessage, [_.PropertyName], null!)).ToArray()
                });
            }

            if (!instanceType.IsPrimitive)
            {
                foreach (var property in instanceType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    commandResult.MergeWith(await Validate(context, property.GetValue(instance)!));
                }
            }
        }

        return commandResult;
    }
}
