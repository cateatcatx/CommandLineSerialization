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

        public object? DeserializeNonValue(CommandLineSerializer serializer, Type objType)
        {
            return 1;
        }

        public object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
        }

        public object? DeserializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, LinkedList<string> argList)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }

        public bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            return true;
        }

        public string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            if (obj is not int)
                throw new InvalidOperationException();
            
            return obj.ToString();
        }

        public LinkedList<string> SerializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }

        public IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj)
        {
            throw new InvalidOperationException();
        }
    }
}