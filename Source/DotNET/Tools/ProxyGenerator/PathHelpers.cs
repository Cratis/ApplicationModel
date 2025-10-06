// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Provides helper methods for working with file paths.
/// </summary>
public static class PathHelpers
{
    /// <summary>
    /// Normalize a path to use the correct directory separator for the current platform.
    /// </summary>
    /// <param name="path">Path to normalize.</param>
    /// <returns>Normalized path.</returns>
    public static string Normalize(string path)
    {
        var correctSeparator = Path.DirectorySeparatorChar;
        var wrongSeparator = correctSeparator == '/' ? '\\' : '/';
        path = path.Replace(wrongSeparator, correctSeparator);
        return Path.GetFullPath(path);
    }
}