// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using Cratis.Applications.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cratis.Applications.Commands;

/// <summary>
/// Represents a <see cref="IAsyncActionFilter"/> for providing a proper <see cref="CommandResult{T}"/> for post actions.
/// </summary>
public class CommandActionFilter : IAsyncActionFilter
{
    /// <inheritdoc/>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.HttpContext.Request.Method == HttpMethod.Post.Method)
        {
            var exceptionMessages = new List<string>();
            var exceptionStackTrace = string.Empty;
            ActionExecutedContext? result = null;
            object? response = null;

            var ignoreValidation = context.ShouldIgnoreValidation();

            var validationResult = ignoreValidation ?
                                        [] :
                                        context.ModelState.SelectMany(_ => _.Value!.Errors.Select(e => e.ToValidationResult(_.Key))).ToList();

            if (context.ModelState.IsValid || ignoreValidation)
            {
                var errorsBefore = context.ModelState.SelectMany(_ => _.Value!.Errors).ToArray();

                result = await next();

                AddAdditionalValidationResultsAfterActionExecute(context, validationResult, errorsBefore);

                if (context.IsAspNetResult()) return;

                if (result.Exception is not null)
                {
                    var exception = result.Exception;
                    exceptionStackTrace = exception.StackTrace;

                    do
                    {
                        exceptionMessages.Add(exception.Message);
                        exception = exception.InnerException;
                    }
                    while (exception is not null);

                    result.Exception = null!;
                }

                if (result.Result is ObjectResult objectResult)
                {
                    response = objectResult.Value;
                }
            }

            var commandResult = new CommandResult<object>
            {
                CorrelationId = context.HttpContext.GetCorrelationId(),
                ValidationResults = validationResult,
                ExceptionMessages = [.. exceptionMessages],
                ExceptionStackTrace = exceptionStackTrace ?? string.Empty,
                Response = response
            };

            if (!commandResult.IsAuthorized)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;            // Forbidden: https://www.rfc-editor.org/rfc/rfc9110.html#name-403-forbidden
            }
            else if (!commandResult.IsValid)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;           // Bad request: https://www.rfc-editor.org/rfc/rfc9110.html#name-400-bad-request
            }
            else if (commandResult.HasExceptions)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;  // Internal Server error: https://www.rfc-editor.org/rfc/rfc9110.html#name-500-internal-server-error
            }

            var actualResult = new ObjectResult(commandResult);

            if (result is not null)
            {
                result.Result = actualResult;
            }
            else
            {
                context.Result = actualResult;
            }
        }
        else
        {
            await next();
        }
    }

    void AddAdditionalValidationResultsAfterActionExecute(ActionExecutingContext context, List<ValidationResult> validationResult, IEnumerable<ModelError> errorsBefore)
    {
        foreach (var (member, entry) in context.ModelState)
        {
            foreach (var error in entry.Errors.Where(_ => !errorsBefore.Contains(_)))
            {
                validationResult.Add(error.ToValidationResult(member));
            }
        }
    }
}
