using System;
using System.Collections.Generic;
// ReSharper disable ReturnTypeCanBeNotNullable

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinStringSerializer : IValueSerializer
    {
        public bool CanHandleType(Type objType)
        {
            return objType == typeof(string);
        }

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Single, objType), objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            return value;
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }
    }
}