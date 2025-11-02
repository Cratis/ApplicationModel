// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Applications.EntityFrameworkCore.Json;
using Cratis.Concepts;

namespace Cratis.Applications.EntityFrameworkCore.for_BaseDbContext;

#pragma warning disable SA1402, SA1649 // Single type per file,  File name should match first type name

public record PersonId(Guid Value) : ConceptAs<Guid>(Value)
{
    public static readonly PersonId NotSet = new(Guid.Empty);
    public static implicit operator PersonId(Guid value) => new(value);
}

public record PersonName(string Value) : ConceptAs<string>(Value)
{
    public static readonly PersonName Empty = new(string.Empty);
    public static implicit operator PersonName(string value) => new(value);
}

public record Age(int Value) : ConceptAs<int>(Value)
{
    public static readonly Age NotSet = new(0);
    public static implicit operator Age(int value) => new(value);
}

public class Person
{
    public PersonId Id { get; set; } = PersonId.NotSet;
    public PersonName Name { get; set; } = PersonName.Empty;
    public Age Age { get; set; } = Age.NotSet;
    public Guid ExternalId { get; set; }
    public string Address { get; set; } = string.Empty;
}

public class Company
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Json]
    public CompanyMetadata Metadata { get; set; } = new();
}

public class CompanyMetadata
{
    public string Industry { get; set; } = string.Empty;
    public int EmployeeCount { get; set; }
    public MetadataId MetadataId { get; set; } = MetadataId.NotSet;
}

public record MetadataId(string Value) : ConceptAs<string>(Value)
{
    public static readonly MetadataId NotSet = new(string.Empty);
    public static implicit operator MetadataId(string value) => new(value);
}

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

// Entity to test that types only in JSON properties don't get Concept/Guid conversions
public class Department
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Json]
    public DepartmentSettings Settings { get; set; } = new();
}

public class DepartmentSettings
{
    public string Description { get; set; } = string.Empty;
    public SettingsId SettingsId { get; set; } = SettingsId.NotSet;
    public Guid TrackingId { get; set; }
}

public record SettingsId(string Value) : ConceptAs<string>(Value)
{
    public static readonly SettingsId NotSet = new(string.Empty);
    public static implicit operator SettingsId(string value) => new(value);
}

// Entity to test mixed scenario: same type appears in both JSON and non-JSON properties
public class Organization
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    [Json]
    public OrganizationMetadata Metadata { get; set; } = new();
    public LocationId LocationId { get; set; } = LocationId.NotSet;
    public Guid ReferenceId { get; set; }
}

public class OrganizationMetadata
{
    public string Description { get; set; } = string.Empty;
    public LocationId LocationId { get; set; } = LocationId.NotSet;
    public Guid CorrelationId { get; set; }
}

public record LocationId(int Value) : ConceptAs<int>(Value)
{
    public static readonly LocationId NotSet = new(0);
    public static implicit operator LocationId(int value) => new(value);
}

#pragma warning restore SA1402, SA1649 // Single type per file,  File name should match first type name
