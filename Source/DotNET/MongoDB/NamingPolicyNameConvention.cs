// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// A naming policy based name convention for MongoDB member maps.
/// This convention applies a naming policy to the element names of member maps.
/// </summary>
public class NamingPolicyNameConvention : ConventionBase, IMemberMapConvention
{
    /// <summary>
    /// Gets the name of the convention.
    /// </summary>
    public const string ConventionName = "Naming policy based name convention";

    /// <summary>
    /// Applies a modification to the member map, using the naming policy to set the element name.
    /// </summary>
    /// <param name="memberMap">The member map.</param>
    public void Apply(BsonMemberMap memberMap)
    {
        var name = memberMap.ElementName;
        memberMap.SetElementName(DatabaseExtensions.NamingPolicy.GetPropertyName(name));
    }
}
