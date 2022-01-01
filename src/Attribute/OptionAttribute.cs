using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class OptionAttribute : SpecAttribute
    {
        public string? Name { get; }
        public OptionValueType ValueType { get; }
        
        public OptionAttribute(string? name = null, OptionValueType valueType = OptionValueType.Single, Type? valueSerializerType = null)
            : base(valueSerializerType)
        {
            if (name != null)
            {
                if (name.Length > 1)
                {
                    if (!ImplUtils.IsValidOptionLongName(name))
                        throw ImplUtils.NewInvalidOptionLongNameException(name, nameof(name));
                }
                else
                {
                    if (!ImplUtils.IsValidOptionShortName(name))
                        throw ImplUtils.NewInvalidOptionShortNameException(name, nameof(name));
                }
            }

            Name = name;
            ValueType = valueType;
        }

        public override Spec GenerateSpec(string fieldName, Type objType)
        {
            IValueSerializer? serializer = null;
            if (ValueSerializerType != null)
            {
                serializer = (IValueSerializer)Activator.CreateInstance(ValueSerializerType);
            }

            return new Option(Name ?? fieldName, ValueType, objType, serializer);
        }
    }
}