// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Authorization;

/// <summary>
/// Defines a contract for performing authorization checks.
/// </summary>
public interface IAuthorizationEvaluator
{
    /// <summary>
    /// Checks if the current user is authorized based on the <see cref="Microsoft.AspNetCore.Authorization.AuthorizeAttribute"/> on the specified type.
    /// </summary>
    /// <param name="type">The type to check for authorization attributes.</param>
    /// <returns>True if authorized, false if unauthorized.</returns>
    bool IsAuthorized(Type type);
}