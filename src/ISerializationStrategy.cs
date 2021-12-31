using System;

namespace Decoherence.CommandLineSerialization
{
    public interface ISerializationStrategy
    {
        object? DeserializeValue(Type valueType, string? value);
    }
}