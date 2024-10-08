// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Cratis.Applications.Validation;

/// <summary>
/// Represents a <see cref="IModelValidator"/> for <see cref="BaseValidator{T}"/>.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DiscoverableModelValidator"/> class.
/// </remarks>
/// <param name="validator">The <see cref="IValidator"/> to use.</param>
public class DiscoverableModelValidator(IValidator validator) : IModelValidator
{
    /// <inheritdoc/>
    public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
    {
        var failures = new List<ModelValidationResult>();
        if (context.Model is not null)
        {
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(context.ModelMetadata.ModelType);
            var validationContext = (Activator.CreateInstance(validationContextType, [context.Model!]) as IValidationContext)!;

            SetValidationType(context, validationContext);

            var result = validator.ValidateAsync(validationContext).GetAwaiter().GetResult();
            failures.AddRange(result.Errors.Select(x => new ModelValidationResult(x.PropertyName, x.ErrorMessage)));
        }
        return failures;
    }

    void SetValidationType(ModelValidationContext context, IValidationContext validationContext)
    {
        if (context.ActionContext.HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            validationContext.SetCommand();
        }
        else
        {
            validationContext.SetQuery();
        }
    }
}
