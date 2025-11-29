// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace Cratis.Arc.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar.given;

public class an_entity_map_registrar : Specification
{
    protected ITypes _types;
    protected IServiceProvider _serviceProvider;
    protected EntityTypeRegistrar _registrar;
    protected DbContext _dbContext;
    protected ModelBuilder _modelBuilder;

    void Establish()
    {
        _types = Substitute.For<ITypes>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _dbContext = Substitute.For<DbContext>();
        _modelBuilder = Substitute.For<ModelBuilder>();

        // Set up default empty return to avoid issues in constructor
        _types.FindMultiple(typeof(IEntityTypeConfiguration<>)).Returns([]);

        _registrar = new(_types, _serviceProvider);
    }
}