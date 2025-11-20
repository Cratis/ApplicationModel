// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cratis.Applications.EntityFrameworkCore.Json.for_JsonConversion.when_checking_has_json_properties;

public class with_entity_having_json_constructor_parameters : Specification
{
    IMutableEntityType _entityType;
    bool _result;

    void Establish()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.Entity<EntityWithJsonConstructorParameters>();
        _entityType = modelBuilder.Model.GetEntityTypes().First(e => e.ClrType == typeof(EntityWithJsonConstructorParameters));
    }

    void Because() => _result = _entityType.HasJsonProperties();

    [Fact] void should_return_true() => _result.ShouldBeTrue();
}
