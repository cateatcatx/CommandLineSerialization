using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public abstract class SpecAttribute : Attribute
    {
        public Type? ValueSerializerType { get; }

        protected SpecAttribute(Type? valueSerializerType)
        {
            if (valueSerializerType != null && ImplUtils.IsValidValueSerializerType(valueSerializerType))
                throw ImplUtils.NewInvalidValueSerializerTypeException(valueSerializerType, nameof(valueSerializerType));
            
            ValueSerializerType = valueSerializerType;
        }

        public abstract Spec GenerateSpec(string fieldName, Type objType);
    }
}