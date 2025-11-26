// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Cratis.Applications.Identity;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable MA0048 // File name must match type name

internal static partial class MicrosoftIDentityPlatformAuthHandlerLogMessages
{
    [LoggerMessage(LogLevel.Error, "Failed resolving client principal - value was {Principal}")]
    internal static partial void FailedResolvingClientPrincipal(this ILogger<MicrosoftIDentityPlatformAuthHandler> logger, string principal, Exception exception);
}