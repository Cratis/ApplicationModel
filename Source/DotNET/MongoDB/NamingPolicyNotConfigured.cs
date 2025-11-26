// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Serialization;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// The exception that is thrown when the naming policy for MongoDB has not been configured.
/// </summary>
/// <param name="message">The additional message of the error.</param>
public class NamingPolicyNotConfigured(string message)
    : Exception($"A naming policy for MongoDB has not been configured. {message}")
{
    /// <summary>
    /// Throw if not configured.
    /// </summary>
    /// <param name="convention">The <see cref="INamingPolicy"/>.</param>
    /// <exception cref="NamingPolicyNotConfigured">Thrown if the resolver is not configured.</exception>
    public static void ThrowIfNotConfigured(INamingPolicy? convention)
    {
        if (convention is null)
        {
            throw new NamingPolicyNotConfigured($"Please configure it using the {nameof(MongoDBBuilderExtensions.WithNamingPolicy)} method");
        }
    }
}
