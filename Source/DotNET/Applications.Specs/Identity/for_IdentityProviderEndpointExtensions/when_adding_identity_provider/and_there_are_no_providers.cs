// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpointExtensions.when_adding_identity_provider;

public class and_there_are_no_providers : Specification
{
    Mock<IServiceCollection> services;
    Mock<ITypes> types;
    ServiceDescriptor service_descriptor;

    void Establish()
    {
        services = new();
        types = new();
        services.Setup(_ => _.Add(IsAny<ServiceDescriptor>())).Callback((ServiceDescriptor _) => service_descriptor = _);
    }

    void Because() => services.Object.AddIdentityProvider(types.Object);

    [Fact] void should_add_one_service_registration() => services.Verify(_ => _.Add(IsAny<ServiceDescriptor>()), Once);
    [Fact] void should_add_default_identity_details_provider() => service_descriptor.ImplementationType.ShouldEqual(typeof(DefaultIdentityDetailsProvider));
}