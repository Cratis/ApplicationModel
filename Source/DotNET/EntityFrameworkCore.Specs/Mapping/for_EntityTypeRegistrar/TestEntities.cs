// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar;

public class TestEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class AnotherTestEntity
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class EntityWithoutMap
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}