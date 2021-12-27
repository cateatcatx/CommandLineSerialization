using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class ValueSerializer
    {
        public Func<CommandLineSerializer, Type, object?, string>? SerializeFunc;
        public Func<CommandLineSerializer, Type, IEnumerable<string>, object?>? DeserializeFunc;
        
        public ValueSerializer(Func<CommandLineSerializer, Type, object?, string>? serializeFunc,
            Func<CommandLineSerializer, Type, IEnumerable<string>, object?>? deserializeFunc)
        {
            SerializeFunc = serializeFunc;
            DeserializeFunc = deserializeFunc;
        }
    }
}