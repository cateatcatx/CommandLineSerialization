using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization;

public class BuiltinNullableSerializer : IValueSerializer
{
    public bool CanHandleType(Type objType)
    {
        return objType.Name == "Nullable`1";
    }

    public ObjectSpecs? GetObjectSpecs(Type objType)
    {
        return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Single, objType), objType);
    }

    public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
    {
        return serializer.ValueSerializer.DeserializeNonValue(serializer, objType.GenericTypeArguments[0], matched);
    }

    public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
    {
        return serializer.ValueSerializer.DeserializeSingleValue(serializer, objType.GenericTypeArguments[0], value);
    }

    public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
    {
        return serializer.ValueSerializer.DeserializeMultiValue(serializer, objType.GenericTypeArguments[0], values);
    }

    public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        return serializer.ValueSerializer.SerializeNonValue(serializer, objType.GenericTypeArguments[0], obj);
    }

    public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        return serializer.ValueSerializer.SerializeSingleValue(serializer, objType.GenericTypeArguments[0], obj);
    }

    public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        return serializer.ValueSerializer.SerializeMultiValue(serializer, objType.GenericTypeArguments[0], obj);
    }
}