// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Cratis.Applications.Validation;
#pragma warning disable CS0618 // Type or member is obsolete - Transform() in FluentValidation is marked as obsolete, but we need to use it for now. We will remove this when we upgrade to a newer version of FluentValidation.

/// <summary>
/// Extensions for working with <see cref="IRuleBuilderOptions{T, TProperty}"/>.
/// </summary>
public static class RuleBuilderOptionsExtensions
{
    /// <summary>
    /// Specifies a condition for when the context is a command.
    /// </summary>
    /// <param name="rule">The current rule.</param>
    /// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain. Default: All.</param>
    /// <typeparam name="T">Type of object being validated.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public static IRuleBuilderOptions<T, TProperty> WhenCommand<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) =>
        rule.When((T x, ValidationContext<T> ctx) => ctx.IsCommand(), applyConditionTo);

    /// <summary>
    /// Specifies a condition for when the context is a query.
    /// </summary>
    /// <param name="rule">The current rule.</param>
    /// <param name="applyConditionTo">Whether the condition should be applied to the current rule or all rules in the chain. Default: All.</param>
    /// <typeparam name="T">Type of object being validated.</typeparam>
    /// <typeparam name="TProperty">Property type.</typeparam>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public static IRuleBuilderOptions<T, TProperty> WhenQuery<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ApplyConditionTo applyConditionTo = ApplyConditionTo.AllValidators) =>
        rule.When((T x, ValidationContext<T> ctx) => ctx.IsQuery(), applyConditionTo);
}
