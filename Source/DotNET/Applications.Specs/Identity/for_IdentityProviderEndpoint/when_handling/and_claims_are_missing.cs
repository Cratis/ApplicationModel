// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text.Json;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_claims_are_missing : given.a_valid_identity_request
{
    void Establish()
    {
        client_principal.claims = null!;
        headers[MicrosoftIdentityPlatformHeaders.PrincipalHeader] =
            Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(client_principal));
    }

    Task Because() => endpoint.Handler(request.Object, response.Object);

    [Fact] void should_not_invoke_identity_provider() => identity_provider.Verify(_ => _.Provide(IsAny<IdentityProviderContext>()), Never);
}
