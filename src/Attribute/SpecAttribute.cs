using System;

namespace Decoherence.CommandLineSerialization.Attributes
{
    public abstract class SpecAttribute : Attribute
    {
        public IValueSerializer? Serializer => 
            mValueSerializerType == null ? null : (IValueSerializer)Activator.CreateInstance(mValueSerializerType);

        private readonly Type? mValueSerializerType;

        protected SpecAttribute(Type? valueSerializerType)
        {
            if (valueSerializerType != null && ImplUtils.IsValidValueSerializerType(valueSerializerType))
                throw ImplUtils.NewInvalidValueSerializerTypeException(valueSerializerType, nameof(valueSerializerType));
            
            mValueSerializerType = valueSerializerType;
        }
    }
}