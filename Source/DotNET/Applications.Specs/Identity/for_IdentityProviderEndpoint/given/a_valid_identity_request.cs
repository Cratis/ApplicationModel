// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.Json;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.given;

public abstract class a_valid_identity_request : an_identity_provider_endpoint
{
    protected string _identityId = "123";
    protected string _identityName = "Test User";
    protected IdentityProviderContext _identityProviderContext;
    protected IdentityDetails _detailsResult = new(true, "Hello world");
    protected ClientPrincipal _clientPrincipal;

    void Establish()
    {
        _headers[MicrosoftIdentityPlatformHeaders.IdentityIdHeader] = "123";
        _headers[MicrosoftIdentityPlatformHeaders.IdentityNameHeader] = "Test User";

        _clientPrincipal = CreateClientPrincipal();
        _headers[MicrosoftIdentityPlatformHeaders.PrincipalHeader] =
            Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(_clientPrincipal, Globals.JsonSerializerOptions));

        _identityProvider.Provide(Arg.Any<IdentityProviderContext>()).Returns((x) =>
        {
            _identityProviderContext = x.Arg<IdentityProviderContext>();
            return Task.FromResult(_detailsResult);
        });
    }

    protected virtual ClientPrincipal CreateClientPrincipal()
    {
        return new()
        {
            IdentityProvider = "aad",
            UserId = "123",
            UserDetails = "Test User",
            Claims =
            [
                new ClientPrincipalClaim { typ = "roles", val = "role1" },
                new ClientPrincipalClaim { typ = "roles", val = "role2" }
            ]
        };
    }
}