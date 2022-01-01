using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class OptionAttribute : SpecAttribute
    {
        public string? Name { get; set; }
        public OptionValueType? ValueType { get; set; }
        
        public OptionAttribute(Type? valueSerializerType = null)
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
    }
}