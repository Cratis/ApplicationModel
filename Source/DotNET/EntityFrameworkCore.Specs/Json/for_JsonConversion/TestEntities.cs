// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Cratis.Applications.EntityFrameworkCore.Json.for_JsonConversion;

#pragma warning disable SA1402, SA1649 // Single type per file, File name should match first type name

public record PersonName(string FirstName, string LastName);
public record Address(string Street, string City);
public record PhoneNumber(string Number);

public class EntityWithJsonProperties
{
    public int Id { get; set; }

    [Json]
    public PersonName Name { get; set; } = null!;

    [Json]
    public Address Address { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
}

public class EntityWithoutJsonProperties
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class EntityWithJsonConstructorParameters
{
    public int Id { get; set; }

    [Json]
    public PersonName Name { get; set; } = null!;

    public string Email { get; set; } = string.Empty;
}

public class EntityWithMixedJsonUsage
{
    public int Id { get; set; }

    [Json]
    public Address Address { get; set; } = null!;

    public PhoneNumber Phone { get; set; } = null!;
}
