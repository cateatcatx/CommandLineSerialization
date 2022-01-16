using System;

namespace Decoherence.CommandLineSerialization
{
    public class Argument : Spec, IArgument
    {
        public int Priority { get; }
        
        public Argument(ValueType valueType, Type objType, int priority = 0, IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            if (!DebugUtil.IsValidArgumentValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidArgumentValueTypeError(valueType), nameof(valueType));

            Priority = priority;
        }
    }
}