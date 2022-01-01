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

        public object? DeserializeNonValue(Type objType)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(Type objType, string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? 0 : int.Parse(value);
        }

        public object? DeserializeMultiValue(Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }
    }
}