// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Serialization;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// The exception that is thrown when the <see cref="IMongoServerResolver"/> is missing.
/// </summary>
/// <param name="message">The additional message of the error.</param>
public class NamingPolicyNotConfigured(string message)
    : Exception($"A model name convention for MongoDB has not been configured. {message}")
{
    /// <summary>
    /// Throw if not configured.
    /// </summary>
    /// <param name="convention">The <see cref="INamingPolicy"/>.</param>
    /// <param name="conventionType">The type of the model name convention.</param>
    /// <exception cref="NamingPolicyNotConfigured">Thrown if the resolver is not configured.</exception>
    public static void ThrowIfNotConfigured(INamingPolicy? convention, Type? conventionType)
    {
        if (convention is not null)
        {
            if (conventionType is not null)
            {
                throw new NamingPolicyNotConfigured($"Two naming policies are configured. Use {nameof(MongoDBBuilderExtensions.WithNamingPolicy)} to configure a specific naming policy.");
            }

            return;
        }

        if (conventionType is null)
        {
            throw new NamingPolicyNotConfigured($"Please configure it using the {nameof(MongoDBBuilderExtensions.WithNamingPolicy)} method");
        }

        if (!conventionType.IsAssignableTo(typeof(INamingPolicy)))
        {
            throw new NamingPolicyNotConfigured($"The given type {conventionType} is not assignable to {typeof(INamingPolicy)}");
        }
    }
}
