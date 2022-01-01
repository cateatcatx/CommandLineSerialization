using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface IValueSerializer
    {
        bool CanHandleType(Type objType);
        
        object? DeserializeNoneValue(Type objType);
        object? DeserializeSingleValue(Type objType, string? value);
        object? DeserializeMultiValue(Type objType, List<string> values);
    }
}