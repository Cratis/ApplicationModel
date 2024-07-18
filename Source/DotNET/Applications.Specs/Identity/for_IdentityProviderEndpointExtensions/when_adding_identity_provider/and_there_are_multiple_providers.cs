// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpointExtensions.when_adding_identity_provider;

public class and_there_are_multiple_providers : Specification
{
    IServiceCollection services;
    ITypes types;
    Exception exception;

    void Establish()
    {
        services = Substitute.For<IServiceCollection>();
        types = Substitute.For<ITypes>();
        types.FindMultiple<IProvideIdentityDetails>().Returns([typeof(object), typeof(object)]);
    }

    void Because() => exception = Catch.Exception(() => services.AddIdentityProvider(types));

    [Fact] void should_throw_multiple_identity_details_providers_found() => exception.ShouldBeOfExactType<MultipleIdentityDetailsProvidersFound>();
    [Fact] void should_not_add_service_registration() => services.DidNotReceive().Add(Arg.Any<ServiceDescriptor>());
}
