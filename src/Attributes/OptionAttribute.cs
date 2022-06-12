using System;

namespace Decoherence.CommandLineSerialization.Attributes;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
public class OptionAttribute : SpecAttribute
{
    public string? ShortName
    {
        get => mShortName;
        set
        {
            if (value != null && !DebugUtil.IsValidOptionShortName(value))
                throw new ArgumentException(DebugUtil.InvalidOptionShortNameError(value));

            mShortName = value;
        }
    }
        
    public string? LongName
    {
        get => mLongName;
        set
        {
            if (value != null && !DebugUtil.IsValidOptionLongName(value))
                throw new ArgumentException(DebugUtil.InvalidOptionLongNameError(value));

            mLongName = value;
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
        
    private string? mLongName;
    private string? mShortName;
    private ValueType mValueType;
}