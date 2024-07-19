// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpointExtensions.when_adding_identity_provider;

public class and_there_are_multiple_providers : Specification
{
    Mock<IServiceCollection> services;
    Mock<ITypes> types;
    Exception exception;

    void Establish()
    {
        services = new();
        types = new();
        types.Setup(_ => _.FindMultiple<IProvideIdentityDetails>()).Returns([typeof(object), typeof(object)]);
    }

    void Because() => exception = Catch.Exception(() => services.Object.AddIdentityProvider(types.Object));

    [Fact] void should_throw_multiple_identity_details_providers_found() => exception.ShouldBeOfExactType<MultipleIdentityDetailsProvidersFound>();
    [Fact] void should_not_add_service_registration() => services.Verify(_ => _.Add(IsAny<ServiceDescriptor>()), Never);
}
