using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface IValueSerializer
    {
        bool CanHandleType(Type objType);
        
        object? DeserializeNonValue(CommandLineDeserializer deserializer, Type objType);
        object? DeserializeSingleValue(CommandLineDeserializer deserializer, Type objType, string? value);
        object? DeserializeSplitedSingleValue(CommandLineDeserializer deserializer, Type objType, LinkedList<string> argList);
        object? DeserializeMultiValue(CommandLineDeserializer deserializer, Type objType, List<string> values);
    }
}