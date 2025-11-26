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

            if (responseType.IsGenericType && responseType.FullName!.StartsWith("System.ValueTuple"))
            {
                responseType = responseType.GetBestTupleType();
            }

            if (!responseType.IsOneOf())
            {
                hasResponse = true;
                responseModel = responseType.ToModelDescriptor();
            }
        }
        else if (method.ReturnType != TypeExtensions._voidType && method.ReturnType != TypeExtensions._taskType)
        {
            var returnType = method.ReturnType;
            if (returnType.IsGenericType &&
                returnType.FullName!.StartsWith("System.ValueTuple"))
            {
                returnType = returnType.GetBestTupleType();
            }

            if (!returnType.IsOneOf())
            {
                hasResponse = true;
                responseModel = returnType.ToModelDescriptor();
            }
        }

        return (hasResponse, responseModel);
    }
}
