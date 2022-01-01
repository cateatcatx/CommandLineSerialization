using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class OptionAttribute : SpecAttribute
    {
        public string? Name
        {
            get => mName;
            set
            {
                if (value != null)
                {
                    if (value.Length > 1)
                    {
                        if (!DebugUtil.IsValidOptionLongName(value))
                            throw new ArgumentException(DebugUtil.InvalidOptionLongNameError(value));
                    }
                    else
                    {
                        if (!DebugUtil.IsValidOptionShortName(value))
                            throw new ArgumentException(DebugUtil.InvalidOptionShortNameError(value));
                    }
                }

                mName = value;
            }
        }
        
        public override ValueType ValueType
        {
            get => mValueType;
            set
            {
                if (value != ValueType.Default && !DebugUtil.IsValidOptionValueType(value))
                    throw new ArgumentException(DebugUtil.InvalidOptionValueTypeError(value));

                mValueType = value;
            }
        }

        private string? mName;
    }
}