using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public class ArgumentAttribute : SpecAttribute
    {
        public ArgumentValueType ValueType { get; }

        public ArgumentAttribute(ArgumentValueType valueType, Type? valueSerializerType = null) 
            : base(valueSerializerType)
        {
            ValueType = valueType;
        }

        public override Spec GenerateSpec(Type objType)
        {
            return new Argument(ValueType, objType, ValueSerializerType != null ? (IValueSerializer)Activator.CreateInstance(ValueSerializerType) : null);
        }
    }
}