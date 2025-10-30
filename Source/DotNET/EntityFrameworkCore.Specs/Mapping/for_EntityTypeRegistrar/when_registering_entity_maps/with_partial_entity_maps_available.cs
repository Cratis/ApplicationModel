// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar.when_registering_entity_maps;

public class with_partial_entity_maps_available : Specification
{
    ITypes _types;
    IServiceProvider _serviceProvider;
    EntityTypeRegistrar _registrar;
    TestDbContext _testDbContext;
    ModelBuilder _modelBuilder;
    TestEntityMap _testEntityMap;

    void Establish()
    {
        _types = Substitute.For<ITypes>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _modelBuilder = Substitute.For<ModelBuilder>();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(":memory:")
            .Options;
        _testDbContext = new TestDbContext(options);

        _testEntityMap = Substitute.For<TestEntityMap>();

        // Only TestEntityMap is available, not AnotherTestEntityMap
        _types.FindMultiple(typeof(IEntityMapFor<>)).Returns([typeof(TestEntityMap)]);

        _serviceProvider.GetService(typeof(TestEntityMap)).Returns(_testEntityMap);

        _registrar = new EntityTypeRegistrar(_types, _serviceProvider);
    }

    void Because() => _registrar.RegisterEntityMaps(_testDbContext, _modelBuilder);

    [Fact] void should_get_test_entity_map_from_service_provider() => _serviceProvider.Received(1).GetService(typeof(TestEntityMap));
    [Fact] void should_configure_test_entity_with_map() => _testEntityMap.Received(1).Configure(Arg.Any<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TestEntity>>());
    [Fact] void should_not_attempt_to_get_unmapped_entities() => _serviceProvider.DidNotReceive().GetService(typeof(AnotherTestEntityMap));
}