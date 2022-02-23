using System;

namespace Decoherence.CommandLineSerialization
{
    public class Option : Spec, IOption
    {
        public string? LongName { get; }
        public char? ShortName { get; }

        public Option(
            char? shortName,
            string? longName,
            ValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            ThrowUtil.ThrowIfArgument(shortName == null && string.IsNullOrWhiteSpace(longName), 
                $"{nameof(shortName)} and {nameof(longName)} can not both be null or empty.");

            if (shortName != null && !DebugUtil.IsValidOptionShortName(shortName.ToString()))
                throw new ArgumentException(DebugUtil.InvalidOptionShortNameError(shortName.ToString()), nameof(shortName));
            
            if (longName != null && !DebugUtil.IsValidOptionLongName(longName))
                throw new ArgumentException(DebugUtil.InvalidOptionLongNameError(longName), nameof(longName));

            if (!DebugUtil.IsValidOptionValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidOptionValueTypeError(valueType), nameof(valueType));

            LongName = longName;
            ShortName = shortName;
        }
        
        public Option(
            string name,
            ValueType valueType, 
            Type objType,
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueSerializer)
        {
            if (name.Length > 1)
            {
                if (!DebugUtil.IsValidOptionLongName(name))
                    throw new ArgumentException(DebugUtil.InvalidOptionLongNameError(name), nameof(name));

                LongName = name;
            }
            else
            {
                if (!DebugUtil.IsValidOptionShortName(name))
                    throw new ArgumentException(DebugUtil.InvalidOptionShortNameError(name), nameof(name));

                ShortName = name[0];
            }

            if (!DebugUtil.IsValidOptionValueType(valueType))
                throw new ArgumentException(DebugUtil.InvalidOptionValueTypeError(valueType), nameof(valueType));
        }
    }
}