// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Cratis.Applications.Identity.for_IdentityProviderEndpointExtensions.when_adding_identity_provider;

public class and_there_is_one_provider : Specification
{
    class MyIdentityProvider : IProvideIdentityDetails
    {
        public Task<IdentityDetails> Provide(IdentityProviderContext context) => throw new NotImplementedException();
    }

    Mock<IServiceCollection> services;
    Mock<ITypes> types;
    ServiceDescriptor _serviceDescriptor;

    void Establish()
    {
        services = new();
        types = new();
        types.Setup(_ => _.FindMultiple<IProvideIdentityDetails>()).Returns([typeof(MyIdentityProvider)]);
        services.Setup(_ => _.Add(IsAny<ServiceDescriptor>())).Callback((ServiceDescriptor _) => _serviceDescriptor = _);
    }

    void Because() => services.Object.AddIdentityProvider(types.Object);

    [Fact] void should_add_one_service_registration() => services.Verify(_ => _.Add(IsAny<ServiceDescriptor>()), Once);
    [Fact] void should_register_as_identity_details_provider() => _serviceDescriptor.ServiceType.ShouldEqual(typeof(IProvideIdentityDetails));
    [Fact] void should_register_expected_provider() => _serviceDescriptor.ImplementationType.ShouldEqual(typeof(MyIdentityProvider));
    [Fact] void should_register_as_singleton() => _serviceDescriptor.Lifetime.ShouldEqual(ServiceLifetime.Singleton);
}