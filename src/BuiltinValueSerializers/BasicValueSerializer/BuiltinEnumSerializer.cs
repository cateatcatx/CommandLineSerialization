using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization;

public class BuiltinEnumSerializer : IValueSerializer
{
    public bool CanHandleType(Type objType)
    {
        return objType.IsEnum;
    }

    public ObjectSpecs? GetObjectSpecs(Type objType)
    {
        return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Single, objType), objType);
    }

    public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
    {
        throw new NotSupportedException();
    }

    public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
    {
        if (value == null)
            throw new InvalidOperationException();

        return Enum.Parse(objType, value);
    }

    public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
    {
        throw new NotSupportedException();
    }

    public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        throw new NotSupportedException();
    }

    public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        if (obj == null)
            throw new InvalidOperationException();
            
        return obj.ToString();
    }

    public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
    {
        throw new NotSupportedException();
    }
}