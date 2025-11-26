// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Arc.EntityFrameworkCore;

/// <summary>
/// Exception that is thrown when a database type is not supported.
/// </summary>
/// <param name="connectionString">The connection string that was used.</param>
public class UnsupportedDatabaseType(string connectionString)
    : Exception($"Connection string '{connectionString}' has an unsupported database type.");
