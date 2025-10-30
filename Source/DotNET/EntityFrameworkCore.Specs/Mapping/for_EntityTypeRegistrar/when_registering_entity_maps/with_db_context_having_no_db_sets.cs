// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar.when_registering_entity_maps;

public class with_db_context_having_no_db_sets : Specification
{
    ITypes _types;
    IServiceProvider _serviceProvider;
    EntityTypeRegistrar _registrar;
    EmptyDbContext _emptyDbContext;
    ModelBuilder _modelBuilder;

    void Establish()
    {
        _types = Substitute.For<ITypes>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _modelBuilder = Substitute.For<ModelBuilder>();

        var options = new DbContextOptionsBuilder<EmptyDbContext>()
            .UseSqlite(":memory:")
            .Options;
        _emptyDbContext = new EmptyDbContext(options);

        _types.FindMultiple(typeof(IEntityMapFor<>)).Returns([typeof(TestEntityMap)]);

        _registrar = new EntityTypeRegistrar(_types, _serviceProvider);
    }

    void Because() => _registrar.RegisterEntityMaps(_emptyDbContext, _modelBuilder);

    [Fact] void should_not_attempt_to_get_any_entity_maps_from_service_provider() => _serviceProvider.DidNotReceive().GetService(Arg.Any<Type>());
}