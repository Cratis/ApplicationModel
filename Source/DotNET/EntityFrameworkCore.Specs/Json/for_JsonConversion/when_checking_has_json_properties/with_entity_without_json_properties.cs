// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Cratis.Arc.EntityFrameworkCore.Json.for_JsonConversion.when_checking_has_json_properties;

public class with_entity_without_json_properties : Specification
{
    IMutableEntityType _entityType;
    bool _result;

    void Establish()
    {
        var modelBuilder = new ModelBuilder();
        modelBuilder.Entity<EntityWithoutJsonProperties>();
        _entityType = modelBuilder.Model.GetEntityTypes().First(e => e.ClrType == typeof(EntityWithoutJsonProperties));
    }

    void Because() => _result = _entityType.HasJsonProperties();

    [Fact] void should_return_false() => _result.ShouldBeFalse();
}
