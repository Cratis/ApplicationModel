// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Tenancy;

/// <summary>
/// Represents the options for the correlation ID.
/// </summary>
public class TenancyOptions
{
    /// <summary>
    /// Gets or sets the HTTP header to use for the correlation ID.
    /// </summary>
    public string HttpHeader { get; set; } = Constants.DefaultTenantIdHeader;
}