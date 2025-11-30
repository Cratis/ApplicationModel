// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Scenarios.for_Commands;

/// <summary>
/// The exception that is thrown when command execution fails intentionally for testing.
/// </summary>
/// <param name="message">The error message.</param>
public class CommandExecutionFailed(string message) : Exception(message);
