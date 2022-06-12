using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization;

public class BuiltinBoolSerializer : IValueSerializer
{
    public bool CanHandleType(Type objType)
    {
        return objType == typeof(bool);
    }

    public ObjectSpecs? GetObjectSpecs(Type objType)
    {
        return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Single, objType), objType);
    }

    public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
    {
        return matched;
    }

    public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
    {
        return value != null && string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0;
    }

    public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
    {
        throw new NotSupportedException();
    }

    public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        return obj != null && (bool)obj == true;
    }

    public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        return obj != null && (bool)obj == true ? "true" : "false";
    }

    public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        throw new NotSupportedException();
    }
}