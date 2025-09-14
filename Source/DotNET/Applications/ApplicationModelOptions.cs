// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.Execution;
using Cratis.Applications.Tenancy;

namespace Cratis.Applications;

/// <summary>
/// Represents the options for the application model.
/// </summary>
public class ApplicationModelOptions
{
    /// <summary>
    /// Gets or sets the options for the correlation ID.
    /// </summary>
    public CorrelationIdOptions CorrelationId { get; set; } = new();

    /// <summary>
    /// Gets or sets the options for the tenancy.
    /// </summary>
    public TenancyOptions Tenancy { get; set; } = new();

    /// <summary>
    /// Gets or sets what type of identity details provider to use. If none is specified it will use type discovery to try to find one.
    /// </summary>
    public Type? IdentityDetailsProvider { get; set; }

    /// <summary>
    /// Gets or sets the options for command handling.
    /// </summary>
    public CommandOptions Commands { get; set; } = new();
}

