// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cratis.Applications.EntityFrameworkCore.Json.for_JsonConversion.when_applying_json_conversion_to_model;

public class with_no_entities_having_json_properties : given.a_json_conversion_context
{
    ModelBuilder _modelBuilder;
    IEnumerable<IMutableEntityType> _entityTypes;
    IEnumerable<Type> _result;

    void Establish()
    {
        _modelBuilder = new ModelBuilder();
        _modelBuilder.Entity<EntityWithoutJsonProperties>();
        _entityTypes = _modelBuilder.Model.GetEntityTypes();
    }

    void Because() => _result = _modelBuilder.ApplyJsonConversion(_entityTypes, DatabaseType.Sqlite);

    [Fact] void should_return_empty_collection() => _result.ShouldBeEmpty();
}
