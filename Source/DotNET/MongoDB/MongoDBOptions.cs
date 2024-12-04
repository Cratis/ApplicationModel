// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents the configuration for MongoDB.
/// </summary>
public class MongoDBOptions
{
    /// <summary>
    /// The server url.
    /// </summary>
    [Required]
    public string Server { get; set; } = null!;

    /// <summary>
    /// The database name.
    /// </summary>
    [Required]
    public string Database { get; set; } = null!;

    /// <summary>
    /// Gets whether or use the direct connection option for MongoDB. Defaults to true.
    /// </summary>
    public bool DirectConnection { get; set; } = true;
}