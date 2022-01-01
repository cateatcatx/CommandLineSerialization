using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class ArgumentAttribute : SpecAttribute
    {
        public ArgumentValueType? ValueType { get; }

        public ArgumentAttribute(ArgumentValueType? valueType = null, Type? valueSerializerType = null) 
            : base(valueSerializerType)
        {
            ValueType = valueType;
        }
    }
}