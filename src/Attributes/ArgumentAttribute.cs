using System;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Decoherence.CommandLineSerialization.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    public class ArgumentAttribute : SpecAttribute
    {
        public int Priority { get; set; }
        
        public override ValueType ValueType
        {
            get => mValueType;
            set
            {
                if (value != ValueType.Default && !DebugUtil.IsValidArgumentValueType(value))
                    throw new ArgumentException(DebugUtil.InvalidArgumentValueTypeError(value));

                mValueType = value;
            }
        }
        
        private ValueType mValueType;
    }
}