using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface IValueSerializer
    {
        bool CanHandleType(Type objType);
        
        object? DeserializeNonValue(CommandLineSerializer serializer, Type objType);
        object? DeserializeSingleValue(CommandLineSerializer serializer, Type objType, string? value);
        object? DeserializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, LinkedList<string> argList);
        object? DeserializeMultiValue(CommandLineSerializer serializer, Type objType, List<string> values);

        bool SerializeNonValue(CommandLineSerializer serializer, Type objType, object? obj);
        string SerializeSingleValue(CommandLineSerializer serializer, Type objType, object? obj);
        LinkedList<string> SerializeSplitedSingleValue(CommandLineSerializer serializer, Type objType, object? obj);
        IEnumerable<string> SerializeMultiValue(CommandLineSerializer serializer, Type objType, object? obj);
    }
}