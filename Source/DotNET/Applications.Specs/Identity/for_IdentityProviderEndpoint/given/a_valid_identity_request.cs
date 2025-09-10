// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Cratis.Json;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.given;

public abstract class a_valid_identity_request : an_identity_provider_endpoint
{
    protected string identity_id = "123";
    protected string identity_name = "Test User";
    protected IdentityProviderContext identity_provider_context;
    protected IdentityDetails details_result = new(true, "Hello world");
    protected ClientPrincipal client_principal;

    void Establish()
    {
        _headers[MicrosoftIdentityPlatformHeaders.IdentityIdHeader] = "123";
        _headers[MicrosoftIdentityPlatformHeaders.IdentityNameHeader] = "Test User";

        client_principal = CreateClientPrincipal();
        _headers[MicrosoftIdentityPlatformHeaders.PrincipalHeader] =
            Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(client_principal, Globals.JsonSerializerOptions));

        _identityProvider.Provide(Arg.Any<IdentityProviderContext>()).Returns((x) =>
        {
            identity_provider_context = x.Arg<IdentityProviderContext>();
            return Task.FromResult(details_result);
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