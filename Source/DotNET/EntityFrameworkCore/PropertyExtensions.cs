// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cratis.Applications.EntityFrameworkCore;

/// <summary>
/// Extensions for configuring properties in Entity Framework Core.
/// </summary>
public static class PropertyExtensions
{
    static readonly ValueConverter<Guid, string> _guidValueConverter = new(
        guid => guid.ToString("D"),
        str => Guid.Parse(str));

    /// <summary>
    /// Configures the property to use a GUID representation that is compatible across different database providers.
    /// </summary>
    /// <param name="propertyBuilder">The property builder to configure.</param>
    /// <param name="database">The database provider, if specific configuration is needed.</param>
    /// <returns>The configured property builder.</returns>
    public static PropertyBuilder AsGuid(this PropertyBuilder propertyBuilder, DatabaseFacade database)
    {
        if (database.GetDatabaseType() == DatabaseType.Sqlite)
        {
            propertyBuilder.HasConversion(_guidValueConverter);
        }

        return propertyBuilder;
    }
}
