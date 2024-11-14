// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_identity_is_valid_with_multiple_roles : given.a_valid_identity_request
{
    protected override ClientPrincipal CreateClientPrincipal() => new()
    {
        IdentityProvider = "aad",
        Claims =
        [
            new ClientPrincipalClaim { typ = ClaimTypes.Role, val = "role1" },
            new ClientPrincipalClaim { typ = ClaimTypes.Role, val = "role2" }
        ]
    };

    Task Because() => endpoint.Handler(request, response);

    [Fact] void should_invoke_identity_provider() => identity_provider.Received(1).Provide(Arg.Any<IdentityProviderContext>());
    [Fact] void should_pass_first_role_to_identity_provider() => identity_provider_context.Claims.ShouldContain(_ => _.Key == ClaimTypes.Role && _.Value == "role1");
    [Fact] void should_pass_second_role_to_identity_provider() => identity_provider_context.Claims.ShouldContain(_ => _.Key == ClaimTypes.Role && _.Value == "role2");
    [Fact] void should_set_status_code_to_200() => response.StatusCode.ShouldEqual(200);
}