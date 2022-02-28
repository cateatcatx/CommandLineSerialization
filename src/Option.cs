using System;
using System.Text;

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
            string? valueName = null,
            string? desc = null,
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueName, desc, valueSerializer)
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
            string? valueName = null, 
            string? desc = null,
            IValueSerializer? valueSerializer = null)
            : base(valueType, objType, valueName, desc, valueSerializer)
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

        private void _AppendHead(StringBuilder sb)
        {
            bool hasBracket = ShortName != null && LongName != null && ValueType != ValueType.Non;
            
            if (hasBracket)
            {
                sb.Append("(");
            }
            
            if (ShortName != null)
            {
                sb.Append($"-{ShortName}");
            }
            if (LongName != null)
            {
                sb.Append($"{(ShortName != null ? " | " : string.Empty)}--{LongName}");
            }

            if (hasBracket)
            {
                sb.Append(")");
            }

            if (ValueType != ValueType.Non)
            {
                sb.Append($" <{ValueName}>");
            }
        }

        public override string GetDrawUsageHead()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("[");
            _AppendHead(sb);
            sb.Append("]");

            return sb.ToString();
        }

        public override string GetDrawExplainHead()
        {
            StringBuilder sb = new StringBuilder();

            _AppendHead(sb);

            return sb.ToString();
        }
    }
}