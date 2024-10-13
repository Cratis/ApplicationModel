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
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>Converted <see cref="CommandDescriptor"/>.</returns>
    public static CommandDescriptor ToCommandDescriptor(this MethodInfo method, string targetPath, int segmentsToSkip)
    {
        var typesInvolved = new List<Type>();
        var properties = method.GetPropertyDescriptors();
        var hasResponse = false;
        var responseModel = ModelDescriptor.Empty;

        if (method.ReturnType.IsAssignableTo<Task>() && method.ReturnType.IsGenericType)
        {
            hasResponse = true;
            var responseType = method.ReturnType.GetGenericArguments()[0];
            responseModel = responseType.ToModelDescriptor();
        }
        else if (method.ReturnType != TypeExtensions._voidType && method.ReturnType != TypeExtensions._taskType)
        {
            hasResponse = true;
            responseModel = method.ReturnType.ToModelDescriptor();
        }

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

        imports = imports.DistinctBy(_ => _.Type).ToList();

        var route = method.GetRoute();

        return new(
            method.DeclaringType!,
            method,
            route,
            route.MakeRouteTemplate(),
            method.Name,
            properties,
            imports.OrderBy(_ => _.Module),
            method.GetArgumentDescriptors(),
            hasResponse,
            responseModel,
            [.. typesInvolved, .. additionalTypesInvolved]);
    }
}
