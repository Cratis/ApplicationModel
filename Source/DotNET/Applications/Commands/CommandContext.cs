// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Execution;
using Microsoft.AspNetCore.Authorization;

namespace Cratis.Applications.Commands;

/// <summary>
/// Represents the context for a command being executed.
/// </summary>
/// <param name="CorrelationId">The correlation ID for the command.</param>
/// <param name="Type">The type of the command.</param>
/// <param name="Command">The command instance.</param>
/// <param name="Dependencies">The dependencies required to handle the command.</param>
/// <param name="Values">A set of values associated with the command context.</param>
/// <param name="Response">The optional response from handling the command, if any.</param>
[Authorize(Roles = "Administrator")]
public record CommandContext(CorrelationId CorrelationId, Type Type, object Command, IEnumerable<object> Dependencies, CommandContextValues Values, object? Response = default);
