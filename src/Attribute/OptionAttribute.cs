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
                        if (!ImplUtils.IsValidOptionLongName(value))
                            throw new ArgumentException(ImplUtils.InvalidOptionLongNameError(value));
                    }
                    else
                    {
                        if (!ImplUtils.IsValidOptionShortName(value))
                            throw new ArgumentException(ImplUtils.InvalidOptionShortNameError(value));
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
                if (value != ValueType.Default && !ImplUtils.IsValidOptionValueType(value))
                    throw new ArgumentException(ImplUtils.InvalidOptionValueTypeError(value));

                mValueType = value;
            }
        }

        private string? mName;
    }
}