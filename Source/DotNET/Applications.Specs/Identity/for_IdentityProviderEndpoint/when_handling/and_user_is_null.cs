// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_user_is_null : given.an_identity_provider_endpoint
{
    void Establish()
    {
        _httpContext.User = null!;
    }

    Task Because() => _endpoint.Handler(_request, _response);

    [Fact] void should_not_invoke_identity_provider() => _identityProvider.DidNotReceive().Provide(Arg.Any<IdentityProviderContext>());
    [Fact] void should_set_status_code_to_401() => _response.StatusCode.ShouldEqual(401);
}