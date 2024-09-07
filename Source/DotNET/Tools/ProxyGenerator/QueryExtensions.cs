// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator;

/// <summary>
/// Extension methods for working with commands.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// Convert a <see cref="MethodInfo"/> to a <see cref="CommandDescriptor"/>.
    /// </summary>
    /// <param name="method">Method to convert.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <returns>Converted <see cref="CommandDescriptor"/>.</returns>
    public static QueryDescriptor ToQueryDescriptor(this MethodInfo method, string targetPath, int segmentsToSkip)
    {
        var typesInvolved = new List<Type>();
        var arguments = method.GetArgumentDescriptors();
        var responseModel = ModelDescriptor.Empty;

        if (method.ReturnType.IsAssignableTo<Task>() && method.ReturnType.IsGenericType)
        {
            var responseType = method.ReturnType.GetGenericArguments()[0];
            responseModel = responseType.ToModelDescriptor();
        }
        else if (method.ReturnType != TypeExtensions._voidType && method.ReturnType != TypeExtensions._taskType)
        {
            responseModel = method.ReturnType.ToModelDescriptor();
        }

        if (!responseModel.Type.IsKnownType())
        {
            typesInvolved.Add(responseModel.Type);
        }

        var argumentsWithComplexTypes = arguments.Where(_ => !_.OriginalType.IsKnownType());
        typesInvolved.AddRange(argumentsWithComplexTypes.Select(_ => _.OriginalType));

        var imports = typesInvolved.GetImports(targetPath, method.DeclaringType!.ResolveTargetPath(segmentsToSkip), segmentsToSkip).ToList();

        var additionalTypesInvolved = new List<Type>();
        foreach (var argument in argumentsWithComplexTypes)
        {
            argument.CollectTypesInvolved(additionalTypesInvolved);
        }

        var propertyDescriptors = responseModel.Type.GetPropertyDescriptors();
        foreach (var property in propertyDescriptors)
        {
            property.CollectTypesInvolved(additionalTypesInvolved);
        }

        var route = method.GetRoute();

        return new(
            method.DeclaringType!,
            method,
            route,
            route.MakeRouteTemplate(),
            method.Name,
            responseModel.Name,
            responseModel.Constructor,
            responseModel.IsEnumerable,
            responseModel.IsObservable,
            imports.ToOrderedImports(),
            arguments,
            arguments.Where(_ => !_.IsOptional).ToList(),
            propertyDescriptors,
            [.. typesInvolved, .. additionalTypesInvolved]);
    }
}
