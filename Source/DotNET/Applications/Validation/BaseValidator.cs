// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using FluentValidation;

namespace Cratis.Applications.Validation;

#pragma warning disable CS0618 // Type or member is obsolete (Related to FluentValidation and the Transform method)
/// <summary>
/// Represents a base validator that we use for discovery.
/// </summary>
/// <typeparam name="T">Type of object the validator is for.</typeparam>
public class BaseValidator<T> : AbstractValidator<T>
{
    /// <summary>
    /// Define a condition for when the context is a command.
    /// </summary>
    /// <param name="callback">Callback for defining the rules when it is a command.</param>
    /// <returns><see cref="IConditionBuilder"/> for building on the condition.</returns>
    public IConditionBuilder WhenCommand(Action callback) => When((model, context) => context.IsCommand(), callback);

    /// <summary>
    /// Define a condition for when the context is a command.
    /// </summary>
    /// <param name="callback">Callback for defining the rules when it is a command.</param>
    /// <returns><see cref="IConditionBuilder"/> for building on the condition.</returns>
    public IConditionBuilder WhenQuery(Action callback) => When((model, context) => context.IsQuery(), callback);

    /// <summary>
    /// Defines a validation rules for a property based on <see cref="ConceptAs{T}"/> for the actual concept type.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <typeparam name="TProperty">Type of the concept.</typeparam>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, TProperty> RuleForConcept<TProperty>(Expression<Func<T, TProperty>> expression) => RuleFor(expression);

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for string.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, string> RuleFor(Expression<Func<T, ConceptAs<string>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for bool.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, bool> RuleFor(Expression<Func<T, ConceptAs<bool>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for Guid.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, Guid> RuleFor(Expression<Func<T, ConceptAs<Guid>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for DateOnly.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, DateOnly> RuleFor(Expression<Func<T, ConceptAs<DateOnly>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for TimeOnly.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, TimeOnly> RuleFor(Expression<Func<T, ConceptAs<TimeOnly>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for DateTime.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, DateTime> RuleFor(Expression<Func<T, ConceptAs<DateTime>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for DateTimeOffset.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, DateTimeOffset> RuleFor(Expression<Func<T, ConceptAs<DateTimeOffset>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for float.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, float> RuleFor(Expression<Func<T, ConceptAs<float>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for double.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, double> RuleFor(Expression<Func<T, ConceptAs<double>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for decimal.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, decimal> RuleFor(Expression<Func<T, ConceptAs<decimal>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for sbyte.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, sbyte> RuleFor(Expression<Func<T, ConceptAs<sbyte>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for short.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, short> RuleFor(Expression<Func<T, ConceptAs<short>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for int.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, int> RuleFor(Expression<Func<T, ConceptAs<int>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for long.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, long> RuleFor(Expression<Func<T, ConceptAs<long>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for byte.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, byte> RuleFor(Expression<Func<T, ConceptAs<byte>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for ushort.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, ushort> RuleFor(Expression<Func<T, ConceptAs<ushort>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for uint.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, uint> RuleFor(Expression<Func<T, ConceptAs<uint>>> expression) =>
        RuleFor(CreateTransform(expression));

    /// <summary>
    /// Defines a validation rules for a specific property based on <see cref="ConceptAs{T}"/> for ulong.
    /// </summary>
    /// <param name="expression">The expression representing the property to validate.</param>
    /// <returns>An IRuleBuilder instance on which validators can be defined.</returns>
    public IRuleBuilderInitial<T, ulong> RuleFor(Expression<Func<T, ConceptAs<ulong>>> expression) =>
        RuleFor(CreateTransform(expression));

    static Expression<Func<T, TProperty>> CreateTransform<TProperty>(Expression<Func<T, ConceptAs<TProperty>>> expression)
        where TProperty : IComparable
        => arg => expression.Compile().Invoke(arg).Value;
}
#pragma warning restore CS0618 // Type or member is obsolete