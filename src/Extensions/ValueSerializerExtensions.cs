using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public static class ValueSerializerExtensions
    {
        public static object? DeserializeSplitedSingleValue(this IValueSerializer self, 
            CommandLineSerializer serializer, 
            Type objType, 
            IEnumerable<string> args, 
            out LinkedList<string> remainArgs)
        {
            remainArgs = new LinkedList<string>(args);
            return self.DeserializeSplitedSingleValue(serializer, objType, remainArgs);
        }
    }
}