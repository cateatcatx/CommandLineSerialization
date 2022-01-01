using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class OptionAttribute : SpecAttribute
    {
        public string? LongName { get; }
        public char? ShortName { get; }
        public OptionValueType ValueType { get; }

        public OptionAttribute(string longName, OptionValueType valueType, Type? valueSerializerType = null)
            : base(valueSerializerType)
        {
            if (!ImplHelper.IsValidOptionLongName(longName))
                throw ImplHelper.NewInvalidOptionLongNameException(longName, nameof(longName));
            
            LongName = longName;
            ValueType = valueType;
        }

        public OptionAttribute(char shortName, OptionValueType valueType, Type? valueSerializerType = null)
            : base(valueSerializerType)
        {
            if (!ImplHelper.IsValidOptionShortName(shortName))
                throw ImplHelper.NewInvalidOptionShortNameException(shortName, nameof(shortName));
            
            ShortName = shortName;
            ValueType = valueType;
        }

        public override Spec GenerateSpec(Type objType)
        {
            IValueSerializer? serializer = null;
            if (ValueSerializerType != null)
            {
                serializer = (IValueSerializer)Activator.CreateInstance(ValueSerializerType);
            }

            return LongName != null ? new Option(LongName, ValueType, objType, serializer) : new Option(ShortName!.Value, ValueType, objType, serializer);
        }
    }
}