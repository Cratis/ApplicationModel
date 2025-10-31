// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityMapRegistrar.when_registering_entity_maps;

public class with_entity_maps_available : Specification
{
    ITypes _types;
    IServiceProvider _serviceProvider;
    EntityMapRegistrar _registrar;
    TestDbContext _testDbContext;
    ModelBuilder _modelBuilder;
    TestEntityMap _testEntityMap;
    AnotherTestEntityMap _anotherTestEntityMap;

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
        _anotherTestEntityMap = Substitute.For<AnotherTestEntityMap>();

        _types.FindMultiple(typeof(IEntityMapFor<>)).Returns([typeof(TestEntityMap), typeof(AnotherTestEntityMap)]);

        _serviceProvider.GetService(typeof(TestEntityMap)).Returns(_testEntityMap);
        _serviceProvider.GetService(typeof(AnotherTestEntityMap)).Returns(_anotherTestEntityMap);

        _registrar = new(_types, _serviceProvider);
    }

    void Because() => _registrar.RegisterEntityMaps(_testDbContext, _modelBuilder);

    [Fact] void should_get_test_entity_map_from_service_provider() => _serviceProvider.Received(1).GetService(typeof(TestEntityMap));
    [Fact] void should_get_another_test_entity_map_from_service_provider() => _serviceProvider.Received(1).GetService(typeof(AnotherTestEntityMap));
    [Fact] void should_configure_test_entity() => _testEntityMap.Received(1).Configure(Arg.Any<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TestEntity>>());
    [Fact] void should_configure_another_test_entity() => _anotherTestEntityMap.Received(1).Configure(Arg.Any<Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<AnotherTestEntity>>());
}