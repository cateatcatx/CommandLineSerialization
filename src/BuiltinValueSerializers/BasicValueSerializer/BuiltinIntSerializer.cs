using System;
using System.Collections.Generic;
// ReSharper disable ReturnTypeCanBeNotNullable

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinIntSerializer : IValueSerializer
    {
        public bool CanHandleType(Type objType)
        {
            return objType == typeof(int);
        }

        public ObjectSpecs? GetObjectSpecs(Type objType)
        {
            return ObjectSpecs.NewSingleSpec(new Argument(ValueType.Single, objType), objType);
        }

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType, bool matched)
        {
            return matched ? 1 : 0;
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (obj is not int)
                throw new InvalidOperationException();
            
            return obj.Equals(1);
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (obj is not int)
                throw new InvalidOperationException();
            
            return obj.ToString();
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }
    }
}