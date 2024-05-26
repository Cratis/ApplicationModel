// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Tenancy;

/// <summary>
/// Holds constants related to correlation id.
/// </summary>
public static class Constants
{
    /// <summary>
    /// Gets the header name for the correlation id.
    /// </summary>
    public const string DefaultTenantIdHeader = "X-Tenant-ID";

    /// <summary>
    /// Gets the item key for the correlation id.
    /// </summary>
    public const string TenantIdItemKey = "TenantId";
}
