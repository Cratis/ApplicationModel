// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;

namespace Cratis.Applications.Commands.Filters.for_AuthorizationFilter.given;

[Authorize(Roles = "Admin,Manager")]
public class CommandWithMultipleRoles;