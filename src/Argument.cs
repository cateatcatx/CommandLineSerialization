using System;
using System.Collections.Generic;

namespace Decoherence.CommandLineSerialization
{
    public class Argument : Spec, IArgument
    {
        public int Priority { get; }
        
        public Argument(int priority, ValueType valueType, Type objType, IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            if (!DebugUtil.IsValidArgumentValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidArgumentValueTypeError(valueType), nameof(valueType));

            Priority = priority;
        }
    }
}