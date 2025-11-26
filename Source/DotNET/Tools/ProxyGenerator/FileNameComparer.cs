// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator;

#pragma warning disable RCS1241 // Implement IEqualityComparer when implementing IEqualityComparer<T>

/// <summary>
/// Represents a comparer for file names.
/// </summary>
public class FileNameComparer : IEqualityComparer<string>
{
    /// <inheritdoc/>
    public bool Equals(string? x, string? y)
    {
        if (x == null || y == null)
            return false;

        return StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileName(x), Path.GetFileName(y));
    }

    /// <inheritdoc/>
    public int GetHashCode(string obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        return StringComparer.OrdinalIgnoreCase.GetHashCode(Path.GetFileName(obj));
    }
}
