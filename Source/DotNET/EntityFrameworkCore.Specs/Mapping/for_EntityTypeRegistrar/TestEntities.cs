// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Mapping.for_EntityTypeRegistrar;

#pragma warning disable SA1402, SA1649 // Single type per file,  File name should match first type name
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
#pragma warning restore SA1402, SA1649 // Single type per file,  File name should match first type name