// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_name_is_missing : given.an_identity_provider_endpoint
{
    void Establish()
    {
        var claims = new List<Claim>
        {
            new("sub", "123"),
            new(ClaimTypes.Role, "role1")
        };

        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        _httpContext.User = new ClaimsPrincipal(identity);

        _identityProvider.Provide(Arg.Any<IdentityProviderContext>()).Returns(new IdentityDetails(true, "Hello world"));
    }

    Task Because() => _endpoint.Handler(_request, _response);

    [Fact] void should_invoke_identity_provider() => _identityProvider.Received(1).Provide(Arg.Any<IdentityProviderContext>());
    [Fact] void should_pass_unknown_as_identity_name() => _identityProvider.Received(1).Provide(Arg.Is<IdentityProviderContext>(ctx => ctx.Name == "unknown"));
    [Fact] void should_set_status_code_to_200() => _response.StatusCode.ShouldEqual(200);
}
