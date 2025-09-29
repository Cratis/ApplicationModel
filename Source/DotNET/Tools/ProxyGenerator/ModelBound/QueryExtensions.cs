// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Applications.ProxyGenerator.Templates;

namespace Cratis.Applications.ProxyGenerator.ModelBound;

/// <summary>
/// Extensions for query types.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    /// Convert a query <see cref="TypeInfo"/> to a collection of <see cref="QueryDescriptor"/>.
    /// </summary>
    /// <param name="readModelType">Read model type to convert.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <param name="skipQueryNameInRoute">True if the query name should be skipped in the route, false if not.</param>
    /// <param name="apiPrefix">The API prefix to use in the route.</param>
    /// <returns>Collection of converted <see cref="QueryDescriptor"/>.</returns>
    public static IEnumerable<QueryDescriptor> ToQueryDescriptors(
        this TypeInfo readModelType,
        string targetPath,
        int segmentsToSkip,
        bool skipQueryNameInRoute,
        string apiPrefix)
    {
        var queryMethods = readModelType.GetQueryMethods();
        var descriptors = new List<QueryDescriptor>();

        foreach (var method in queryMethods)
        {
            var descriptor = method.ToQueryDescriptor(readModelType, targetPath, segmentsToSkip, skipQueryNameInRoute, apiPrefix);
            descriptors.Add(descriptor);
        }

        return descriptors;
    }

    /// <summary>
    /// Convert a static query method to a <see cref="QueryDescriptor"/>.
    /// </summary>
    /// <param name="method">Query method to convert.</param>
    /// <param name="readModelType">The read model type that contains this method.</param>
    /// <param name="targetPath">The target path the proxies are generated to.</param>
    /// <param name="segmentsToSkip">Number of segments to skip from the namespace when generating the output path.</param>
    /// <param name="skipQueryNameInRoute">True if the query name should be skipped in the route, false if not.</param>
    /// <param name="apiPrefix">The API prefix to use in the route.</param>
    /// <returns>Converted <see cref="QueryDescriptor"/>.</returns>
    public static QueryDescriptor ToQueryDescriptor(
        this MethodInfo method,
        TypeInfo readModelType,
        string targetPath,
        int segmentsToSkip,
        bool skipQueryNameInRoute,
        string apiPrefix)
    {
        var typesInvolved = new List<Type>();

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

        var arguments = method.GetQueryArgumentDescriptors();
        var properties = method.GetQueryPropertyDescriptors();

        var argumentsWithComplexTypes = arguments.Where(_ => !_.OriginalType.IsKnownType());
        typesInvolved.AddRange(argumentsWithComplexTypes.Select(_ => _.OriginalType));

        var location = readModelType.Namespace?.Split('.') ?? [];
        var segments = location.Skip(segmentsToSkip);
        var baseUrl = $"/{apiPrefix}/{string.Join('/', segments)}";
        var route = skipQueryNameInRoute ? baseUrl : $"{baseUrl}/{method.Name.ToKebabCase()}".ToLowerInvariant();

        var relativePath = readModelType.ResolveTargetPath(segmentsToSkip);
        var imports = typesInvolved
                        .GetImports(targetPath, relativePath, segmentsToSkip)
                        .DistinctBy(_ => _.Type)
                        .ToList();

        var additionalTypesInvolved = new List<Type>();
        foreach (var argument in argumentsWithComplexTypes)
        {
            argument.CollectTypesInvolved(additionalTypesInvolved);
        }

        var argumentsNeedingImportStatements = arguments.Where(_ => _.OriginalType.HasModule()).ToList();
        imports.AddRange(argumentsNeedingImportStatements.Select(_ => _.OriginalType.GetImportStatement(targetPath, relativePath, segmentsToSkip)));

        foreach (var property in responseModel.Type.GetPropertyDescriptors())
        {
            property.CollectTypesInvolved(additionalTypesInvolved);
        }

        imports = [.. imports.DistinctBy(_ => _.Type)];

        return new(
            readModelType,
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
            [.. arguments.Where(_ => !_.IsOptional)],
            properties,
            [.. typesInvolved, .. additionalTypesInvolved]);
    }

    /// <summary>
    /// Get query argument descriptors from a method - only primitives and concepts are included.
    /// </summary>
    /// <param name="method">Method to get arguments for.</param>
    /// <returns>Collection of <see cref="RequestArgumentDescriptor"/>.</returns>
    static IEnumerable<RequestArgumentDescriptor> GetQueryArgumentDescriptors(this MethodInfo method)
    {
        var parameters = method.GetParameters();

        // Include only primitive types and concepts as query parameters
        // Everything else is assumed to be a dependency
        var queryParameters = parameters.Where(p => p.ParameterType.IsAPrimitiveType() || p.ParameterType.IsConcept());

        return queryParameters.Select(p => p.ToQueryRequestArgumentDescriptor());
    }

    /// <summary>
    /// Get query property descriptors from a method - only primitives and concepts are included.
    /// </summary>
    /// <param name="method">Method to get properties for.</param>
    /// <returns>Collection of <see cref="PropertyDescriptor"/>.</returns>
    static IEnumerable<PropertyDescriptor> GetQueryPropertyDescriptors(this MethodInfo method)
    {
        var parameters = method.GetParameters();

        // Include only primitive types and concepts as query properties
        var queryParameters = parameters.Where(p => p.ParameterType.IsAPrimitiveType() || p.ParameterType.IsConcept());

        return queryParameters.Select(p => p.ToPropertyDescriptor());
    }

    /// <summary>
    /// Convert a <see cref="ParameterInfo"/> to a <see cref="RequestArgumentDescriptor"/> for queries.
    /// </summary>
    /// <param name="parameterInfo">Parameter to convert.</param>
    /// <returns>Converted <see cref="RequestArgumentDescriptor"/>.</returns>
    static RequestArgumentDescriptor ToQueryRequestArgumentDescriptor(this ParameterInfo parameterInfo)
    {
        var type = parameterInfo.ParameterType.GetTargetType();
        var optional = parameterInfo.IsOptional() || parameterInfo.HasDefaultValue;

        // All query parameters are considered query string parameters
        return new RequestArgumentDescriptor(parameterInfo.ParameterType, parameterInfo.Name!, type.Type, optional, true);
    }

    /// <summary>
    /// Check if a parameter is optional for model bound queries.
    /// </summary>
    /// <param name="parameter">Parameter to check.</param>
    /// <returns>True if it is optional, false if not.</returns>
    static bool IsOptional(this ParameterInfo parameter)
    {
        return parameter.ParameterType.IsNullable() || parameter.HasDefaultValue;
    }
}