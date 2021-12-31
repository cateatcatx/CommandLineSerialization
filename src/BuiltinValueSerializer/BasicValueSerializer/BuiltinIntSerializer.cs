using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinIntSerializer : IValueSerializer
    {
        public object? DeserializeNoneValue(Type objType)
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

        public object? DeserializeWhenNoMatch(Type objType)
        {
            return 0;
        }
    }
}