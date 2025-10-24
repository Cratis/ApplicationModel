// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.given;

public abstract class a_valid_identity_request : an_identity_provider_endpoint
{
    protected string _identityId = "123";
    protected string _identityName = "Test User";
    protected IdentityProviderContext _identityProviderContext;
    protected IdentityDetails _detailsResult = new(true, "Hello world");
    protected ClaimsPrincipal _claimsPrincipal;

    void Establish()
    {
        _claimsPrincipal = CreateClaimsPrincipal();
        _httpContext.User = _claimsPrincipal;

        _identityProvider.Provide(Arg.Any<IdentityProviderContext>()).Returns((x) =>
        {
            _identityProviderContext = x.Arg<IdentityProviderContext>();
            return Task.FromResult(_detailsResult);
        });
    }

    protected virtual ClaimsPrincipal CreateClaimsPrincipal()
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
}