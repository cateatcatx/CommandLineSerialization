using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinIntSerializer : IValueSerializer
    {
        public bool CanHandleType(Type objType)
        {
            return objType == typeof(int);
        }

        public object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
        }

        public object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (obj is not int)
                throw new InvalidOperationException();
            
            return obj.ToString();
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new NotImplementedException();
        }
    }
}