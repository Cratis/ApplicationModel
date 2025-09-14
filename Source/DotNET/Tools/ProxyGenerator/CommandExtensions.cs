// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Extension methods for working with commands.
/// </summary>
public static class CommandExtensions
{
    /// <summary>
    /// Convert a <see cref="MethodInfo"/> to a <see cref="CommandDescriptor"/>.
    /// </summary>
    /// <param name="method">Method to convert.</param>
    /// <param name="commandName">Name of the command.</param>
    /// <param name="properties">Properties of the command.</param>
    /// <param name="arguments">Arguments of the command.</param>
    /// <param name="route">Route of the command.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>Converted <see cref="CommandDescriptor"/>.</returns>
    public static CommandDescriptor ToCommandDescriptor(this MethodInfo method, string commandName, IEnumerable<PropertyDescriptor> properties, IEnumerable<RequestArgumentDescriptor> arguments, string route, string targetPath, int segmentsToSkip)
    {
        var (hasResponse, responseModel) = method.GetResponseModel();
        var typesInvolved = new List<Type>();

        if (!(responseModel.Type?.IsKnownType() ?? true))
        {
            typesInvolved.Add(responseModel.Type);
        }

        var propertiesWithComplexTypes = properties.Where(_ => !_.OriginalType.IsKnownType());
        var propertiesNeedingImportStatements = properties.Where(_ => _.OriginalType.HasModule()).ToList();
        typesInvolved.AddRange(propertiesWithComplexTypes.Select(_ => _.OriginalType));
        var relativePath = method.DeclaringType!.ResolveTargetPath(segmentsToSkip);
        var imports = typesInvolved.GetImports(targetPath, relativePath, segmentsToSkip).ToList();
        imports.AddRange(propertiesNeedingImportStatements.Select(_ => _.OriginalType.GetImportStatement(targetPath, relativePath, segmentsToSkip)));

        if (responseModel.Type is not null && responseModel.Type.GetTargetType().TryGetImportStatement(out var responseTypeImportStatement))
        {
            imports.Add(responseTypeImportStatement);
        }
        var additionalTypesInvolved = new List<Type>();
        foreach (var property in propertiesWithComplexTypes)
        {
            property.CollectTypesInvolved(additionalTypesInvolved);
        }

        imports = [.. imports.DistinctBy(_ => _.Type)];

        return new(
            method.DeclaringType!,
            method,
            route,
            route.MakeRouteTemplate(),
            commandName,
            properties,
            imports.OrderBy(_ => _.Module),
            arguments,
            hasResponse,
            responseModel,
            [.. typesInvolved, .. additionalTypesInvolved]);
    }
}
