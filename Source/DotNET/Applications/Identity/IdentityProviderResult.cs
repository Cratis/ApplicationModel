// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Identity;

/// <summary>
/// Represents the result of providing identity.
/// </summary>
/// <param name="Id">Unique identifier for the identity.</param>
/// <param name="Name">Name of the identity.</param>
/// <param name="Details">The resolved details.</param>
public record IdentityProviderResult(IdentityId Id, IdentityName Name, object Details);
