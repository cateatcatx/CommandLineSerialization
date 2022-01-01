using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class ArgumentAttribute : SpecAttribute
    {
        public override ValueType ValueType
        {
            get => mValueType;
            set
            {
                if (value != ValueType.Default && !ImplUtils.IsValidArgumentValueType(value))
                    throw new ArgumentException(ImplUtils.InvalidArgumentValueTypeError(value));

                mValueType = value;
            }
        }
    }
}