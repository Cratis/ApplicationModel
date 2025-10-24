// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_identity_is_valid_with_multiple_roles : given.a_valid_identity_request
{
    protected override ClaimsPrincipal CreateClaimsPrincipal()
    {
        var claims = new List<Claim>
        {
            new("sub", _identityId),
            new(ClaimTypes.Name, _identityName),
            new(ClaimTypes.Role, "role1"),
            new(ClaimTypes.Role, "role2")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        return new ClaimsPrincipal(identity);
    }

    Task Because() => _endpoint.Handler(_request, _response);

    [Fact] void should_invoke_identity_provider() => _identityProvider.Received(1).Provide(Arg.Any<IdentityProviderContext>());
    [Fact] void should_pass_first_role_to_identity_provider() => _identityProviderContext.Claims.ShouldContain(_ => _.Key == ClaimTypes.Role && _.Value == "role1");
    [Fact] void should_pass_second_role_to_identity_provider() => _identityProviderContext.Claims.ShouldContain(_ => _.Key == ClaimTypes.Role && _.Value == "role2");
    [Fact] void should_set_status_code_to_200() => _response.StatusCode.ShouldEqual(200);
}