// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Templates;

/// <summary>
/// Describes an import statement.
/// </summary>
public record ImportStatement
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImportStatement"/> class.
    /// </summary>
    /// <param name="originalType">The original type of the property.</param>
    /// <param name="type">Type to use.</param>
    /// <param name="module">Source module the type is originating from.</param>
    public ImportStatement(Type originalType, string type, string module)
    {
        OriginalType = originalType;
        Type = type;
        Module = module.Replace('\\', '/');
    }

    /// <summary>
    /// Gets the original type.
    /// </summary>
    public Type OriginalType { get; }

    /// <summary>
    /// Gets the type to use.
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Gets the source module in which the type is from.
    /// </summary>
    public string Module { get; }
}
