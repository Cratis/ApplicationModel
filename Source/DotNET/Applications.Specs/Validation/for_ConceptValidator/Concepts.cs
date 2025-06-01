// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;

namespace Cratis.Applications.Validation.for_ConceptValidator;

#pragma warning disable SA1649
public record string_concept(string Value) : ConceptAs<string>(Value);
public record bool_concept(bool Value) : ConceptAs<bool>(Value);
public record guid_concept(Guid Value) : ConceptAs<Guid>(Value);
public record date_only_concept(DateOnly Value) : ConceptAs<DateOnly>(Value);
public record time_only_concept(TimeOnly Value) : ConceptAs<TimeOnly>(Value);
public record date_time_concept(DateTime Value) : ConceptAs<DateTime>(Value);
public record date_time_offset_concept(DateTimeOffset Value) : ConceptAs<DateTimeOffset>(Value);
public record float_concept(float Value) : ConceptAs<float>(Value);
public record double_concept(double Value) : ConceptAs<double>(Value);
public record decimal_concept(decimal Value) : ConceptAs<decimal>(Value);
public record sbyte_concept(sbyte Value) : ConceptAs<sbyte>(Value);
public record short_concept(short Value) : ConceptAs<short>(Value);
public record int_concept(int Value) : ConceptAs<int>(Value);
public record long_concept(long Value) : ConceptAs<long>(Value);
public record byte_concept(byte Value) : ConceptAs<byte>(Value);
public record ushort_concept(ushort Value) : ConceptAs<ushort>(Value);
public record uint_concept(uint Value) : ConceptAs<uint>(Value);
public record ulong_concept(ulong Value) : ConceptAs<ulong>(Value);

#pragma warning restore SA1649
