// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace Cratis.Arc.MongoDB;

/// <summary>
/// Extension methods for working with <see cref="BsonClassMap{T}"/>.
/// </summary>
public static class BsonClassMapExtension
{
    /// <summary>
    /// Unmaps all members except the ones specified.
    /// </summary>
    /// <param name="classMap"><see cref="BsonClassMap{T}"/> to unmap from.</param>
    /// <param name="membersToKeep">Members to keep represented as lambdas.</param>
    /// <typeparam name="T">Type of class the class map is for.</typeparam>
    public static void UnmapAllMembersExcept<T>(this BsonClassMap<T> classMap, params Expression<Func<T, object>>[] membersToKeep)
    {
        var membersToKeepNames = membersToKeep.Select(_ => ((MemberExpression)_.Body).Member.Name.ToLowerInvariant()).ToArray();
        foreach (var member in classMap.DeclaredMemberMaps.Where(_ => !membersToKeepNames.Contains(_.MemberName.ToLowerInvariant())).ToArray())
        {
            classMap.UnmapMember(member.MemberInfo);
        }
    }

    /// <summary>
    /// Apply conventions to a <see cref="BsonClassMap{T}"/>.
    /// </summary>
    /// <param name="classMap"><see cref="BsonClassMap{T}"/> to apply to.</param>
    /// <typeparam name="T">Type of class the class map is for.</typeparam>
    public static void ApplyConventions<T>(this BsonClassMap<T> classMap)
    {
        foreach (var convention in ConventionRegistry.Lookup(typeof(T)).Conventions.OfType<IMemberMapConvention>())
        {
            foreach (var member in classMap.DeclaredMemberMaps)
            {
                convention.Apply(member);
            }
        }
    }
}
