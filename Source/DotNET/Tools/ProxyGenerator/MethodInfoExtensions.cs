// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Cratis.Arc.ProxyGenerator.Templates;

namespace Cratis.Arc.ProxyGenerator;

/// <summary>
/// Extension methods for working with <see cref="MethodInfo"/>.
/// </summary>
public static class MethodInfoExtensions
{
    /// <summary>
    /// Get the response model for a method.
    /// </summary>
    /// <param name="method">Method to inspect.</param>
    /// <returns>Tuple indicating if there is a response and the response model.</returns>
    public static (bool HasResponse, ModelDescriptor ResponseModel) GetResponseModel(this MethodInfo method)
    {
        var hasResponse = false;
        var responseModel = ModelDescriptor.Empty;

        if (method.ReturnType.IsAssignableTo<Task>() && method.ReturnType.IsGenericType)
        {
            var responseType = method.ReturnType.GetGenericArguments()[0];
            (hasResponse, responseModel) = GetResponseFromType(responseType);
        }
        else if (method.ReturnType != TypeExtensions._voidType && method.ReturnType != TypeExtensions._taskType)
        {
            (hasResponse, responseModel) = GetResponseFromType(method.ReturnType);
        }

        return (hasResponse, responseModel);
    }

    static (bool HasResponse, ModelDescriptor ResponseModel) GetResponseFromType(Type type)
    {
        if (type.IsGenericType && type.FullName!.StartsWith("System.ValueTuple"))
        {
            var bestType = type.GetBestTupleType();
            return (true, bestType.ToModelDescriptor());
        }

        if (type.IsOneOf())
        {
            var bestType = type.GetBestOneOfResponseType();
            if (bestType is not null)
            {
                return (true, bestType.ToModelDescriptor());
            }

            return (false, ModelDescriptor.Empty);
        }

        return (true, type.ToModelDescriptor());
    }
}
