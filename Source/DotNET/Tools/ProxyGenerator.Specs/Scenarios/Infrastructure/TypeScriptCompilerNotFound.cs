// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Scenarios.Infrastructure;

/// <summary>
/// The exception that is thrown when the TypeScript compiler is not found.
/// </summary>
/// <param name="path">The path where the compiler was expected.</param>
public class TypeScriptCompilerNotFound(string path)
    : Exception($"TypeScript compiler not found at '{path}'. Ensure you have run 'yarn install' in the repository root.");
