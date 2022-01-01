using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class BuiltinObjectSerializer : IValueSerializer
    {
        public object? DeserializeNoneValue(Type objType)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeSingleValue(Type objType, string? value)
        {
            throw new InvalidOperationException();
        }

        public object? DeserializeMultiValue(Type objType, List<string> values)
        {
            throw new InvalidOperationException();
        }
    }
}