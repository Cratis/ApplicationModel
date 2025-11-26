// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cratis.Arc.EntityFrameworkCore.Json.for_JsonConversion.when_applying_json_conversion_to_model;

public class with_mixed_json_usage_excluding_shared_types : given.a_json_conversion_context
{
    ModelBuilder _modelBuilder;
    IEnumerable<IMutableEntityType> _entityTypes;
    IEnumerable<Type> _result;

    void Establish()
    {
        _modelBuilder = new ModelBuilder();
        _modelBuilder.Entity<EntityWithMixedJsonUsage>();
        _modelBuilder.Entity<EntityWithoutJsonProperties>();
        _entityTypes = _modelBuilder.Model.GetEntityTypes();
    }

    void Because() => _result = _modelBuilder.ApplyJsonConversion(_entityTypes, DatabaseType.Sqlite);

    [Fact] void should_return_only_address_type() => _result.ShouldContainOnly([typeof(Address)]);
    [Fact] void should_not_return_phone_number_type() => _result.ShouldNotContain(typeof(PhoneNumber));
}
