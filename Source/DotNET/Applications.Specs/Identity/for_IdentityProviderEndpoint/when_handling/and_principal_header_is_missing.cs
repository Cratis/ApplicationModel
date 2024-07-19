// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_principal_header_is_missing : given.a_valid_identity_request
{
    void Establish() => headers.Remove(MicrosoftIdentityPlatformHeaders.PrincipalHeader);

    Task Because() => endpoint.Handler(request.Object, response.Object);

    [Fact] void should_not_invoke_identity_provider() => identity_provider.Verify(_ => _.Provide(IsAny<IdentityProviderContext>()), Never);
}
