// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.ProxyGenerator.Templates;

/// <summary>
/// Extension methods for <see cref="ImportStatement"/>.
/// </summary>
public static class ImportStatementExtensions
{
    /// <summary>
    /// Converts the enumerable of <see cref="ImportStatement"/> to an ordered enumerable.
    /// </summary>
    /// <param name="imports">The imports.</param>
    /// <returns>The ordered enumerable.</returns>
    public static IOrderedEnumerable<ImportStatement> ToOrderedImports(this IEnumerable<ImportStatement> imports) =>
        imports.Order(new ImportStatementComparer());

    sealed class ImportStatementComparer : IComparer<ImportStatement>
    {
        public int Compare(ImportStatement? x, ImportStatement? y)
        {
            var moduleLeft = x?.Module;
            var moduleRight = y?.Module;

            if (moduleLeft == null || moduleRight == null)
            {
                return 0;
            }

            if (!IsPathImport(moduleLeft) && !IsPathImport(moduleRight))
            {
                return string.Compare(moduleLeft, moduleLeft, StringComparison.OrdinalIgnoreCase);
            }
            if (!IsPathImport(moduleLeft))
            {
                return -1;
            }
            return !IsPathImport(moduleRight)
                ? 1
                : string.Compare(GetPathImportFilename(moduleLeft), GetPathImportFilename(moduleRight), StringComparison.OrdinalIgnoreCase);
        }

        static bool IsPathImport(string module) => module.StartsWith("./") || module.StartsWith("../");
        static string GetPathImportFilename(string module) => module.Split('/')[^1];
    }
}
