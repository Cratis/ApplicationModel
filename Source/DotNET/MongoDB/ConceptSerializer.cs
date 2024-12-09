// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Cratis.Concepts;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace Cratis.Applications.MongoDB;

/// <summary>
/// Represents a <see cref="IBsonSerializer{T}"/> for <see cref="ConceptAs{T}"/> types.
/// </summary>
/// <typeparam name="T">Type of concept.</typeparam>
public class ConceptSerializer<T> : IBsonSerializer<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConceptSerializer{T}"/> class.
    /// </summary>
    public ConceptSerializer()
    {
        ValueType = typeof(T);

        if (!ValueType.IsConcept())
            throw new TypeIsNotAConcept(ValueType);
    }

    /// <inheritdoc/>
    public Type ValueType { get; }

    /// <inheritdoc/>
    public T Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonReader = context.Reader;

        var actualType = args.NominalType;
        var bsonType = bsonReader.GetCurrentBsonType();

        var valueType = actualType.GetConceptValueType();

        object value;

        // It should be a Concept object
        if (bsonType == BsonType.Document)
        {
            bsonReader.ReadStartDocument();
            var keyName = bsonReader.ReadName(Utf8NameDecoder.Instance);
            if (keyName == "Value" || keyName == "value")
            {
                value = GetDeserializedValue(context, args, valueType, ref bsonReader);
                bsonReader.ReadEndDocument();
            }
            else
            {
                throw new MissingValueKeyInConcept();
            }
        }
        else
        {
            value = GetDeserializedValue(context, args, valueType, ref bsonReader);
        }

        if (value is null)
        {
            return default!;
        }

        return (T)ConceptFactory.CreateConceptInstance(ValueType, value);
    }

    /// <inheritdoc/>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
    {
        var bsonWriter = context.Writer;
        if (value is null)
        {
            bsonWriter.WriteNull();
            return;
        }

        var underlyingValue = value.GetConceptValue();
        var nominalType = args.NominalType;
        var underlyingValueType = nominalType.GetConceptValueType();

        if (underlyingValueType == typeof(Guid))
        {
            var guid = (Guid)underlyingValue;
            bsonWriter.WriteBinaryData(new BsonBinaryData(guid, GuidRepresentation.Standard));
        }
        else if (underlyingValueType == typeof(double))
        {
            bsonWriter.WriteDouble((double)underlyingValue);
        }
        else if (underlyingValueType == typeof(float))
        {
            bsonWriter.WriteDouble((double)underlyingValue);
        }
        else if (underlyingValueType == typeof(int) || underlyingValueType == typeof(uint))
        {
            bsonWriter.WriteInt32((int)underlyingValue);
        }
        else if (underlyingValueType == typeof(uint))
        {
            bsonWriter.WriteInt64((uint)underlyingValue);
        }
        else if (underlyingValueType == typeof(long))
        {
            bsonWriter.WriteInt64((long)underlyingValue);
        }
        else if (underlyingValueType == typeof(ulong))
        {
            bsonWriter.WriteDecimal128((ulong)underlyingValue);
        }
        else if (underlyingValueType == typeof(bool))
        {
            bsonWriter.WriteBoolean((bool)underlyingValue);
        }
        else if (underlyingValueType == typeof(string))
        {
            bsonWriter.WriteString((string)(underlyingValue ?? string.Empty));
        }
        else if (underlyingValueType == typeof(decimal))
        {
            bsonWriter.WriteDecimal128((decimal)underlyingValue);
        }
        else if (underlyingValueType == typeof(DateTime))
        {
            var dateTime = (DateTime)underlyingValue;
            bsonWriter.WriteDateTime(dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond);
        }
        else if (underlyingValueType == typeof(DateTimeOffset))
        {
            var serializer = new DateTimeOffsetSupportingBsonDateTimeSerializer();
            serializer.Serialize(context, args, (DateTimeOffset)underlyingValue);
        }
        else if (underlyingValueType == typeof(DateOnly))
        {
            var serializer = new DateOnlySerializer();
            serializer.Serialize(context, args, (DateOnly)underlyingValue);
        }
        else if (underlyingValueType == typeof(TimeOnly))
        {
            var serializer = new TimeOnlySerializer();
            serializer.Serialize(context, args, (TimeOnly)underlyingValue);
        }
    }

    /// <inheritdoc/>
    public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, T value)
    {
        Serialize(context, args, (object)value!);
    }

    /// <inheritdoc/>
    object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => Deserialize(context, args)!;

    object GetDeserializedValue(BsonDeserializationContext context, BsonDeserializationArgs args, Type valueType, ref IBsonReader bsonReader)
    {
        var bsonType = bsonReader.CurrentBsonType;
        if (bsonType == BsonType.Null)
        {
            bsonReader.ReadNull();
            return null!;
        }

        if (valueType == typeof(Guid))
        {
            if (bsonReader.GetCurrentBsonType() == BsonType.String)
            {
                return Guid.Parse(bsonReader.ReadString());
            }
            var binaryData = bsonReader.ReadBinaryData();
            return binaryData.ToGuid();
        }

        if (valueType == typeof(double))
        {
            return bsonReader.ReadDouble();
        }

        if (valueType == typeof(float))
        {
            return (float)bsonReader.ReadDouble();
        }

        if (valueType == typeof(int))
        {
            return bsonReader.ReadInt32();
        }

        if (valueType == typeof(uint))
        {
            return (uint)bsonReader.ReadInt64();
        }

        if (valueType == typeof(long))
        {
            return bsonReader.ReadInt64();
        }

        if (valueType == typeof(ulong))
        {
            return (ulong)bsonReader.ReadDecimal128();
        }

        if (valueType == typeof(bool))
        {
            return bsonReader.ReadBoolean();
        }

        if (valueType == typeof(string))
        {
            return bsonReader.ReadString();
        }

        if (valueType == typeof(decimal))
        {
            return bsonReader.ReadDecimal128();
        }

        if (valueType == typeof(DateTime))
        {
            var dateTimeValue = bsonReader.ReadDateTime();
            return DateTimeOffset.FromUnixTimeMilliseconds(dateTimeValue).DateTime;
        }

        if (valueType == typeof(DateTimeOffset))
        {
            var serializer = new DateTimeOffsetSupportingBsonDateTimeSerializer();
            return serializer.Deserialize(context, args);
        }

        if (valueType == typeof(DateOnly))
        {
            var serializer = new DateOnlySerializer();
            return serializer.Deserialize(context, args);
        }

        if (valueType == typeof(TimeOnly))
        {
            var serializer = new TimeOnlySerializer();
            return serializer.Deserialize(context, args);
        }

        throw new UnableToDeserializeValueForConcept(valueType);
    }
}
