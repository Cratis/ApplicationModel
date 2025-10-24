// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.Identity.for_IdentityProviderEndpoint.when_handling;

public class and_user_is_not_authorized : given.a_valid_identity_request
{
    void Establish()
    {
        _detailsResult = new IdentityDetails(false, "Not authorized");
    }

    Task Because() => _endpoint.Handler(_request, _response);

    [Fact] void should_invoke_identity_provider() => _identityProvider.Received(1).Provide(Arg.Any<IdentityProviderContext>());
    [Fact] void should_set_status_code_to_403() => _response.StatusCode.ShouldEqual(403);
}