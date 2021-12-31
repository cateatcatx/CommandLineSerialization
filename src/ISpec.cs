using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public interface ISpec
    {
        Type objType { get; }
        
        object? DeserializeNoneValue();
        object? DeserializeSingleValue(string? value);
        object? DeserializeMultiValue(List<string> values);
        object? DeserializeWhenNoMatch();
    }
}