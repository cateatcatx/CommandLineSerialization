using System;

namespace Decoherence.CommandLineSerialization
{
    public class Argument : Spec, IArgument
    {
        public int Priority { get; }
        
        public Argument(
            ValueType valueType, 
            Type objType, 
            string? valueName = null, 
            string? desc = null, 
            int priority = 0, 
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueName, desc, valueSerializer)
        {
            if (!DebugUtil.IsValidArgumentValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidArgumentValueTypeError(valueType), nameof(valueType));

            Priority = priority;
        }
        
        public override string GetDrawUsageHead()
        {
            return $"<{ValueName}>";
        }

        public override string GetDrawExplainHead()
        {
            return ValueName;
        }
    }
}