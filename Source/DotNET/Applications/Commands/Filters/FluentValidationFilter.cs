// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        if (discoverableValidators.TryGet(context.Type, out var validator))
        {
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(context.Type);
            var validationContext = Activator.CreateInstance(validationContextType, context.Command) as IValidationContext;
            var validationResult = await validator.ValidateAsync(validationContext);
            if (!validationResult.IsValid)
            {
                return new CommandResult
                {
                    CorrelationId = context.CorrelationId,
                    IsAuthorized = true,
                    ValidationResults = validationResult.Errors.Select(_ =>
                        new ValidationResult(ValidationResultSeverity.Error, _.ErrorMessage, [_.PropertyName], null!)).ToArray()
                };
            }
        }

        return CommandResult.Success(context.CorrelationId);
    }
}
